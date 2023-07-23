using ABCo.Multicam.Core.Structures;
using ABCo.Multicam.Core.Switchers;
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

        void CreateStrip(StripTypes type);
        void SetStripsChangeForVM(Action act);
        void MoveUp(IRunningStrip strip);
        void MoveDown(IRunningStrip strip);
        void Delete(IRunningStrip strip);
    }

    public class StripManager : IStripManager
    {
        IServiceSource _servSource;
        Action? _onStripsChange;
        List<IRunningStrip> _runningStrips = new();

        public IReadOnlyList<IRunningStrip> Strips => _runningStrips;

        public StripManager(IServiceSource source) => _servSource = source;

        public void SetStripsChangeForVM(Action act) => _onStripsChange = act;

        public void CreateStrip(StripTypes type)
        {
            _runningStrips.Add(GetStripFromType(type));
            _onStripsChange?.Invoke();
        }

        IRunningStrip GetStripFromType(StripTypes type)
        {
            return type switch
            {
                StripTypes.Switcher => _servSource.Get<ISwitcherRunningStrip>(),
                _ => _servSource.Get<IUnsupportedRunningStrip>()
            };
        }

        public void MoveUp(IRunningStrip strip)
        {
            int indexOfStrip = _runningStrips.IndexOf(strip);

            // Don't do anything if it's at the start
            if (indexOfStrip == 0) return;

            (_runningStrips[indexOfStrip], _runningStrips[indexOfStrip - 1]) = (_runningStrips[indexOfStrip - 1], _runningStrips[indexOfStrip]);

            _onStripsChange?.Invoke();
        }

        public void MoveDown(IRunningStrip strip)
        {
            int indexOfStrip = _runningStrips.IndexOf(strip);

            // Don't do anything if it's at the end
            if (indexOfStrip == _runningStrips.Count - 1) return;

            (_runningStrips[indexOfStrip], _runningStrips[indexOfStrip + 1]) = (_runningStrips[indexOfStrip + 1], _runningStrips[indexOfStrip]);

            _onStripsChange?.Invoke();
        }

        public void Delete(IRunningStrip strip)
        {
            _runningStrips.Remove(strip);
            _onStripsChange?.Invoke();
        }
    }
}
