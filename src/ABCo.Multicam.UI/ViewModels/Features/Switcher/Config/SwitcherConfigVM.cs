using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Features.Switchers;
using ABCo.Multicam.Core.Features.Switchers.Types;
using ABCo.Multicam.Core.Features.Switchers.Types.ATEM;
using ABCo.Multicam.UI.ViewModels.Features.Switcher.Types;
using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;

namespace ABCo.Multicam.UI.ViewModels.Features.Switcher
{
	public interface ISwitcherConfigVM : IParameteredService<SwitcherConfig, ISwitcherFeatureVM>, INotifyPropertyChanged
    {
		string[] Items { get; }
        string SelectedItem { get; set; }
		ISpecificSwitcherConfigVM? CurrentConfig { get; }
		SwitcherType SwitcherType { get; }
	}

    public interface ISpecificSwitcherConfigVM
    {

    }

    public partial class SwitcherConfigVM : ViewModelBase, ISwitcherConfigVM
    {
        bool _initialized = false;

        ISpecificSwitcherConfigVMFactory _configVMFactory;
        ISwitcherFeatureVM _parent = null!;
        IServiceSource _servSource;

        [ObservableProperty] string[] _items = new string[]
        {
            "Dummy",
            "ATEM"
        };

        [ObservableProperty] string _selectedItem = "Dummy";
        [ObservableProperty] ISpecificSwitcherConfigVM? _currentConfig;
        [ObservableProperty] SwitcherType _switcherType;

        public static ISwitcherConfigVM New(SwitcherConfig config, ISwitcherFeatureVM parent, IServiceSource servSource) => new SwitcherConfigVM(config, parent, servSource);
        public SwitcherConfigVM(SwitcherConfig config, ISwitcherFeatureVM parent, IServiceSource servSource)
        {
            _servSource = servSource;
            _configVMFactory = servSource.Get<ISpecificSwitcherConfigVMFactory>();

			_parent = parent;

			// Update the selected item
			_switcherType = config.Type;
			SelectedItem = config.Type switch
			{
				SwitcherType.ATEM => "ATEM",
				_ => "Dummy"
			};

			CurrentConfig = _configVMFactory.Create(config, parent);
			_initialized = true;
		}

        partial void OnSelectedItemChanged(string value)
        {
            if (!_initialized) return;

            _parent.UpdateConfig(value switch
            {
                "Dummy" => new DummySwitcherConfig(),
                "ATEM" => new ATEMSwitcherConfig(),
                _ => throw new Exception("Unsupported combo box item")
            });
        }
    }

    public record class SwitcherConfigComboItemVM(SwitcherType Type, string Text, int Id);

    public interface ISpecificSwitcherConfigVMFactory
    {
        ISpecificSwitcherConfigVM Create(SwitcherConfig config, ISwitcherFeatureVM parent);
    }

    public class SpecificSwitcherConfigVMFactory : ISpecificSwitcherConfigVMFactory
    {
        public ISpecificSwitcherConfigVM Create(SwitcherConfig config, ISwitcherFeatureVM parent)
        {
            return config switch
            {
                DummySwitcherConfig dummyConfig => new DummySwitcherConfigVM(dummyConfig, parent),
                ATEMSwitcherConfig atemConfig => new TempEmptyVM(),
                _ => throw new Exception("Unrecognised config")
            };
        }
    }

    class TempEmptyVM : ViewModelBase, ISpecificSwitcherConfigVM
	{
        
    }
}
