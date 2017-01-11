using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Data;
using TestWriting.Model;

namespace TestWriting.ViewModel
{
    public class WorkAssignmentViewModel : PropertyChangedMainViewModel, INotifyPropertyChanged, IDataErrorInfo
    {

        string[] validationFields = new string[] { "WorkName",  "AssignTo" };

        Work work;

        bool isGroup;
        object assignTo;

        public Work Work
        {
            get { return work; }
            set { work = value; }
        }


        public string WorkName
        {
            get { return Work.Name; }
            set
            {
                Work.Name = value;
                NotifyPropertyChanged("WorkName");
            }
        }



        public bool IsGroup
        {
            get { return isGroup; }
            set 
            { 
                isGroup = value;
                NotifyPropertyChanged("IsGroup");
                NotifyPropertyChanged("AssignList");
            }
        }

        public object AssignTo
        {
            get { return assignTo; }
            set 
            { 
                assignTo = value;
                NotifyPropertyChanged("AssignTo");
            }
        }

        public List<object> AssignList
        {
            get
            {
                List<object> objs = new List<object>();
                if (IsGroup)
                {
                    objs.Add("KM-31");
                    objs.Add("KM-32");
                    objs.Add("KM-33");

                }
                else
                {
                    using (StudentRepository stRep = new StudentRepository())
                    {
                        List<Student> students = stRep.ListOf();

                        foreach (var student in students)
                        {
                            objs.Add(student);
                        }
                    }
                }

                return objs;

            }
        
        }


        public string this[string _columnName]
        {
            get
            {

                switch (_columnName)
                {
                    case "WorkName":
                        if (WorkName == null || WorkName.Length < 1)
                            return "Work name field is mandatory!";
                        break;


                    case "AssignTo":
                        if (AssignTo == null || ((AssignTo is string) && ((AssignTo as string).Length < 1)))
                            return "You should select student or group!";
                        break;

                }

                return null;

            }
        }

        public string Error
        {
            get
            {
                string error = "";
                foreach (string property in validationFields)
                {
                    string s = this[property];
                    if (s != null)
                        error += s + "\n";
                }

                return error;
            }
        }

        public bool IsValid
        {
            get
            {
                foreach (string property in validationFields)
                {

                    if (this[property] != null)
                        return false;
                }

                return true;
            }
        }

        public WorkAssignmentViewModel()
        {
            
        }

        public WorkAssignmentViewModel(Work work)
        {
            Work = work;
            AssignTo = "";
        }

        public void AssignToProcess()
        {
            using (WorkRepository workRep = new WorkRepository(IsolationLevel.Serializable))
            { 
                if(IsGroup)
                {
                    workRep.AssignToGroup(Work, (AssignTo as string));
                }
                else
                {
                    workRep.AssignToStudent(Work, (AssignTo as Student).StudentNum);
                }

                workRep.Transaction.Commit();

            }
        }
    }
}
