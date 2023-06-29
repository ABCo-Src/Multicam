using ABCo.Multicam.UI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.UI.Avalonia.Services
{
    public class DesktopUIPlatformWindowCapabilities : IUIPlatformWindowCapabilities
    {
        public bool CanMinimize => true;
        public bool CanMaximize => true;
        public bool CloseBtnRecommended => true;
        public bool BorderRecommended => true;
    }

    public class WebUIPlatformWindowCapabilities : IUIPlatformWindowCapabilities
    {
        public bool CanMinimize => false;
        public bool CanMaximize => false;
        public bool CloseBtnRecommended => false;
        public bool BorderRecommended => false;
    }
}
