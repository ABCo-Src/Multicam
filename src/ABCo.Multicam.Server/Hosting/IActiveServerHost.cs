using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Server.Hosting
{
    public interface IActiveServerHost
    {
        void Connect(string hostPath);
    }
}
