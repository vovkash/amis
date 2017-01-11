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
    /// Логика взаимодействия для AssignedToMePage.xaml
    /// </summary>
    public partial class AssignedToMePage : Page
    {
        public AssignedToMePage()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            AssignedWorksViewModel awvm = this.DataContext as AssignedWorksViewModel;

            TasksToDoWindow wnd = new TasksToDoWindow();
            wnd.DataContext = new TasksToDoViewModel(awvm.SelectedAssignedWork);
            wnd.Owner = Window.GetWindow(this);
            wnd.ShowDialog();

            awvm.Populate();

        }

        private void DataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            DataGrid datagrid = sender as DataGrid;

            if (Session.HasRole(Session.Role.Student))
            {
                datagrid.Columns[4].Visibility = Visibility.Visible;
                datagrid.Columns[5].Visibility = Visibility.Collapsed;
            }
            else
            {
                datagrid.Columns[4].Visibility = Visibility.Collapsed;
                datagrid.Columns[5].Visibility = Visibility.Visible;
            }

            (this.DataContext as AssignedWorksViewModel).Populate();
        }

        private void ComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            ComboBox combobox = sender as ComboBox;

            if (Session.HasRole(Session.Role.Teacher))
            {
                combobox.Items.RemoveAt(3);
            }

            if (Session.HasRole(Session.Role.Student))
            {
                combobox.Items.RemoveAt(4);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            (this.DataContext as AssignedWorksViewModel).Populate();
        }


    }
}
