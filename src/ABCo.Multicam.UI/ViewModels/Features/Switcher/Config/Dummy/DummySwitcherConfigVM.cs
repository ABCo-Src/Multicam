using ABCo.Multicam.Core.Features.Switchers.Types;
using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;

namespace ABCo.Multicam.UI.ViewModels.Features.Switcher.Types
{
	public interface IDummySwitcherConfigVM : ISpecificSwitcherConfigVM, INotifyPropertyChanged
    {
        void UpdateModel();
    }

    public partial class DummySwitcherConfigVM : ViewModelBase, IDummySwitcherConfigVM
    {
        ISwitcherFeatureVM _parent = null!;

        [ObservableProperty] int[] _mixBlockCountOptions = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        [ObservableProperty] int _selectedMixBlockCount;

        [ObservableProperty] IDummySwitcherConfigMixBlockVM[] _mixBlockVMs = null!;

        public DummySwitcherConfigVM(DummySwitcherConfig config, ISwitcherFeatureVM parent) 
        {
            _selectedMixBlockCount = config.MixBlocks.Length;
            _mixBlockVMs = new IDummySwitcherConfigMixBlockVM[config.MixBlocks.Length];
            for (int i = 0; i < config.MixBlocks.Length; i++)
                _mixBlockVMs[i] = new DummySwitcherConfigMixBlockVM(this, i + 1, config.MixBlocks[i]);

            _parent = parent;
        }

        partial void OnSelectedMixBlockCountChanged(int oldValue, int newValue)
        {
            // Resize the array
            var arr = MixBlockVMs;
            Array.Resize(ref arr, newValue);
            MixBlockVMs = arr;

            // Create new items where there weren't any
            for (int i = oldValue; i < newValue; i++)
                MixBlockVMs[i] = new DummySwitcherConfigMixBlockVM(this, i + 1, 1);

            // Update the model to match
            UpdateModel();
        }

        public void UpdateModel()
        {
            int[] arr = new int[SelectedMixBlockCount];

            for (int i = 0; i < arr.Length; i++)
                arr[i] = MixBlockVMs[i].InputCount;

            _parent.UpdateConfig(new DummySwitcherConfig(arr));
        }
    }

    public interface IDummySwitcherConfigMixBlockVM
    { 
        int InputCount { get; set; }
    }

    public partial class DummySwitcherConfigMixBlockVM : ViewModelBase, IDummySwitcherConfigMixBlockVM
    {
        IDummySwitcherConfigVM _parent;
        [ObservableProperty] int _index;

        [ObservableProperty] int[] _inputCountItems = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        [ObservableProperty] int _inputCount;

        public DummySwitcherConfigMixBlockVM(IDummySwitcherConfigVM parent, int inputIdx, int inputCount)
        {
            _parent = parent;
            _index = inputIdx;
            _inputCount = inputCount;
        }

        partial void OnInputCountChanged(int value)
        {
            _parent.UpdateModel();
        }
    }
}
