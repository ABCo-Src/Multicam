using ABCo.Multicam.Core;
using ABCo.Multicam.UI.Enumerations;
using ABCo.Multicam.UI.Helpers;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.UI.ViewModels.Features.Switcher
{
    public interface ISwitcherButtonViewModel
    {
        string Text { get; set; }
        SwitcherButtonStatus Status { get; set; }
    }

    public abstract partial class SwitcherButtonViewModel : ViewModelBase, ISwitcherButtonViewModel
    {
        protected ISwitcherMixBlockVM _parent = null!;

        [ObservableProperty] string _text = "";
        [ObservableProperty] SwitcherButtonStatus _status;

        public abstract void Click();
    }
}
