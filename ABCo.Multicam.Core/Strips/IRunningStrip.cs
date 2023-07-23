using ABCo.Multicam.Core.Strips.Switchers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Core.Strips
{
    /// <summary>
    /// Represents a strip currently running
    /// </summary>
    public interface IRunningStrip : IDisposable
    {
        
    }

    public interface IUnsupportedRunningStrip : IRunningStrip { }

    public class UnsupportedRunningStrip : IUnsupportedRunningStrip
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
