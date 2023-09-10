using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Features.Switchers.Types;
using ABCo.Multicam.UI.Presenters.Features.Switcher.Config;
using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;

namespace ABCo.Multicam.UI.ViewModels.Features.Switcher.Types
{
	public interface IDummySwitcherConfigVM : ISwitcherSpecificConfigVM, IParameteredService<ISwitcherDummyConfigPresenter>, INotifyPropertyChanged
    {
        string SelectedMixBlockCount { get; set; }
		int[] MixBlockCountOptions { get; }
		IDummySwitcherConfigMixBlockVM[] MixBlockVMs { get; set; }
    }

    public partial class DummySwitcherConfigVM : ViewModelBase, IDummySwitcherConfigVM
    {
        readonly ISwitcherDummyConfigPresenter _presenter;

        public int[] MixBlockCountOptions => new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

        [ObservableProperty] string _selectedMixBlockCount;
        [ObservableProperty] IDummySwitcherConfigMixBlockVM[] _mixBlockVMs = null!;

		public DummySwitcherConfigVM(ISwitcherDummyConfigPresenter presenter) => _presenter = presenter;

        public void MixBlockCountChange() => _presenter.OnChange();
    }

    public interface IDummySwitcherConfigMixBlockVM : IParameteredService<ISwitcherDummyConfigPresenter>
    { 
        string InputCount { get; set; }
        int InputIndex { get; set; }
        void InputCountChange();
    }

    public partial class DummySwitcherConfigMixBlockVM : ViewModelBase, IDummySwitcherConfigMixBlockVM
    {
		readonly ISwitcherDummyConfigPresenter _presenter;

        [ObservableProperty] int[] _inputCountItems = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        [ObservableProperty] int _inputIndex;
        [ObservableProperty] string _inputCount;

		public DummySwitcherConfigMixBlockVM(ISwitcherDummyConfigPresenter presenter) => _presenter = presenter;

		public void InputCountChange() => _presenter.OnChange();
    }
}
