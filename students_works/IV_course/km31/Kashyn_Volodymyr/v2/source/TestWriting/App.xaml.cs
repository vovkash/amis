using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using TestWriting.View;

namespace TestWriting
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            try
            {
                using (Database db = new Database())
                {
                    db.TestConnection();
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show("Cant connect to database!/n" + exc.Message);
            }
        }
    }
}
