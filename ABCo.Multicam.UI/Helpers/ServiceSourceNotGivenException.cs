using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.UI.Helpers
{
    public class ServiceSourceNotGivenException : Exception
    {
        public ServiceSourceNotGivenException() : base("A view-model was created without a service source being given.") { }
    }
}
