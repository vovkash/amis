using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestWriting.Model;

namespace TestWriting.ViewModel
{
    public class WorkWritingViewModel : PropertyChangedMainViewModel
    {
        WorkAssignment workAssignment;

        public WorkAssignment WorkAssignment
        {
          get { return workAssignment; }
          set 
          { 
              workAssignment = value;

              NotifyPropertyChanged("WorkAssignment");
              NotifyPropertyChanged("Task");
              NotifyPropertyChanged("StudentAnswer");
              NotifyPropertyChanged("MaxPoint");
              NotifyPropertyChanged("SettedPoint");
          }
        }

        public string Task
        {
            get 
            {
                return WorkAssignment.ExerciseText;
            }
        }

        public string StudentAnswer
        {
            get 
            {
                return WorkAssignment.StudentAnswer;
            }

            set 
            {
                WorkAssignment.StudentAnswer = value;
                NotifyPropertyChanged("StudentAnswer");
            }
        
        }

        public int MaxPoint
        {
            get
            {
                return WorkAssignment.ExerciseMaxPoint;
            }
        }

        public int SettedPoint
        {
            get
            {
                return WorkAssignment.WorkPoint;
            }

            set
            {
                if (value > MaxPoint || value < 0)
                    throw new Exception("Point should be positive and not more than max point!"); 

                WorkAssignment.WorkPoint = value;
                NotifyPropertyChanged("SettedPoint");
            }

        }

        public string TeacherAnswer
        {
            get
            {
                return workAssignment.TeacherAnswer;
            }
        
        }


        public WorkWritingViewModel() :
            this(new WorkAssignment())
        {}

        public WorkWritingViewModel(WorkAssignment workAssignment)
        {
            this.WorkAssignment = workAssignment;
        }



        public void SaveWork()
        { 
            using (WorkAssignmentRepository war = new WorkAssignmentRepository())
            {
                war.Update(WorkAssignment);
            }
        }
    }
}
