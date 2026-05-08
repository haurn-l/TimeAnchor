using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeAnchor.Models
{
    public enum RecurrenceType
    {
        None = 0,
        Daily = 1,
        Weekly = 2,
        Monthly = 3,
        Yearly = 4
    }

    public class Reminder
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime EventDate { get; set; }
        public bool IsTimeSpecific { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsActive { get; set; }
        public bool IsSynced { get; set; }
        public RecurrenceType Recurrence { get; set; }
        public string Category { get; set; }
        public string AlarmSoundPath { get; set; }
    }
}