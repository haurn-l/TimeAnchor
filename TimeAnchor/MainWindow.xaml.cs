using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading; 
using TimeAnchor.Models;
using TimeAnchor.Repositories;

namespace TimeAnchor
{
    public partial class MainWindow : Window
    {
        private DatabaseHelper dbHelper;
        private DispatcherTimer _heartbeatTimer;
        private DispatcherTimer _clockTimer;   
        private System.Windows.Forms.NotifyIcon _notifyIcon;
        public MainWindow()
        {
            InitializeComponent();
            dbHelper = new DatabaseHelper();
            LoadReminders();
            LoadWindowSettings();
            StartHeartbeat();
            StartLiveClock();
            SetupSystemTray();
        }

        private void StartHeartbeat()
        {
            _heartbeatTimer = new DispatcherTimer();
            _heartbeatTimer.Interval = TimeSpan.FromSeconds(10); 
            _heartbeatTimer.Tick += Heartbeat_Tick;
            _heartbeatTimer.Start();
        }
        private void StartLiveClock()
        {
            _clockTimer = new DispatcherTimer();
            _clockTimer.Interval = TimeSpan.FromSeconds(1); 
            _clockTimer.Tick += Clock_Tick;
            _clockTimer.Start();

            Clock_Tick(null, null); 
        }

        private void Clock_Tick(object sender, EventArgs e)
        {
            TxtLiveTime.Text = DateTime.Now.ToString("HH:mm:ss");
            TxtLiveDate.Text = DateTime.Now.ToString("dd.MM.yyyy"); 
        }

        private void BtnCalendar_Click(object sender, RoutedEventArgs e)
        {
            CalendarPopup.IsOpen = true;
        }

        private void Heartbeat_Tick(object sender, EventArgs e)
        {
            var activeReminders = dbHelper.GetAllReminders().Where(r => !r.IsCompleted && r.IsActive).ToList();
            DateTime now = DateTime.Now;

            foreach (var reminder in activeReminders)
            {
                if (reminder.EventDate <= now)
                {
                    _heartbeatTimer.Stop(); 

                    AlarmWindow alarmWin = new AlarmWindow(reminder);

                    alarmWin.Topmost = true;
                    alarmWin.ShowDialog();

                    if (alarmWin.UserChoice == AlarmResult.Complete)
                    {
                        CompleteTaskWithLogic(reminder);
                    }
                    else if (alarmWin.UserChoice == AlarmResult.Snooze)
                    {
                        reminder.EventDate = now.AddMinutes(15);
                        dbHelper.UpdateReminder(reminder);
                    }

                    LoadReminders();
                    _heartbeatTimer.Start();
                    break;
                }
            }
        }

        private void CompleteTaskWithLogic(Reminder task)
        {
            if (task.Recurrence == RecurrenceType.None)
            {
                task.IsCompleted = true;
                task.IsActive = false;
                dbHelper.UpdateReminder(task);
            }
            else
            {
                Reminder historyClone = new Reminder
                {
                    Title = task.Title,
                    Description = task.Description,
                    EventDate = task.EventDate, 
                    IsTimeSpecific = task.IsTimeSpecific,
                    IsCompleted = true, 
                    IsActive = false,
                    IsSynced = task.IsSynced,
                    Recurrence = task.Recurrence
                };
                dbHelper.AddReminder(historyClone); 

                switch (task.Recurrence)
                {
                    case RecurrenceType.Daily: task.EventDate = task.EventDate.AddDays(1); break;
                    case RecurrenceType.Weekly: task.EventDate = task.EventDate.AddDays(7); break; 
                    case RecurrenceType.Monthly: task.EventDate = task.EventDate.AddMonths(1); break;
                    case RecurrenceType.Yearly: task.EventDate = task.EventDate.AddYears(1); break;
                }
                dbHelper.UpdateReminder(task);
            }
        }

        private void LoadReminders()
        {
            var allReminders = dbHelper.GetAllReminders();
            LstActiveReminders.ItemsSource = allReminders.Where(r => !r.IsCompleted && r.IsActive).ToList();
            LstArchiveReminders.ItemsSource = allReminders.Where(r => !r.IsCompleted && !r.IsActive).ToList();
            LstCompletedReminders.ItemsSource = allReminders.Where(r => r.IsCompleted).ToList();
        }

        private void BtnAddReminder_Click(object sender, RoutedEventArgs e)
        {
            Reminder newReminder = new Reminder { EventDate = DateTime.Now.AddHours(1), Recurrence = RecurrenceType.None, IsActive = true };
            ReminderDetailWindow addWindow = new ReminderDetailWindow(newReminder) { Owner = this };
            addWindow.ShowDialog();
            LoadReminders();
        }

        private void LstReminders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListBox listBox && listBox.SelectedItem is Reminder selectedReminder)
            {
                ReminderDetailWindow detailWindow = new ReminderDetailWindow(selectedReminder) { Owner = this };
                detailWindow.ShowDialog();
                LoadReminders();
                listBox.SelectedItem = null;
            }
        }

        private void TglActive_Click(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.Primitives.ToggleButton tgl && tgl.DataContext is Reminder clickedReminder)
            {
                bool isTurningOn = tgl.IsChecked ?? false;
                if (!isTurningOn)
                {
                    MessageBoxResult result = MessageBox.Show($"'{clickedReminder.Title}' görevini durdurup Arşiv sekmesine kaldırmak istiyor musunuz?", "Arşive Taşı", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.No) { tgl.IsChecked = true; e.Handled = true; return; }
                }
                clickedReminder.IsActive = isTurningOn;
                dbHelper.UpdateReminder(clickedReminder);
                LoadReminders();
            }
            e.Handled = true;
        }

        private void BtnComplete_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Reminder clickedReminder)
            {
                MessageBoxResult result = MessageBox.Show($"'{clickedReminder.Title}' görevini başarıyla tamamladınız mı?", "Tebrikler!", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    CompleteTaskWithLogic(clickedReminder);
                    LoadReminders();
                }
            }
            e.Handled = true;
        }

        private void BtnQuickDelete_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Reminder clickedReminder)
            {
                MessageBoxResult result = MessageBox.Show($"Bu kaydı veritabanından KALICI OLARAK silmek istiyor musunuz?", "Kalıcı Silme Onayı", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    dbHelper.DeleteReminder(clickedReminder.Id);
                    LoadReminders();
                }
            }
            e.Handled = true;
        }
        // --- SYSTEM TRAY (HAYALET MOD) İŞLEMLERİ ---
        private void SetupSystemTray()
        {
            _notifyIcon = new System.Windows.Forms.NotifyIcon();

            // Şimdilik Windows'un varsayılan "Bilgi" ikonunu kullanıyoruz
            try
            {
                _notifyIcon.Icon = new System.Drawing.Icon("TimeAnchor.ico");
            }
            catch
            {
                // Eğer bir hata olursa uygulama çökmesin diye varsayılanı kullanmaya devam etsin
                _notifyIcon.Icon = System.Drawing.SystemIcons.Information;
            }
            _notifyIcon.Visible = true;
            _notifyIcon.Text = "TimeAnchor Arka Planda Çalışıyor...";

            // İkona çift tıklanınca uygulamayı ekrana geri getir
            _notifyIcon.DoubleClick += (s, e) => ShowApplication();

            // İkona sağ tıklayınca açılacak menü (Profesyonel dokunuş)
            var contextMenu = new System.Windows.Forms.ContextMenuStrip();
            contextMenu.Items.Add("TimeAnchor'ı Aç", null, (s, e) => ShowApplication());
            contextMenu.Items.Add("Tamamen Çıkış Yap", null, (s, e) => ExitApplication());
            _notifyIcon.ContextMenuStrip = contextMenu;
        }

        private void ShowApplication()
        {
            this.Show(); // Pencereyi görünür yap
            this.WindowState = WindowState.Normal; // Küçültülmüşse normal boyuta al
            this.Topmost = true; // Uygulamayı diğer pencerelerin önüne getir
            this.Topmost = false; // Sürekli en önde kalmasın diye geri bırak
        }

        private void ExitApplication()
        {
            // İkonu görev çubuğundan temizle ve uygulamayı tamamen öldür
            SaveWindowSettings();
            _notifyIcon.Visible = false;
            _notifyIcon.Dispose();
            System.Windows.Application.Current.Shutdown();
        }

        // EN KRİTİK NOKTA: Kullanıcı sağ üstteki 'X' butonuna bastığında çalışan metodu eziyoruz (Override)
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            SaveWindowSettings();
            e.Cancel = true; // Uygulamanın Windows tarafından tamamen kapatılmasını İPTAL ET
            this.Hide();     // Pencereyi sadece gizle (Hayalet moda geç)

            // Kullanıcıya küçük bir balon bildirimle bilgi ver (2 saniye ekranda kalır)
            _notifyIcon.ShowBalloonTip(2000, "TimeAnchor", "Arka planda çalışmaya devam ediyorum. Görev çubuğundan (saatin yanından) bana ulaşabilirsin.", System.Windows.Forms.ToolTipIcon.Info);
        }
        // --- PENCERE BOYUT HAFIZASI ---
        private string settingsFile = "window_settings.txt";

        private void LoadWindowSettings()
        {
            try
            {
                if (System.IO.File.Exists(settingsFile))
                {
                    // Dosyadan oku ve ayır
                    var parts = System.IO.File.ReadAllText(settingsFile).Split(',');
                    this.Width = double.Parse(parts[0]);
                    this.Height = double.Parse(parts[1]);
                    this.Top = double.Parse(parts[2]);
                    this.Left = double.Parse(parts[3]);

                    // Windows'un bunu ekranın ortasında değil, bizim verdiğimiz koordinatlarda açması için:
                    this.WindowStartupLocation = WindowStartupLocation.Manual;
                }
            }
            catch { } // Eğer dosya bozuksa hiçbir şey yapma, varsayılan boyutta açılsın
        }

        private void SaveWindowSettings()
        {
            try
            {
                // Sadece pencere normal durumdayken (Tam ekran vs değilken) kaydet
                if (this.WindowState == WindowState.Normal)
                {
                    string data = $"{this.Width},{this.Height},{this.Top},{this.Left}";
                    System.IO.File.WriteAllText(settingsFile, data);
                }
            }
            catch { }
        }
    }
}