using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestWriting.Model;
using System.ComponentModel;
using System.Windows.Data;

namespace TestWriting.ViewModel
{
    public class ExercisesViewModel : PropertyChangedMainViewModel
    {
        List<ExerciseViewModel> exercises = new List<ExerciseViewModel>();

        ExerciseViewModel selectedExercise;

        CollectionViewSource _itemSourceList = new CollectionViewSource();
        ICollectionView itemlist;

        public ICollectionView Itemlist
        {
            get { return itemlist; }
            set 
            { 
                itemlist = value;
                NotifyPropertyChanged("Itemlist");

                
            }
        }

        string filterValue;
        string filterField;


        public string FilterValue
        {
            get { return filterValue; }
            set 
            { 
                filterValue = value;
                NotifyPropertyChanged("FilterValue");

                SetFilter();
                
            }
        }

        public string FilterField
        {
            get { return filterField; }
            set 
            { 
                filterField = value;

                NotifyPropertyChanged("FilterField");
                SetFilter();
            }
        }


        public List<ExerciseViewModel> Exercises
        {
            get { return exercises; }
            set 
            { 
                exercises = value;
                NotifyPropertyChanged("Exercises");
            }
        }

        public ExerciseViewModel SelectedExercise
        {
            get { return selectedExercise; }
            set 
            {
                selectedExercise = value;
                NotifyPropertyChanged("IsExerciseSelected");
            
            }
        }

        public bool IsExerciseSelected
        {
            get 
            { 
                return (SelectedExercise == null) ? false : true;
            }
        
        }


        public ExercisesViewModel()
        {
            _itemSourceList.Source = this.Exercises;
            Itemlist = _itemSourceList.View;
        }

        public void Populate()
        {
            Exercises.Clear();
            using (ExerciseRepository exRep = new ExerciseRepository())
            {       
                List<Exercise> exercises = exRep.ListOf();

                foreach (var exercise in exercises)
                {
                    Exercises.Add(new ExerciseViewModel(exercise));   
                }
            }
            NotifyPropertyChanged("Exercises");
            NotifyPropertyChanged("Itemlist");
            SetFilter();
        }

        public void DeleteSelected()
        {
            if (!IsExerciseSelected)
            {
                return;
            }

            using (ExerciseRepository exRep = new ExerciseRepository())
            {
                exRep.Delete(this.SelectedExercise.Exercise);
            }

            this.Populate();
        }


        private void SetFilter()
        {
            switch (FilterField)
            {
                case "Task name":
                    Itemlist.Filter = new Predicate<object>(item => ((ExerciseViewModel)item).TaskName.Contains(FilterValue));
                    break;

                case "Subject":
                    Itemlist.Filter = new Predicate<object>(item => ((ExerciseViewModel)item).Subject.Contains(FilterValue));
                    break;

                case "Theme":
                    Itemlist.Filter = new Predicate<object>(item => ((ExerciseViewModel)item).Theme.Contains(FilterValue));
                    break;

                case "Description":
                    Itemlist.Filter = new Predicate<object>(item => ((ExerciseViewModel)item).Description.Contains(FilterValue));
                    break;

            }
        }
        
        


    }
}
