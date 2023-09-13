using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Features.Switchers.Types;
using ABCo.Multicam.UI.Presenters.Features.Switcher.Config;
using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;

namespace ABCo.Multicam.UI.ViewModels.Features.Switcher.Types
{
	public interface ISwitcherDummyConfigVM : ISwitcherSpecificConfigVM, IParameteredService<ISwitcherDummyConfigPresenter>, INotifyPropertyChanged
    {
        string SelectedMixBlockCount { get; set; }
		int[] MixBlockCountOptions { get; }
		ISwitcherDummyConfigMixBlockVM[] MixBlockVMs { get; set; }
    }

    public partial class SwitcherDummyConfigVM : ViewModelBase, ISwitcherDummyConfigVM
    {
        readonly ISwitcherDummyConfigPresenter _presenter;
        public int[] MixBlockCountOptions => new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

        [ObservableProperty] string _selectedMixBlockCount = "1";
        [ObservableProperty] ISwitcherDummyConfigMixBlockVM[] _mixBlockVMs = null!;

		public SwitcherDummyConfigVM(ISwitcherDummyConfigPresenter presenter) => _presenter = presenter;

        public void MixBlockCountChange() => _presenter.OnUIChange();
    }

    public interface ISwitcherDummyConfigMixBlockVM : IParameteredService<ISwitcherDummyConfigPresenter>
    { 
        string InputCount { get; set; }
        int InputIndex { get; set; }
        void InputCountChange();
    }

    public partial class DummySwitcherConfigMixBlockVM : ViewModelBase, ISwitcherDummyConfigMixBlockVM
    {
		readonly ISwitcherDummyConfigPresenter _presenter;

        [ObservableProperty] int[] _inputCountItems = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        [ObservableProperty] int _inputIndex;
        [ObservableProperty] string _inputCount;

		public DummySwitcherConfigMixBlockVM(ISwitcherDummyConfigPresenter presenter) => _presenter = presenter;

		public void InputCountChange() => _presenter.OnUIChange();
    }
}
