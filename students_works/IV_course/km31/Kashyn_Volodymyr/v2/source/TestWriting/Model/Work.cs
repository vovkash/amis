using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestWriting.Model
{
    public class Work
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Subject { get; set; }
        public string Theme { get; set; }
        public string Teacher { get; set; }

        public List<WorkExercise> WorkExercises { get; set; }
    }
}
