using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using TimeAnchor.Models;

namespace TimeAnchor
{
    public enum AlarmResult
    {
        Complete,
        Snooze,
        Ignore
    }

    public partial class AlarmWindow : Window
    {
        public AlarmResult UserChoice { get; private set; }

        private MediaPlayer _mediaPlayer;

        public AlarmWindow(Reminder task)
        {
            InitializeComponent();

            TxtTaskTitle.Text = task.Title;

            if (string.IsNullOrWhiteSpace(task.Description))
            {
                TxtTaskDescription.Visibility = Visibility.Collapsed;
            }
            else
            {
                TxtTaskDescription.Text = task.Description;
                TxtTaskDescription.Visibility = Visibility.Visible;
            }

            UserChoice = AlarmResult.Ignore;

            PlayAlarmSound(task.AlarmSoundPath);
        }

        private void PlayAlarmSound(string soundPath)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(soundPath) && File.Exists(soundPath))
                {
                    _mediaPlayer = new MediaPlayer();
                    _mediaPlayer.Open(new Uri(soundPath));
                    _mediaPlayer.Volume = 1.0; 
                    _mediaPlayer.Play();
                }
                else
                {
                    System.Media.SystemSounds.Exclamation.Play();
                }
            }
            catch
            {
            }
        }

        private void StopAlarmSound()
        {
            if (_mediaPlayer != null)
            {
                _mediaPlayer.Stop();
                _mediaPlayer.Close();
            }
        }

        private void BtnComplete_Click(object sender, RoutedEventArgs e)
        {
            UserChoice = AlarmResult.Complete;
            StopAlarmSound(); // Sesi Sustur
            this.Close();
        }

        private void BtnSnooze_Click(object sender, RoutedEventArgs e)
        {
            UserChoice = AlarmResult.Snooze;
            StopAlarmSound(); // Sesi Sustur
            this.Close();
        }

        private void BtnIgnore_Click(object sender, RoutedEventArgs e)
        {
            UserChoice = AlarmResult.Ignore;
            StopAlarmSound(); // Sesi Sustur
            this.Close();
        }

        // Sürükleme metodu aynen kalıyor
        private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == System.Windows.Input.MouseButton.Left)
            {
                this.DragMove();
            }
        }
    }
}