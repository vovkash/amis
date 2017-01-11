using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TestWriting.ViewModel;

namespace TestWriting.View.Pages
{
    /// <summary>
    /// Логика взаимодействия для ProfilePage.xaml
    /// </summary>
    public partial class WorksPage : Page
    {
        public WorksPage()
        {
            InitializeComponent();
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            WorkViewWindow workMaintain = new WorkViewWindow(true);
            workMaintain.Owner = Window.GetWindow(this);
            workMaintain.ShowDialog();
        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            WorksViewModel evm = this.DataContext as WorksViewModel;
            if (!evm.IsWorkSelected)
            {
                MessageBox.Show("You need to select exercise to edit it!", "Please select record");
                return;
            }

            WorkViewWindow workMaintain = new WorkViewWindow(false);
            workMaintain.DataContext = evm.SelectedWork;

            workMaintain.Owner = Window.GetWindow(this);
            workMaintain.ShowDialog();
        }

        private void ButtonAssignTo_Click(object sender, RoutedEventArgs e)
        {
            WorksViewModel evm = this.DataContext as WorksViewModel;
            WorkAssignmentWindow wnd = new WorkAssignmentWindow();
            wnd.DataContext = new WorkAssignmentViewModel(evm.SelectedWork.Work);
            wnd.Owner = Window.GetWindow(this);
            wnd.ShowDialog();
        }

        private void DataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            (this.DataContext as WorksViewModel).Populate();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Do you really want to delete selected exercise?", "Deletion confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
            {
                return;
            }

            try
            {
                (this.DataContext as WorksViewModel).DeleteSelected();
            }
            catch (Exception exc)
            {
                if (exc.Message.Contains("ORA-02292"))
                {
                    MessageBox.Show("You can't delete work beacuse it has been already assigned to students!");
                }
                else
                    MessageBox.Show("Some errors were occured while deleteing exercise. Please try again later or contact system administrator.\n Error log: " + exc.Message);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            (this.DataContext as WorksViewModel).Populate();
        }


    }
}
