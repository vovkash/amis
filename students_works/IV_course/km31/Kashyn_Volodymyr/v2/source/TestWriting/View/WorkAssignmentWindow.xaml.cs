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

namespace TestWriting.View
{
    /// <summary>
    /// Логика взаимодействия для WorkAssignmentWindow.xaml
    /// </summary>
    public partial class WorkAssignmentWindow : Window
    {
        public WorkAssignmentWindow()
        {
            InitializeComponent();
        }

        private void ButtonAssign_Click(object sender, RoutedEventArgs e)
        {
            WorkAssignmentViewModel wavm = this.DataContext as WorkAssignmentViewModel;
            if (!wavm.IsValid)
            {
                MessageBox.Show("Some errors were occured:\n" + wavm.Error);
                return;
            }

            try
            {
                wavm.AssignToProcess();
                this.Close();
            }
            catch (Exception exc)
            {
                if (exc.Message.Contains("ORA-00001"))
                {
                    MessageBox.Show("You can't assign work to the student because it has benn already assigned to him!");
                }
                else
                {
                    MessageBox.Show(exc.Message);
                    this.Close();
                }
            }
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
