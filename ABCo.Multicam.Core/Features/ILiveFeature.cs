using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Core.Features
{
    public interface ILiveFeatureBinder { }
    public interface ILiveFeature : IDisposable
    {
        ILiveFeatureBinder UIBinder { get; }
    }
}
