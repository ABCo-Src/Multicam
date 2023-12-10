using ABCo.Multicam.Client.Enumerations;
using ABCo.Multicam.Server.Features.Switchers;
using ABCo.Multicam.Server.Hosting.Clients;
using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;

namespace ABCo.Multicam.Client.ViewModels.Features.Switcher
{
	public interface ISwitcherButtonVM : INotifyPropertyChanged
    {
        string Text { get; set; }
        SwitcherButtonStatus Status { get; set; }
        void Click();
    }

    public abstract partial class SwitcherButtonVM : ViewModelBase, ISwitcherButtonVM
    {
        protected readonly Dispatched<ISwitcher> _switcher;

        [ObservableProperty] string _text;
        [ObservableProperty] SwitcherButtonStatus _status = SwitcherButtonStatus.NeutralActive;

		public SwitcherButtonVM(Dispatched<ISwitcher> switcher, string text)
		{
			_switcher = switcher;
            _text = text;
		}

		public abstract void Click();
        public abstract void UpdateState(MixBlockState state);
	}
}
