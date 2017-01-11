using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestWriting.Model
{
    public class Student : User
    {
        public string StudentNum {get; set;}
        public string GroupAlias { get; set; }

        public override string UniqueNumber
        {
            get
            {
                return StudentNum;
            }
            set
            {
                StudentNum = value;
            }
        }

        

    }
}
