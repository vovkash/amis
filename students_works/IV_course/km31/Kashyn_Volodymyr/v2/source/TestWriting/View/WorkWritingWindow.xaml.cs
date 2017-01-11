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
using TestWriting.Model;
using TestWriting.ViewModel;
using System.Data;

namespace TestWriting.View
{
    /// <summary>
    /// Логика взаимодействия для WorkWritingWindow.xaml
    /// </summary>
    public partial class WorkWritingWindow : Window
    {
        bool saved;

        public WorkWritingWindow()
        {
            InitializeComponent();
            saved = false;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WorkWritingViewModel wwvm = this.DataContext as WorkWritingViewModel;

            if (Session.HasRole(Session.Role.Teacher))
            {
                btn_end.Content = (object)"Set mark";
                tb_answer.IsReadOnly = true;

            }
            else
            {
                exp_teacheransw.Visibility = Visibility.Collapsed;
            }

            if (Session.HasRole(Session.Role.Student) && wwvm.WorkAssignment.Status != "Checked")
            {
                lbl_point.Visibility = Visibility.Collapsed;
                tb_point.Visibility = Visibility.Collapsed;     
            }

            if (wwvm.WorkAssignment.Status == "Checked")
            {
                btn_end.Content = (object)"Close";
                saved = true;
                tb_answer.IsReadOnly = true;
                tb_point.IsReadOnly = true;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            WorkWritingViewModel wwvm = this.DataContext as WorkWritingViewModel;

            if (wwvm.WorkAssignment.Status == "Checked")
            {
                this.Close();
                return;
            }

            try
            {
                if (!IsValid(this))
                {
                    MessageBox.Show("Some errors are in input data. Please check and try again.", "Invalid input data");
                    return;
                }

                (this.DataContext as WorkWritingViewModel).SaveWork();
                saved = true;
                this.Close();
            }
            catch(Exception exc)
            {
                MessageBox.Show("Some error occured while saving work. Please try again and if it happens again ask your teacher or administrator to help you.\nError info:\n" + exc.Message);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!saved && MessageBox.Show("Are that you want close this window witout saving?", "Confirm exit without saving", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
            
        }



        private bool IsValid(DependencyObject parent)
        {
            if (Validation.GetHasError(parent))
                return false;

            // Validate all the bindings on the children
            for (int i = 0; i != VisualTreeHelper.GetChildrenCount(parent); ++i)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if (!IsValid(child)) { return false; }
            }

            return true;
        }
    }
}
