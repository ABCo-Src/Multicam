using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.UI.Services
{
    public interface IUIPlatformWindowCapabilities
    {
        bool CanMinimize { get; }
        bool CanMaximize { get; }
        bool CloseBtnRecommended { get; }
        bool BorderRecommended { get; }
    }
}
