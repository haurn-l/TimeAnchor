using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using TimeAnchor.Models;
using TimeAnchor.Repositories;

namespace TimeAnchor
{
    public partial class MainWindow : Window
    {
        private DatabaseHelper dbHelper;

        public MainWindow()
        {
            InitializeComponent();
            dbHelper = new DatabaseHelper();
            LoadReminders();
        }

        private void LoadReminders()
        {
            var allReminders = dbHelper.GetAllReminders();

            // 1. Liste: Tamamlanmamış VE Aktif olanlar
            LstActiveReminders.ItemsSource = allReminders.Where(r => !r.IsCompleted && r.IsActive).ToList();

            // 2. Liste: Tamamlanmamış AMA Pasife alınmış (Arşivlenmiş) olanlar
            LstArchiveReminders.ItemsSource = allReminders.Where(r => !r.IsCompleted && !r.IsActive).ToList();

            // 3. Liste: Tamamlanmış olanlar (Aktiflik durumu önemsiz)
            LstCompletedReminders.ItemsSource = allReminders.Where(r => r.IsCompleted).ToList();
        }

        private void BtnAddReminder_Click(object sender, RoutedEventArgs e)
        {
            Reminder newReminder = new Reminder
            {
                EventDate = DateTime.Now.AddHours(1),
                Recurrence = RecurrenceType.None,
                IsActive = true
            };

            ReminderDetailWindow addWindow = new ReminderDetailWindow(newReminder);
            addWindow.Owner = this;
            addWindow.ShowDialog();
            LoadReminders();
        }

        private void LstReminders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox listBox = sender as ListBox;
            if (listBox != null && listBox.SelectedItem is Reminder selectedReminder)
            {
                ReminderDetailWindow detailWindow = new ReminderDetailWindow(selectedReminder);
                detailWindow.Owner = this;
                detailWindow.ShowDialog();

                LoadReminders();
                listBox.SelectedItem = null;
            }
        }

        // AKILLI SWITCH: Kapatılırken uyarı verir, açılırken direkt aktife alır
        private void TglActive_Click(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.Primitives.ToggleButton tgl && tgl.DataContext is Reminder clickedReminder)
            {
                bool isTurningOn = tgl.IsChecked ?? false;

                // Eğer kullanıcı görevi kapatıyorsa (Arşive atıyorsa)
                if (!isTurningOn)
                {
                    MessageBoxResult result = MessageBox.Show($"'{clickedReminder.Title}' görevini durdurup Arşiv sekmesine kaldırmak istiyor musunuz?", "Arşive Taşı", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.No)
                    {
                        // Kullanıcı vazgeçerse switch'i eski açık haline geri getir
                        tgl.IsChecked = true;
                        e.Handled = true;
                        return;
                    }
                }

                // Veritabanını güncelle
                clickedReminder.IsActive = isTurningOn;
                dbHelper.UpdateReminder(clickedReminder);

                // Listeleri yenile (Görev sekmeler arası yer değiştirecek)
                LoadReminders();
            }
            e.Handled = true;
        }

        private void BtnComplete_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null && button.DataContext is Reminder clickedReminder)
            {
                MessageBoxResult result = MessageBox.Show($"'{clickedReminder.Title}' görevini başarıyla tamamladınız mı?", "Tebrikler!", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    clickedReminder.IsCompleted = true;
                    clickedReminder.IsActive = false; // Bittiği için alarm motorundan düşürüyoruz
                    dbHelper.UpdateReminder(clickedReminder);
                    LoadReminders();
                }
            }
            e.Handled = true;
        }

        // BU BUTON HER 3 SEKMEDE DE "VERİTABANINDAN KALICI SİLME" İŞLEMİ YAPAR
        private void BtnQuickDelete_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null && button.DataContext is Reminder clickedReminder)
            {
                MessageBoxResult result = MessageBox.Show($"Bu kaydı veritabanından KALICI OLARAK silmek istiyor musunuz?\n\n(Bu işlem geri alınamaz!)", "Kalıcı Silme Onayı", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    dbHelper.DeleteReminder(clickedReminder.Id);
                    LoadReminders();
                }
            }
            e.Handled = true;
        }
    }
}