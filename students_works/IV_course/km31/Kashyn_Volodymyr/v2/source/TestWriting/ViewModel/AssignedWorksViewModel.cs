using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.ComponentModel;
using TestWriting.Model;

namespace TestWriting.ViewModel
{
    public class AssignedWorksViewModel : PropertyChangedMainViewModel
    {
        List<WorkAssignment> assignedWorks = new List<WorkAssignment>();

        CollectionViewSource _itemSourceList = new CollectionViewSource();
        ICollectionView itemlist;

        WorkAssignment selectedAssignedWork;
        bool isToShowCheckedWorks;
        string filterValue;
        string filterField;

        public List<WorkAssignment> AssignedWorks
        {
            get { return assignedWorks; }
            set 
            { 
                assignedWorks = value;
                NotifyPropertyChanged("AssignedWorks");
            }
        }

        public ICollectionView Itemlist
        {
            get { return itemlist; }
            set
            {
                itemlist = value;
                NotifyPropertyChanged("Itemlist");


            }
        }

        public WorkAssignment SelectedAssignedWork
        {
            get { return selectedAssignedWork; }
            set 
            { 
                selectedAssignedWork = value;
                NotifyPropertyChanged("SelectedAssignedWork");
                NotifyPropertyChanged("IsAssignedWorkSelected");
            }
        }

        public bool IsAssignedWorkSelected
        {
            get
            {
                return (SelectedAssignedWork != null);
            }
        
        }

        public bool IsToShowCheckedWorks
        {
            get { return isToShowCheckedWorks; }
            set 
            { 
                isToShowCheckedWorks = value;
                NotifyPropertyChanged("IsToShowCheckedWorks");
                NotifyPropertyChanged("StartButtonLabel");
                this.Populate();
            }
        }

        public string StartButtonLabel
        {
            get
            { 
                if(IsToShowCheckedWorks)
                {
                    return "View results";
                }

                if (Session.HasRole(Session.Role.Teacher))
                {
                    return "Start checking";
                }

                return "Start execution";
            
            }
        
        }


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



        public AssignedWorksViewModel()
        {
            _itemSourceList.Source = this.AssignedWorks;
            Itemlist = _itemSourceList.View;
        }

        


        public void Populate()
        {
            using (WorkAssignmentRepository wa = new WorkAssignmentRepository())
            {
                if (Session.HasRole(Session.Role.Student))
                {

                    AssignedWorks = wa.GetByStudent((Session.User as Student).StudentNum, IsToShowCheckedWorks);
                }
                else
                {

                    AssignedWorks = wa.GetByTeacher((Session.User as Teacher).AgreementId, IsToShowCheckedWorks);
                }

                _itemSourceList.Source = this.AssignedWorks;
                Itemlist = _itemSourceList.View;

                NotifyPropertyChanged("AssignedWorks");
                NotifyPropertyChanged("Itemlist");

                SetFilter();
            }
        }


        private void SetFilter()
        {
            switch (FilterField)
            {
                case "Work name":
                    Itemlist.Filter = new Predicate<object>(item => ((WorkAssignment)item).WorkName.Contains(FilterValue));
                    break;

                case "Subject":
                    Itemlist.Filter = new Predicate<object>(item => ((WorkAssignment)item).Subject.Contains(FilterValue));
                    break;

                case "Theme":
                    Itemlist.Filter = new Predicate<object>(item => ((WorkAssignment)item).Theme.Contains(FilterValue));
                    break;

                case "Teacher":
                    Itemlist.Filter = new Predicate<object>(item => ((WorkAssignment)item).TeacherName.Contains(FilterValue));
                    break;

                case "Student":
                    Itemlist.Filter = new Predicate<object>(item => ((WorkAssignment)item).StudentName.Contains(FilterValue));
                    break;

            }
        }
    }
}
