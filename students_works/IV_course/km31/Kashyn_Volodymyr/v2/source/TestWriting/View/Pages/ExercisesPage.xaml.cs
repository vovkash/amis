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
    /// Логика взаимодействия для TaskPage.xaml
    /// </summary>
    public partial class ExercisesPage : Page
    {
        public ExercisesPage()
        {
            InitializeComponent();
        }

        private void ButtonNew_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = Window.GetWindow(this) as MainWindow;

            if(Session.HasRole(Session.Role.Teacher))
            {
                ExerciseViewWindow wnd = new ExerciseViewWindow(true);
                wnd.Owner = mainWindow;
                wnd.ShowDialog();
            }
            else
            {
                MessageBox.Show("You dont have permissions for this form!");
            }



        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            ExercisesViewModel evm = this.DataContext as ExercisesViewModel;
            if (!evm.IsExerciseSelected)
            {
                MessageBox.Show("You need to select exercise to edit it!", "Please select record");
                return;
            }

            ExerciseViewWindow wnd = new ExerciseViewWindow(false);
            wnd.DataContext = evm.SelectedExercise;
            wnd.Owner = Window.GetWindow(this);
            wnd.ShowDialog();
        }

        private void DataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            (this.DataContext as ExercisesViewModel).Populate();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Do you really want to delete selected exercise?", "Deletion confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
            {
                return;
            }

            try
            {
                (this.DataContext as ExercisesViewModel).DeleteSelected();
            }
            catch (Exception exc)
            {
                if (exc.Message.Contains("ORA-02292"))
                {
                    MessageBox.Show("You can't delete task beacuse it is used in the work!");
                }
                else
                 MessageBox.Show("Some errors were occured while deleteing exercise. Please try again later or contact system administrator.\n Error log: " + exc.Message);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            (this.DataContext as ExercisesViewModel).Populate();
        }
    }
}
