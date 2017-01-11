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

namespace TestWriting.View
{
    /// <summary>
    /// Логика взаимодействия для TasksToDoWindow.xaml
    /// </summary>
    public partial class TasksToDoWindow : Window
    {
        public TasksToDoWindow()
        {
            InitializeComponent();
        }

        private void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString(); 
        }

        private void ButtonOpen_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            WorkWritingWindow wnd = new WorkWritingWindow();
            wnd.DataContext = new WorkWritingViewModel(button.Tag as WorkAssignment);
            wnd.Owner = this;
            wnd.ShowDialog();

            (this.DataContext as TasksToDoViewModel).RefreshWorks();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            TasksToDoViewModel ttdvm = this.DataContext as TasksToDoViewModel;

            if (ttdvm.Status == "Checked")
            {
                return;
            }

            string item = Session.HasRole(Session.Role.Student) ? "answer" : "mark";

            e.Cancel = true;

            if (MessageBox.Show("Are you sure that You want to send " + item + "?\nYou won't have any chance to change " + item + "!", "Confirm " + item + " sending", MessageBoxButton.YesNo, MessageBoxImage.Warning) == System.Windows.MessageBoxResult.No)
            {
                return;
            }

            try
            {
                (this.DataContext as TasksToDoViewModel).Finish();
                e.Cancel = false;
            }
            catch (Exception exc)
            {
                MessageBox.Show("Some errors occured while saving your work. Please try again. If it doesn't help, ask teacher or administrator to help you.\nError:\n" + exc.Message, "Error while saving work", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TasksToDoViewModel ttdvm = this.DataContext as TasksToDoViewModel;

            if (Session.HasRole(Session.Role.Teacher))
            {
                
                lbl_whos.Content = "Student";
                btn_send.Content = "End checking";
                this.Title = "Work checking";
            }

            if (ttdvm.Status == "Checked")
            {
                this.Title = "Work results";
                btn_send.Content = "Close";
                group_send.Header = "Close the form";
            }
            
        }
    }
}
