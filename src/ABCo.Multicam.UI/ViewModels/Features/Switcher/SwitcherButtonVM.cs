using ABCo.Multicam.Core;
using ABCo.Multicam.UI.Enumerations;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.UI.ViewModels.Features.Switcher
{
    public interface ISwitcherButtonVM
    {
        string Text { get; set; }
        SwitcherButtonStatus Status { get; set; }
    }

    public abstract partial class SwitcherButtonVM : ViewModelBase, ISwitcherButtonVM
    {
        protected ISwitcherMixBlockVM _parent = null!;

        [ObservableProperty] string _text = "";
        [ObservableProperty] SwitcherButtonStatus _status;

        public abstract void Click();
    }
}
