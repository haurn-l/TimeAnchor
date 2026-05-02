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
using TimeAnchor.Models;
using TimeAnchor.Repositories;

namespace TimeAnchor
{
    public partial class ReminderDetailWindow : Window
    {
        private Reminder _currentReminder;
        private DatabaseHelper _dbHelper;

        public ReminderDetailWindow(Reminder selectedReminder)
        {
            InitializeComponent();
            _dbHelper = new DatabaseHelper();
            _currentReminder = selectedReminder;

            // Pencere açılır açılmaz verileri doldur
            TxtTitle.Text = _currentReminder.Title;
            TxtDescription.Text = _currentReminder.Description;
            DpDate.SelectedDate = _currentReminder.EventDate.Date;
            TpTime.SelectedTime = _currentReminder.EventDate;

            // Veritabanındaki değere göre (0, 1, 2, 3, 4) açılır listeyi (ComboBox) ayarla
            CmbRecurrence.SelectedIndex = (int)_currentReminder.Recurrence;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // Kutulardaki verileri al
            _currentReminder.Title = TxtTitle.Text;
            _currentReminder.Description = TxtDescription.Text;

            // ComboBox'tan seçilen satırın indexini (0,1,2,3,4) Enum'a çeviriyoruz
            _currentReminder.Recurrence = (RecurrenceType)CmbRecurrence.SelectedIndex;

            if (DpDate.SelectedDate.HasValue && TpTime.SelectedTime.HasValue)
            {
                DateTime selectedDate = DpDate.SelectedDate.Value;
                DateTime selectedTime = TpTime.SelectedTime.Value;
                DateTime combinedDate = new DateTime(selectedDate.Year, selectedDate.Month, selectedDate.Day, selectedTime.Hour, selectedTime.Minute, 0);

                // GÜVENLİK DUVARI: Görev "Tek Seferlik" (None) ise VE geçmiş bir zamansa ENGELLE!
                if (_currentReminder.Recurrence == RecurrenceType.None && combinedDate < DateTime.Now)
                {
                    MessageBox.Show("Tek seferlik görevler geçmiş bir zamana kurulamaz! Lütfen ileri bir tarih/saat seçin.", "Geçersiz Zaman", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return; // Metodu durdur, veritabanına kaydetme
                }

                _currentReminder.EventDate = combinedDate;
            }

            // Güvenlik duvarından geçtiyse güncellemeyi yap
            _dbHelper.UpdateReminder(_currentReminder);
            this.Close();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Bu görevi silmek istediğinize emin misiniz?", "Silme Onayı", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                _dbHelper.DeleteReminder(_currentReminder.Id);
                this.Close();
            }
        }
    }
}