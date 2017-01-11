using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestWriting.Model;
using System.ComponentModel;

namespace TestWriting.ViewModel
{
    public class WorkExerciseViewModel : PropertyChangedMainViewModel, IDataErrorInfo
    {
        public WorkExercisesViewModel WorkExercisesViewModel;

        WorkExercise workExercise;

        string[] validationFields = { "ExerciseName", "Points" };

        public WorkExercise WorkExercise
        {
            get { return workExercise; }
            set 
            { 
                workExercise = value;
                NotifyPropertyChanged("WorkExercise");
            }
        }

        List<Exercise> availableTasks = new List<Exercise>();



        public List<Exercise> AvailableTasks
        {
            get 
            {     
                using (ExerciseRepository exRep = new ExerciseRepository())
                {
                    availableTasks = exRep.FindBySubjectAndThemeAndAuthor(this.WorkExercise.Subject, this.WorkExercise.Theme, (Session.User as Teacher).AgreementId);
                }
                return availableTasks; 
            }
        }

        

        public string ExerciseName
        {
            get { return WorkExercise.ExerciseName; }
            set
            {
                if (value != WorkExercise.ExerciseName)
                {
                    WorkExercise.ExerciseName = value;
                    NotifyPropertyChanged("ExerciseName");
                    WorkExercisesViewModel.NotifyPropertyChanged("WorkExercises");
                }
            }
        }


        public int Points
        {
            get { return WorkExercise.Points; }
            set
            {
                if (value != WorkExercise.Points)
                {
                    WorkExercise.Points = value;
                    NotifyPropertyChanged("Points");
                    WorkExercisesViewModel.NotifyPropertyChanged("WorkExercises");
                }
            }
        }

        public string this[string _columnName]
        {
            get
            {

                switch (_columnName)
                {
                    case "ExerciseName":
                        if (ExerciseName== null || ExerciseName.Length < 1)
                            return "Exercise field is mandatory!";

                        try
                        {
                            if (WorkExercisesViewModel != null && WorkExercisesViewModel.WorkExercises.Count(x => x.ExerciseName == this.ExerciseName) > 1)
                                return "You can't add two same exercises to this work";
                        }
                        catch (Exception exc)
                        {
                            System.Windows.MessageBox.Show(exc.Message);
                        }

                        break;

                    case "Points":
                        if (Points < 1)
                            return "Points should be more tha zero!";
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


        public WorkExerciseViewModel() :
            this(new WorkExercise())
        {
            ExerciseName = "";
        }

        public WorkExerciseViewModel(WorkExercise workEx)
        {
            WorkExercise = workEx;

            
        }

        public WorkExerciseViewModel(Work work)
        {
            WorkExercise = new WorkExercise();
            WorkExercise.WorkName = work.Name;
            WorkExercise.Subject = work.Subject;
            WorkExercise.Theme = work.Theme;
            WorkExercise.Teacher = work.Teacher;
        
        }



    }
}
