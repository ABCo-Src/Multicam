using ABCo.Multicam.Core.Features.Switchers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Core.Features
{
    /// <summary>
    /// Represents a feature currently running
    /// </summary>
    public interface IRunningFeature : IDisposable
    {
        
    }

    public interface IUnsupportedRunningFeature : IRunningFeature { }

    public class UnsupportedRunningFeature : IUnsupportedRunningFeature
    {
        public void Dispose() { }
    }
}
