using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestWriting.Model
{
    public class WorkAssignment
    {
        public string WorkName { get; set; }
        public string ExerciseName { get; set; }
        public string ExerciseText { get; set; }
        public int    ExerciseMaxPoint { get; set; }
        public string Subject { get; set; }
        public string Theme { get; set; }
        public string Teacher { get; set; }
        public string StudentNum { get; set; }
        public string TeacherAnswer { get; set; }
        public string StudentAnswer { get; set; }
        public int    WorkPoint { get; set; }
        public string Status { get; set; }
        


        public string TeacherName
        {
            get
            {
                string name;
                using (TeacherRepository tr = new TeacherRepository())
                {
                    Teacher t = tr.FindByAgreementId(this.Teacher);
                    if (t == null)
                        name = "";
                    else
                        name = t.ToString();
                }

                return name;
            }
        
        }


        public string StudentName
        {
            get 
            {
                string name;

                using (StudentRepository sr = new StudentRepository())
                {
                    Student st = sr.FindByStudentNumber(this.StudentNum);
                    if (st == null)
                        name = "";
                    else
                        name = st.ToString();
                }

                return name;
            }

        
        }

        public int MaxPoints
        {
            get 
            {
                using (WorkExerciseRepository weRep = new WorkExerciseRepository())
                {
                    return (weRep.FindByIndex(this.WorkName, this.ExerciseName, this.Subject, this.Theme, this.Teacher)).Points;
                }
            }
        
        }

        public WorkAssignment()
        {
            this.WorkName = "";
            this.ExerciseName = "";
            this.Subject = "";
            this.Theme = "";
            this.Teacher = "";
        
        }

        public WorkAssignment(WorkExercise we)
        {
            this.WorkName       = we.WorkName;
            this.ExerciseName   = we.ExerciseName;
            this.Subject        = we.Subject;
            this.Theme          = we.Theme;
            this.Teacher        = we.Teacher;
        }



    }
}
