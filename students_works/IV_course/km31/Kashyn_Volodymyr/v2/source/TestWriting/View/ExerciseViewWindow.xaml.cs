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
using System.Windows.Shapes;
using TestWriting.ViewModel;
using TestWriting.Model;
using TestWriting.View.Pages;

namespace TestWriting.View
{
    /// <summary>
    /// Логика взаимодействия для TaskMaintain.xaml
    /// </summary>
    public partial class ExerciseViewWindow : Window
    {
        public ExerciseViewWindow(bool _isNew)
        {
            InitializeComponent();

            if(!Session.HasRole(Session.Role.Teacher))
            {
                MessageBox.Show("You dont have permissions for this form!");
                this.Close();
            }

            ExerciseViewModel tmvm = this.DataContext as ExerciseViewModel;
            tmvm.IsNew = _isNew;

            if (_isNew)
                this.Title = "New task";
            else
                this.Title = "View/edit task";

            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ExerciseViewModel tmvm = this.DataContext as ExerciseViewModel;
            if (!tmvm.IsValid)
            {
                MessageBox.Show("Some errors were occured:\n" + tmvm.Error);
                return;
            }


            try
            {
                using (ExerciseRepository exRep = new ExerciseRepository())
                {
                    if (tmvm.IsNew)
                    {
                        exRep.Insert(tmvm.Exercise);
                    }
                    else
                    {
                        exRep.Update(tmvm.Exercise);
                    }
                }

                this.Close();
                if (this.Owner is MainWindow && (this.Owner as MainWindow).frameContent.Content is ExercisesPage)
                {
                    ExercisesPage exPage = (this.Owner as MainWindow).frameContent.Content as ExercisesPage;
                    (exPage.DataContext as ExercisesViewModel).Populate(); 
                    
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show("Some error occured while saving:\n" + exc.Message);
            }

            
            
            
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
