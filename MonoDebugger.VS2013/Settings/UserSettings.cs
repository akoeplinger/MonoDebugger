using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoDebugger.VS2013.Settings
{
    public class UserSettings
    {
        public string LastIp { get; set; }

        public UserSettings()
        {
            LastIp = "127.0.0.1";
        }
    }
}
