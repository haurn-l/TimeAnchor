using System.Windows;
using TimeAnchor.Models;
using System.Windows.Input;

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
        }

        private void BtnComplete_Click(object sender, RoutedEventArgs e)
        {
            UserChoice = AlarmResult.Complete;
            this.Close();
        }

        private void BtnSnooze_Click(object sender, RoutedEventArgs e)
        {
            UserChoice = AlarmResult.Snooze;
            this.Close();
        }

        private void BtnIgnore_Click(object sender, RoutedEventArgs e)
        {
            UserChoice = AlarmResult.Ignore;
            this.Close();
        }
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }
    }
}