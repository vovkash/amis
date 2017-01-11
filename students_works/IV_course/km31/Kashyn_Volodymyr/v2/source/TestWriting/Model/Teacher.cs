using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestWriting.Model
{
    public class Teacher : User
    {
        public string AgreementId{ get; set;}

        public override string UniqueNumber
        {
            get
            {
                return AgreementId;
            }
            set
            {
                AgreementId = value;
            }
        }

    }
}
