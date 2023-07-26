using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.UI.ViewModels.Strips
{
    public interface IStripViewModelFactory
    {
        StripViewModel Create<T>(T strip, IProjectStripsViewModel parent) where T : IStripViewModel;
    }

    // WARNING: This class acts as a boundary between vms and is not unit-tested (as mocking needed parameters is too difficult).
    // Be careful making changes here, always check your changed function works in all cases!
    // In the future, we *may* be able to create a shared IServiceSource mock in unit tests to create intelligent mocks of params so this class be removed (maybe this would help?)
    public class StripViewModelFactory : IStripViewModelFactory
    {
        public StripViewModel Create<T>(T strip, IProjectStripsViewModel parent) where T : IStripViewModel
        {
            throw new NotImplementedException();
        }
    }
}
