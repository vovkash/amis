using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using TestWriting.Model;

namespace TestWriting.ViewModel
{
    public class ExerciseViewModel : INotifyPropertyChanged, IDataErrorInfo
    {
        public event PropertyChangedEventHandler PropertyChanged;

        string[] validationFields = { "TaskName", "Description", "Subject", "Theme", "Task", "Answer", "CreatedBy"};

        Exercise exercise;
        bool isNew;

        public Exercise Exercise
        {
            get { return exercise; }
            set 
            { 
                exercise = value;
                NotifyPropertyChanged("Exercise");
            }
        }

        public string TaskName
        {
            get { return Exercise.Name; }
            set
            {
                Exercise.Name = value;
                NotifyPropertyChanged("TaskName");
            }
        }


        public string Description
        {
            get { return Exercise.Description; }
            set 
            { 
                Exercise.Description = value;
                NotifyPropertyChanged("Description");
            }
        }

        public string Subject
        {
            get { return Exercise.Subject; }
            set
            {
                Exercise.Subject = value;
                NotifyPropertyChanged("Subject");
            }
        }

        public string Theme
        {
            get { return Exercise.Theme; }
            set
            {
                Exercise.Theme = value;
                NotifyPropertyChanged("Theme");
            }
        }

        public string Task
        {
            get { return Exercise.Task; }
            set
            {
                Exercise.Task = value;
                NotifyPropertyChanged("Task");
            }
        }

        public string Answer
        {
            get { return Exercise.Answer; }
            set
            {
                Exercise.Answer = value;
                NotifyPropertyChanged("Answer");
            }
        }

        public string CreatedBy
        {
            get { return Exercise.CreatedBy; }
            set
            {
                Exercise.CreatedBy = value;
                NotifyPropertyChanged("CreatedBy");
            }
        }

        public bool IsNew
        {
            get { return isNew; }
            set { isNew = value;
            NotifyPropertyChanged("IsNew");
            }
        }

        public string this[string _columnName]
        {
            get {

                switch (_columnName)
                {
                    case "TaskName":
                        if (TaskName == null || TaskName.Length < 1)
                            return "Task name field is mandatory!";
                        break;
                    case "Subject":
                        if (Subject == null || Subject.Length < 1)
                            return "Subject field is mandatory!";
                        break;
                    case "Theme":
                        if (Theme == null || Theme.Length < 1)
                            return "Theme field is mandatory!";
                        break;

                    case "Task":
                        if(Task == null || Task.Length < 1)
                            return "Target field is mandatory!";
                        break;

                    case "CreatedBy":
                        if(CreatedBy == null || CreatedBy.Length < 1)
                            return "There must be an author of the exercise!";
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

        public ExerciseViewModel() : 
            this(new Exercise())
        {
            if(Session.Roles.Count > 0)
                this.Exercise.CreatedBy = (Session.User as Teacher).AgreementId;
        }

        public ExerciseViewModel(Exercise exercise)
        {
            this.Exercise = exercise;
        }

        private void NotifyPropertyChanged(string propertyname)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));


        }
    }
}
