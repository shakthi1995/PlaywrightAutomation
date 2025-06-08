using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaywrightTestScripts.Base
{
    public class Settings
    {
        public static string? environment { get; set; }
        public static string? url { get; set; }

        public static string? browser { get; set; }

        public static bool? headlessMode { get; set; }

        public static string? username { get; set; }

        public static string? password { get; set; }
    }
}
