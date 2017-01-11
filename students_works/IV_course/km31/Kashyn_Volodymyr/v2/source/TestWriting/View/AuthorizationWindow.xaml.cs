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

namespace TestWriting.View
{
    /// <summary>
    /// Логика взаимодействия для AuthorizationWindow.xaml
    /// </summary>
    public partial class AuthorizationWindow : Window
    {
        private AuthorizationViewModel.AuthType authType;
        private bool authorized = false;

        public AuthorizationWindow(AuthorizationViewModel.AuthType authType = AuthorizationViewModel.AuthType.Login, Student student = null)
        {
            authorized = false;
            InitializeComponent();

            this.authType = authType;
            AuthorizationViewModel avm = this.DataContext as AuthorizationViewModel;
            avm.AccessType = authType;

            if (authType == AuthorizationViewModel.AuthType.Login)
            {
                this.Title = "Login in the system";
                Session.Roles.Clear();
            }

            if (authType == AuthorizationViewModel.AuthType.Register)
            {
                this.Title = "Register in the system";
                Session.Roles.Clear();
            }

            if (authType == AuthorizationViewModel.AuthType.ProfileEdit)
            {
                avm.User = Session.User;
                this.Title = "Profile view";
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Window wnd = new MainWindow();
            AuthorizationViewModel avm = this.DataContext as AuthorizationViewModel;
            switch (authType)
            {
                case AuthorizationViewModel.AuthType.Login:
                    Environment.Exit(0);
                    break;

                case AuthorizationViewModel.AuthType.Register:             
                        this.Close(); 
                    break;

                default:
                    this.Close();
                    break;

            }
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            AuthorizationViewModel avm = this.DataContext as AuthorizationViewModel;
            if (!IsValid(this) || !avm.IsValid)
            {
                MessageBox.Show("Some errors were occured:\n"+avm.Error);
                return;
            }

            try
            {

                switch (authType)
                {
                    case AuthorizationViewModel.AuthType.Login:

                        

                        avm.Login();
                        authorized = true;

                        MainWindow mwnd = this.Owner as MainWindow;
                        if (Session.HasRole(Session.Role.Teacher))
                        {
                            mwnd.menu_assign.Header = "WORK CHECKING";
                            mwnd.menu_works.Visibility = Visibility.Visible;
                            mwnd.menu_taks.Visibility = Visibility.Visible;
                            mwnd.frameContent.Navigate(new Pages.WorksPage());
                        }
                        else
                        {
                            mwnd.menu_assign.Header = "ASSIGNED TO ME";
                            mwnd.menu_works.Visibility = Visibility.Collapsed;
                            mwnd.menu_taks.Visibility = Visibility.Collapsed;
                            mwnd.frameContent.Navigate(new Pages.AssignedToMePage());
                        }
                        
                        this.Owner.Visibility = Visibility.Visible;
                        this.Close();

                        break;

                    case AuthorizationViewModel.AuthType.Register:
                        if (avm.Register())
                        {
                            this.Close();
                        }
                        break;

                    default:
                        if (avm.UpdateProfile())
                            this.Close();
                        break;

                }
            }
            catch (Exception exc)
            {
                MessageBox.Show("Some errors were occured:\n" + exc.Message);
            
            }
            
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
            AuthorizationWindow wnd = new AuthorizationWindow(AuthorizationViewModel.AuthType.Register);
            wnd.Owner = this;
            wnd.ShowDialog();
            this.Visibility = Visibility.Visible;
            
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (this.authType == AuthorizationViewModel.AuthType.Login && !authorized)
            {
                Environment.Exit(0);
            }
        }


        public bool IsValid(DependencyObject parent)
        {
            // Validate all the bindings on the parent
            bool valid = true;
            LocalValueEnumerator localValues = parent.GetLocalValueEnumerator();
            while (localValues.MoveNext())
            {
                LocalValueEntry entry = localValues.Current;
                if (BindingOperations.IsDataBound(parent, entry.Property))
                {
                    Binding binding = BindingOperations.GetBinding(parent, entry.Property);
                    foreach (ValidationRule rule in binding.ValidationRules)
                    {
                        ValidationResult result = rule.Validate(parent.GetValue(entry.Property), null);
                        if (!result.IsValid)
                        {
                            BindingExpression expression = BindingOperations.GetBindingExpression(parent, entry.Property);
                            Validation.MarkInvalid(expression, new ValidationError(rule, expression, result.ErrorContent, null));
                            valid = false;
                        }
                    }
                }
            }

            // Validate all the bindings on the children
            for (int i = 0; i != VisualTreeHelper.GetChildrenCount(parent); ++i)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if (!IsValid(child))
                {
                    valid = false;
                }
            }

            return valid;
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox passbox = sender as PasswordBox;
            if (this.DataContext != null)
            {
                AuthorizationViewModel avm = this.DataContext as AuthorizationViewModel;
                avm.Password = passbox.SecurePassword;
                if (passbox.SecurePassword.Length < 1 && this.authType != AuthorizationViewModel.AuthType.ProfileEdit)
                {
                    passbox.BorderBrush = Brushes.Red;
                }
                else
                {
                    passbox.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFE3E9EF"));
                }
            }
        
        }

        private void PasswordBox_PasswordChanged_1(object sender, RoutedEventArgs e)
        {
            PasswordBox passbox = sender as PasswordBox;
            if (this.DataContext != null)
            {
                AuthorizationViewModel avm = this.DataContext as AuthorizationViewModel;
                avm.Passwordreenter = passbox.SecurePassword;
                if (passbox.SecurePassword.Length < 1 && this.authType != AuthorizationViewModel.AuthType.ProfileEdit)
                {
                    passbox.BorderBrush = Brushes.Red;
                }
                else
                { 
                    passbox.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFE3E9EF"));  
                }
            }
        }

        private void PasswordBox_Loaded(object sender, RoutedEventArgs e)
        {
            PasswordBox passbox = sender as PasswordBox;
            if (this.authType == AuthorizationViewModel.AuthType.ProfileEdit)
                return;

            passbox.BorderBrush = Brushes.Red;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Session.HasRole(Session.Role.Teacher))
            {
                labelUniqueNumber.Content = "Agreement";
            }
            else
            {
                labelUniqueNumber.Content = "Student number";
            }
        }

    }
}
