using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestWriting.Model
{
    public class User
    {
        

        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public virtual string UniqueNumber { get; set;}

        public override string ToString()
        {
            return LastName + " " + FirstName;
        }

    }
}
