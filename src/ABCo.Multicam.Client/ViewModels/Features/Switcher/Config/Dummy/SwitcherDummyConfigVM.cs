using ABCo.Multicam.Server;
using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;
using ABCo.Multicam.Client.Presenters.Features.Switchers.Config;

namespace ABCo.Multicam.Client.ViewModels.Features.Switcher.Types
{
    public interface ISwitcherDummyConfigVM : ISwitcherSpecificConfigVM, IClientService<ISwitcherDummyConfigPresenter>, INotifyPropertyChanged
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
        [ObservableProperty] ISwitcherDummyConfigMixBlockVM[] _mixBlockVMs = Array.Empty<ISwitcherDummyConfigMixBlockVM>();

		public SwitcherDummyConfigVM(ISwitcherDummyConfigPresenter presenter) => _presenter = presenter;

        public void MixBlockCountChange() => _presenter.OnUIChange();
    }

    public interface ISwitcherDummyConfigMixBlockVM : IClientService<ISwitcherDummyConfigPresenter>
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
