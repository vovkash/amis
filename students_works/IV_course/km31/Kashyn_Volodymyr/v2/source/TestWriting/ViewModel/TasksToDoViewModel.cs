using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using TestWriting.Model;

namespace TestWriting.ViewModel
{
    public class TasksToDoViewModel : PropertyChangedMainViewModel
    {
        WorkAssignment workAssignment;

        public string WorkName
        {
            get 
            {
                return workAssignment.WorkName;
            }
        }

        public string WorkSubject
        {
            get
            {
                return workAssignment.Subject;
            }
        }

        public string WorkTheme
        {
            get
            {
                return workAssignment.Theme;
            }
        }

        public string WhosWork
        {
            get 
            {
                if (Session.HasRole(Session.Role.Teacher))
                {
                    return workAssignment.StudentName;
                }

                if (Session.HasRole(Session.Role.Student))
                {
                    return workAssignment.TeacherName;
                }

                return "None";
            }
        
        }

        public string Status
        {
            get 
            {
                return workAssignment.Status;
            }
        
        }


        List<WorkAssignment> todoList = new List<WorkAssignment>();

        public List<WorkAssignment> TodoList
        {
            get { return todoList; }
            set
            {
                todoList = value;
                NotifyPropertyChanged("TodoList");
            }
        }



        public TasksToDoViewModel()
        {
            workAssignment = new WorkAssignment();
        }

        public TasksToDoViewModel(WorkAssignment workAssignment)
        {
            this.workAssignment = workAssignment;
            this.RefreshWorks();
        }


        public void RefreshWorks()
        {
            using (WorkAssignmentRepository workRep = new WorkAssignmentRepository())
            {
                TodoList = workRep.GetAllTasksAssignedToStudentByWork(workAssignment.WorkName, workAssignment.Subject, workAssignment.Theme, workAssignment.Teacher, workAssignment.StudentNum);
            }
        }


        public void Finish()
        {

            using (WorkAssignmentRepository waRep = new WorkAssignmentRepository(IsolationLevel.Serializable))
            {
                foreach (var wa in TodoList)
                {
                    switch (wa.Status)
                    {
                        case "To do":

                            wa.Status = "To check";
                            break;

                        case "To check":
                            if (!Session.HasRole(Session.Role.Teacher))
                                continue;
                            wa.Status = "Checked";
                            break;

                        default:
                            continue;
                    }

                    waRep.Update(wa);
                }

                waRep.Transaction.Commit();
            }
        }

    }
}
