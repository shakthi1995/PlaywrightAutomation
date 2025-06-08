using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaywrightTestScripts.Model
{

    public class Userdetails
    {
        public Standard_User? standard_user { get; set; }
        public Problem_User? problem_user { get; set; }
    }

    public class Standard_User
    {
        public string? username { get; set; }
        public string? password { get; set; }
    }

    public class Problem_User
    {
        public string? username { get; set; }
        public string? password { get; set; }
    }

}
