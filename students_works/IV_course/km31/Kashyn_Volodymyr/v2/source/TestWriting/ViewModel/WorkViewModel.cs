using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Data;
using TestWriting.Model;

namespace TestWriting.ViewModel
{
    public class WorkViewModel : INotifyPropertyChanged, IDataErrorInfo
    {
        public event PropertyChangedEventHandler PropertyChanged;

        Work work;

        public Work Work
        {
          get { return work; }
          set 
          { 
              work = value; 
              NotifyPropertyChanged("Work");
          }
        }


        bool isNew;

        WorkExercisesViewModel workExercisesViewModel = new WorkExercisesViewModel();

        public WorkExercisesViewModel WorkExercisesViewModel
        {
            get { return workExercisesViewModel; }
            set { workExercisesViewModel = value; }
        }

        public string Name
        {
            get { return Work.Name; }
            set 
            { 
                Work.Name = value;
                NotifyPropertyChanged("Name");
            }
        }

        public string Description
        {
            get { return Work.Description; }
            set 
            { 
                Work.Description = value;
                NotifyPropertyChanged("Description");
            }
        }

        public string Theme
        {
            get { return Work.Theme; }
            set 
            { 
                Work.Theme = value;
                NotifyPropertyChanged("Theme");
            }
        }

        public string Subject
        {
            get { return Work.Subject; }
            set 
            { 
                Work.Subject = value;
                NotifyPropertyChanged("Subject");
            }
        }

        public string Teacher
        {
            get { return Work.Teacher; }
            set
            {
                Work.Teacher = value;
                NotifyPropertyChanged("Teacher");
            }

        }

        public bool IsNew
        {
            get { return isNew; }
            set 
            { 
                    isNew = value;
                    NotifyPropertyChanged("IsNew");
                    NotifyPropertyChanged("IsIdFieldEditable");
            }
        }

        public bool IsIdFieldEditable
        {
            get 
            {
                if (WorkExercisesViewModel.WorkExercises.Count > 0 || !IsNew)
                {
                    return false;
                }

                return true;
            }
        }

        public string Error
        {
            get
            {
                string error = "";
                foreach (string property in new string[] { "Name", "Theme", "Subject", "WorkExercises" })
                {
                    string s = this[property];
                    if (s != null) 
                        error += s + "\n";
                }

                if (!WorkExercisesViewModel.IsValid)
                {
                   error +=  WorkExercisesViewModel.Error;
                }

                return error;
            }
        }

        public bool IsValid
        {
            get
            {
                foreach (string property in new string[] { "Name", "Theme", "Subject", "WorkExercises" })
                {

                    if (this[property] != null)
                        return false;
                }


                if (!WorkExercisesViewModel.IsValid)
                {
                    return false;
                }

                return true;
            }
        }

        public string this[string _columnName]
        {
            get
            {
                if (_columnName == "Name")
                {
                    if (Name.Length < 1)
                    {
                        return "Work name field is mandatory!";
                    }
                }

                if (_columnName == "Theme")
                {
                    if (Theme.Length < 1)
                    {
                        return "Theme field is mandatory!";
                    }
                }

                if (_columnName == "Subject")
                {
                    if (Subject.Length < 1)
                    {
                        return "Subject field is mandatory!";
                    }
                }

                return null;
            }
        }



        public WorkViewModel()
        {
            Work = new Work();

            this.Name = "";
            this.Theme = "";
            this.Subject = "";
            this.Description = "";
            this.Teacher = (Session.User as Teacher).AgreementId;
        }

        public WorkViewModel(Work work)
        {
            Work = work;
            if (WorkExercisesViewModel != null)
            {
                WorkExercisesViewModel.workViewModel = this;
            }
            this.Teacher = (Session.User as Teacher).AgreementId;
        }

        public void NewWorkExercise()
        {
            WorkExercisesViewModel.AddNew(Name, Subject, Theme, Teacher);
        }

        public void DeleteWorkExercise()
        {
            workExercisesViewModel.DeleteSelected();
        }

        public void SaveNew()
        {
            List<WorkExercise> exercisesToInsert = new List<WorkExercise>();
            foreach (var wevm in WorkExercisesViewModel.WorkExercises)
            {
                exercisesToInsert.Add(wevm.WorkExercise);
            }

            using(WorkRepository workRep = new WorkRepository(IsolationLevel.Serializable))
            {
                workRep.InsertWithWorkExercises(Work, exercisesToInsert);
                workRep.Transaction.Commit();
            }
        
        }

        public void SaveEdit()
        {
            List<WorkExercise> exercisesToInsert = new List<WorkExercise>();
            foreach (var wevm in WorkExercisesViewModel.WorkExercises)
            {
                exercisesToInsert.Add(wevm.WorkExercise);
            }

            using (WorkRepository workRep = new WorkRepository(IsolationLevel.Serializable))
            {
                workRep.UpdateWithWorkExercises(Work, exercisesToInsert);
                workRep.Transaction.Commit();
            }

        }

        public void NotifyPropertyChanged(string propertyname)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));


        }
    }
}
