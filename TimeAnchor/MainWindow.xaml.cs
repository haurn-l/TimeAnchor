using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TimeAnchor.Models;
using TimeAnchor.Repositories;

namespace TimeAnchor
{
    public partial class MainWindow : Window
    {
        // Veritabanı işlemleri için nesnemizi tanımlıyoruz
        private DatabaseHelper dbHelper;

        public MainWindow()
        {
            InitializeComponent();

            // Nesneyi ayağa kaldırıyoruz
            dbHelper = new DatabaseHelper();

            // Ekran açılır açılmaz görevleri yükleyen metodu çağırıyoruz
            LoadReminders();
        }

        private void LoadReminders()
        {
            // Veritabanındaki tüm hatırlatıcıları çekiyoruz
            var reminders = dbHelper.GetAllReminders();

            // SADECE TEST İÇİN: Eğer liste boşsa, arayüzün nasıl göründüğünü anlamak için 1 tane sahte veri ekliyoruz.
            if (reminders.Count == 0)
            {
                dbHelper.AddReminder(new Reminder
                {
                    Title = "İlk Görev: Sistemi Test Et",
                    Description = "HarunSinevazyon arka plan mimarisi tıkır tıkır çalışıyor.",
                    EventDate = DateTime.Now.AddHours(2), // Şu andan 2 saat sonrasına kurduk
                    IsTimeSpecific = true,
                    IsCompleted = false,
                    IsSynced = false
                });

                // Sahte veriyi ekledikten sonra listeyi veritabanından tekrar güncel haliyle çekiyoruz
                reminders = dbHelper.GetAllReminders();
            }

            // XAML tarafındaki 'LstReminders' isimli listemizin kaynağını bu çektiğimiz veriler yapıyoruz
            LstReminders.ItemsSource = reminders;
        }

        private void LstReminders_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // Eğer listeden gerçekten bir öğe seçildiyse (tıklandıysa)
            if (LstReminders.SelectedItem is Reminder selectedReminder)
            {
                // Detay penceresini oluştur ve içine seçilen görevi gönder
                ReminderDetailWindow detailWindow = new ReminderDetailWindow(selectedReminder);

                // Owner() kısmı, detay penceresinin ana pencerenin tam ortasında açılmasını sağlar
                detailWindow.Owner = this;

                // Pencereyi ShowDialog ile açıyoruz (bu pencere kapanana kadar alt satıra geçmez)
                detailWindow.ShowDialog();

                // Detay penceresi kapandıktan sonra (belki güncelleme yapılmıştır diye) listeyi yeniliyoruz
                LoadReminders();

                // Aynı öğeye tekrar tıklanabilmesi için seçimi sıfırlıyoruz
                LstReminders.SelectedItem = null;
            }
        }

        // Listedeki hızlı "Tamamlandı" (✔) butonuna basılınca
        private void BtnComplete_Click(object sender, RoutedEventArgs e)
        {
            // Tıklanan butonun hangi görev kartına ait olduğunu buluyoruz
            var button = sender as System.Windows.Controls.Button;
            if (button != null && button.DataContext is Reminder clickedReminder)
            {
                // Görevi tamamlandı olarak işaretle
                clickedReminder.IsCompleted = true;
                dbHelper.UpdateReminder(clickedReminder);

                // Listeyi yenile
                LoadReminders();
            }

            // Satır seçimi tetiklenmesin diye event'i burada durduruyoruz
            e.Handled = true;
        }

        // Listedeki hızlı "Sil" (X) butonuna basılınca
        private void BtnQuickDelete_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as System.Windows.Controls.Button;
            if (button != null && button.DataContext is Reminder clickedReminder)
            {
                MessageBoxResult result = MessageBox.Show($"'{clickedReminder.Title}' görevini silmek istiyor musun?", "Onay", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    dbHelper.DeleteReminder(clickedReminder.Id);

                    // Listeyi yenile
                    LoadReminders();
                }
            }

            // Satır seçimi tetiklenmesin diye event'i burada durduruyoruz
            e.Handled = true;
        }
    }
}