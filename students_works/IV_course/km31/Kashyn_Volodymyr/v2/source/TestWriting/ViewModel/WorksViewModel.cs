using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Data;
using TestWriting.Model;

namespace TestWriting.ViewModel
{
    public class WorksViewModel : PropertyChangedMainViewModel
    {
        List<WorkViewModel> works = new List<WorkViewModel>();
        WorkViewModel selectedWork;

        CollectionViewSource _itemSourceList = new CollectionViewSource();
        ICollectionView itemlist;



        public List<WorkViewModel> Works
        {
            get { return works; }
            set
            {
                works = value;
                NotifyPropertyChanged("Works");
            }
        }

        public WorkViewModel SelectedWork
        {
            get { return selectedWork; }
            set
            {

                selectedWork = value;
                NotifyPropertyChanged("SelectedWork");
                NotifyPropertyChanged("IsWorkSelected");
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

        public bool IsWorkSelected
        {
            get
            {
                return (SelectedWork == null) ? false : true;
            }

        }


        public WorksViewModel()
        {
            _itemSourceList.Source = this.Works;
            Itemlist = _itemSourceList.View;
        }

        public void Populate()
        {
            Works.Clear();
            using (WorkRepository workRep = new WorkRepository())
            {
                List<Work> works = workRep.ListOf();

                foreach (var work in works)
                {
                    Works.Add(new WorkViewModel(work));
                }
            }
            NotifyPropertyChanged("Works");
            NotifyPropertyChanged("Itemlist");
            SetFilter();
        }

        public void DeleteSelected()
        {
            if (!IsWorkSelected)
            {
                return;
            }

            using (WorkRepository workRep = new WorkRepository())
            {
                workRep.Delete(this.SelectedWork.Work);
            }

            this.Populate();
        }


        private void SetFilter()
        {
            switch (FilterField)
            {
                case "Work name":
                    Itemlist.Filter = new Predicate<object>(item => ((WorkViewModel)item).Name.Contains(FilterValue));
                    break;

                case "Subject":
                    Itemlist.Filter = new Predicate<object>(item => ((WorkViewModel)item).Subject.Contains(FilterValue));
                    break;

                case "Theme":
                    Itemlist.Filter = new Predicate<object>(item => ((WorkViewModel)item).Theme.Contains(FilterValue));
                    break;

                case "Description":
                    Itemlist.Filter = new Predicate<object>(item => ((WorkViewModel)item).Description.Contains(FilterValue));
                    break;

            }
        }

    }
}
