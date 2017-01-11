using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestWriting.Model
{
    public class Exercise
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public string Subject { get; set; }
        public string Theme { get; set; }

        public string Task { get; set; }
        public string Answer { get; set; }

        public string CreatedBy { get; set; }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
