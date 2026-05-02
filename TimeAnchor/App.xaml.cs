using System.Configuration;
using System.Data;
using System.Windows;
using TimeAnchor.Repositories; // DatabaseHelper sınıfımıza ulaşmak için paketi import ediyoruz

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

            // Veritabanı yardımcımızdan bir nesne türetip başlatma işlemini çağırıyoruz
            DatabaseHelper dbHelper = new DatabaseHelper();
            dbHelper.InitializeDatabase();
        }
    }

}
