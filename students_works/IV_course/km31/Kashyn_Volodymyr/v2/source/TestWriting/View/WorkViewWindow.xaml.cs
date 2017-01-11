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
using TestWriting.View.Pages;

namespace TestWriting.View
{
    /// <summary>
    /// Логика взаимодействия для WorkMaintain.xaml
    /// </summary>
    public partial class WorkViewWindow : Window
    {


        public WorkViewWindow(bool _isNewRecord)
        {
            

            InitializeComponent();

            WorkExercisesViewModel wevm = datagrid.DataContext as WorkExercisesViewModel;

            WorkViewModel wmvm = this.DataContext as WorkViewModel;
            wevm.workViewModel = wmvm;

            wmvm.IsNew = _isNewRecord;

            if (_isNewRecord)
                this.Title = "New work";
            else
            {
                this.Title = "Edit/view work";
                
            }

           
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            WorkViewModel wmvm = this.DataContext as WorkViewModel;

            if (!wmvm.IsValid)
            {
                MessageBox.Show("Some errors were occured:\n" + wmvm.Error);
                return;
            }

            if (!IsValid(this))
            {
                MessageBox.Show("Please enter valid data to continue.");
                return;

            }

            try
            {
                if (wmvm.IsNew)
                    wmvm.SaveNew();
                else
                    wmvm.SaveEdit();

                this.Close();
                if (this.Owner is MainWindow && (this.Owner as MainWindow).frameContent.Content is WorksPage)
                {
                    WorksPage worksPage = (this.Owner as MainWindow).frameContent.Content as WorksPage;
                    (worksPage.DataContext as WorksViewModel).Populate();

                }
            }
            catch (Exception exc)
            {
                MessageBox.Show("Some errors were occured:\n" + exc.Message);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            (this.DataContext as WorkViewModel).NewWorkExercise();
        }



        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            (this.DataContext as WorkViewModel).DeleteWorkExercise();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WorkViewModel wmvm = this.DataContext as WorkViewModel;
            
            wmvm.WorkExercisesViewModel.Populate();      
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
       
    }
}
