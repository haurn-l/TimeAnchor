using System;
using System.Windows;
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
            TxtTitle.Text = _currentReminder.Title;
            TxtDescription.Text = _currentReminder.Description;
            DpDate.SelectedDate = _currentReminder.EventDate.Date;
            TpTime.SelectedTime = _currentReminder.EventDate;
            CmbRecurrence.SelectedIndex = (int)_currentReminder.Recurrence;

            if (_currentReminder.Id == 0)
            {
                this.Title = "Yeni Görev Ekle"; 
                BtnSave.Content = "KAYDET"; 
                BtnDelete.Visibility = Visibility.Collapsed; 
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            _currentReminder.Title = TxtTitle.Text;
            _currentReminder.Description = TxtDescription.Text;
            _currentReminder.Recurrence = (RecurrenceType)CmbRecurrence.SelectedIndex;

            if (DpDate.SelectedDate.HasValue && TpTime.SelectedTime.HasValue)
            {
                DateTime selectedDate = DpDate.SelectedDate.Value;
                DateTime selectedTime = TpTime.SelectedTime.Value;
                DateTime combinedDate = new DateTime(selectedDate.Year, selectedDate.Month, selectedDate.Day, selectedTime.Hour, selectedTime.Minute, 0);

                // GÜVENLİK DUVARI
                if (_currentReminder.Recurrence == RecurrenceType.None && combinedDate < DateTime.Now)
                {
                    MessageBox.Show("Tek seferlik görevler geçmiş bir zamana kurulamaz! Lütfen ileri bir tarih/saat seçin.", "Geçersiz Zaman", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                _currentReminder.EventDate = combinedDate;
            }
            if (_currentReminder.Id == 0)
            {
                _dbHelper.AddReminder(_currentReminder);
            }
            else
            {
                _dbHelper.UpdateReminder(_currentReminder);
            }

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