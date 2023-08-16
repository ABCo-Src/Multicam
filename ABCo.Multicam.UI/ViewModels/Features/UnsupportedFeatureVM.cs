using ABCo.Multicam.Core.Features;
using ABCo.Multicam.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ABCo.Multicam.UI.Bindings;

namespace ABCo.Multicam.UI.ViewModels.Features
{
    public interface IUnsupportedFeatureViewModel : ILiveFeatureViewModel { }
    public class UnsupportedFeatureVM : ViewModelBase, IUnsupportedFeatureViewModel { }
}
