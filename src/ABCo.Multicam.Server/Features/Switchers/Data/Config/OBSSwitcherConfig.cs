using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Server.Features.Switchers.Data.Config
{
    public class OBSSwitcherConfig : SwitcherConfig
    {
        public override SwitcherType Type => SwitcherType.OBS;

        public string IP { get; set; }
        public int Port { get; set; }
        public string Password { get; set; }

        public OBSSwitcherConfig(string ip, int port, string password)
        {
            IP = ip;
            Port = port;
            Password = password;
        }
    }
}
