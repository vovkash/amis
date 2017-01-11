using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestWriting.Model;
using System.ComponentModel;

namespace TestWriting.ViewModel
{
    public class WorkExercisesViewModel : PropertyChangedMainViewModel, IDataErrorInfo
    {
        public WorkViewModel  workViewModel;


        WorkExerciseViewModel selectedWorkExercise;

        public List<WorkExerciseViewModel> workExercises = new List<WorkExerciseViewModel>();


        public WorkExerciseViewModel SelectedWorkExercise
        {
            get { return selectedWorkExercise; }
            set 
            {
                if (value != selectedWorkExercise)
                {
                    selectedWorkExercise = value;
                    NotifyPropertyChanged("SelectedWorkExercise");
                }
            }
        }

        public List<WorkExerciseViewModel> WorkExercises
        {
            get { return workExercises; }
            set 
            {
                workExercises = value;
                NotifyPropertyChanged("WorkExercises");
            }
        }


        public string Error
        {
            get
            {
                string error = "";
                foreach (string property in new string[] { "WorkExercises"})
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
                foreach (string property in new string[] { "WorkExercises" })
                {

                    if (this[property] != null)
                        return false;
                }

                return true;
            }
        }

        public string this[string _columnName]
        {
            get
            {
                if (_columnName == "WorkExercises")
                {
                    if (WorkExercises.Count < 1)
                    {
                        return "You should add any exercise to work!";
                    }

                    foreach (var we in WorkExercises)
                    {
                        if (!we.IsValid)
                        {
                            return we.Error;
                        }

                        if (WorkExercises.Count(x => x.ExerciseName == we.ExerciseName) > 1)
                        {
                            return "There're some duplicates of exercises. Please delete them and trye again.";
                        }

                        
                    }
                }

                return null;

            }
        }


        public void Populate()
        {
            using (WorkExerciseRepository weRep = new WorkExerciseRepository())
            {

                WorkExercises.Clear();

                var wes = weRep.FindByWorkAndSubjectAndThemeAndAuthor(workViewModel.Name, workViewModel.Subject, workViewModel.Theme, (Session.User as Teacher).AgreementId);
                foreach (var we in wes)
                {
                    WorkExercises.Add(new WorkExerciseViewModel(we));
                }
            }

            NotifyPropertyChanged("WorkExercises");
        
        }

        public void AddNew(string workName, string subject, string theme, string teacher, string taskName = "")
        {
            WorkExercise we = new WorkExercise();
            we.WorkName = workName;
            we.Subject = subject;
            we.Theme = theme;
            we.Teacher = teacher;
            we.ExerciseName = taskName;

            WorkExerciseViewModel wevm = new WorkExerciseViewModel(we);
            wevm.WorkExercisesViewModel = this;

            WorkExercises.Add(wevm);

            workViewModel.NotifyPropertyChanged("IsIdFieldEditable");
            NotifyPropertyChanged("WorkExercises");
        }

        public void DeleteSelected()
        {
            if (SelectedWorkExercise == null)
            {
                return;
            }

            if (System.Windows.MessageBox.Show("Do you really want to delete selected task from work?", "Confirm deletion", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Warning) == System.Windows.MessageBoxResult.No)
            {
                return;
            }


            WorkExercises.Remove(SelectedWorkExercise);

            workViewModel.NotifyPropertyChanged("IsIdFieldEditable");
            NotifyPropertyChanged("WorkExercises");
        }
    }
}
