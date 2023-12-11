using ABCo.Multicam.Server;
using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;

namespace ABCo.Multicam.Client.Presenters.Features.Switchers.Config.Virtual
{
	public interface ISwitcherVirtualConfigMixBlockVM : INotifyPropertyChanged
	{
		int[] InputCountItems { get; }
		string InputCount { get; set; }
		int InputIndex { get; }
		void InputCountChange();
	}

	public partial class SwitcherVirtualConfigMixBlockVM : ViewModelBase, ISwitcherVirtualConfigMixBlockVM
	{
		readonly ISwitcherVirtualConfigVM _parent;

		[ObservableProperty] int[] _inputCountItems = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
		[ObservableProperty] int _inputIndex;
		[ObservableProperty] string _inputCount;

		public SwitcherVirtualConfigMixBlockVM(ISwitcherVirtualConfigVM parent, int count, int index)
		{
			_parent = parent;
			_inputCount = count.ToString();
			_inputIndex = index + 1;
		}

		public void InputCountChange() => _parent.OnUIChange();
	}
}
