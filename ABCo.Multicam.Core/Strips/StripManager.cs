using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Core.Strips
{
    /// <summary>
    /// Manages all the (running) strips in the current project.
    /// </summary>
    public interface IStripManager
    {
        IReadOnlyList<IRunningStrip> Strips { get; }

        void CreateStrip();
        void MoveUp(IRunningStrip strip);
        void MoveDown(IRunningStrip strip);
        void Delete(IRunningStrip strip);
    }

    public class StripManager : IStripManager
    {
        List<IRunningStrip> _runningStrips = new();

        public IReadOnlyList<IRunningStrip> Strips => _runningStrips;
        
        public void CreateStrip()
        {
            _runningStrips.Add(new DummyRunningStrip());
        }

        public void MoveUp(IRunningStrip strip)
        {
            int indexOfStrip = _runningStrips.IndexOf(strip);

            // Don't do anything if it's at the start
            if (indexOfStrip == 0) return;

            (_runningStrips[indexOfStrip], _runningStrips[indexOfStrip - 1]) = (_runningStrips[indexOfStrip - 1], _runningStrips[indexOfStrip]);
        }

        public void MoveDown(IRunningStrip strip)
        {
            int indexOfStrip = _runningStrips.IndexOf(strip);

            // Don't do anything if it's at the end
            if (indexOfStrip == _runningStrips.Count - 1) return;

            (_runningStrips[indexOfStrip], _runningStrips[indexOfStrip + 1]) = (_runningStrips[indexOfStrip + 1], _runningStrips[indexOfStrip]);
        }

        public void Delete(IRunningStrip strip)
        {
            _runningStrips.Remove(strip);
        }
    }
}
