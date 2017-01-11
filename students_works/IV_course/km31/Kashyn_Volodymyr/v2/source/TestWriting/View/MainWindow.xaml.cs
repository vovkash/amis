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

namespace TestWriting.View
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {


        public MainWindow()
        {
            InitializeComponent();



        }

        private void MenuItemProfileEdit_Click(object sender, RoutedEventArgs e)
        {
            AuthorizationWindow wnd = new AuthorizationWindow(ViewModel.AuthorizationViewModel.AuthType.ProfileEdit);
            wnd.Owner = this;
            wnd.ShowDialog();
        }

        private void MenuItemSignOut_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
            AuthorizationWindow wnd = new AuthorizationWindow(ViewModel.AuthorizationViewModel.AuthType.Login);
            wnd.Owner = this;
            wnd.ShowDialog();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
            AuthorizationWindow wnd = new AuthorizationWindow(ViewModel.AuthorizationViewModel.AuthType.Login);
            wnd.Owner = this;
            wnd.ShowDialog();

            
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Title = "Works list | Test writer";
            frameContent.Navigate(new Pages.WorksPage());
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            this.Title = "Tasks list | Test writer";
            frameContent.Navigate(new Pages.ExercisesPage());
        }

        private void MenuItemAssignedToMe_Click(object sender, RoutedEventArgs e)
        {
            this.Title = "Works assigned to me | Test writer";
            frameContent.Navigate(new Pages.AssignedToMePage());
        }

        private void MenuItem_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

    }
}
