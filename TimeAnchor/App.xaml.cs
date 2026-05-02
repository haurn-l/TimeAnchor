using System.Configuration;
using System.Data;
using System.Windows;
using TimeAnchor.Repositories;

namespace TimeAnchor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            DatabaseHelper dbHelper = new DatabaseHelper();
            dbHelper.InitializeDatabase();
        }
    }

}
