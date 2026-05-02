using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeAnchor.Models
{
    // Tekrarlanma tiplerimizi bir sözlük gibi tanımlıyoruz
    public enum RecurrenceType
    {
        None = 0,     // Tek Seferlik
        Daily = 1,    // Her Gün
        Weekly = 2,   // Her Hafta
        Monthly = 3,  // Her Ay
        Yearly = 4    // Her Yıl
    }

    public class Reminder
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime EventDate { get; set; }
        public bool IsTimeSpecific { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsSynced { get; set; }

        // YENİ EKLENEN ÖZELLİK: Görevin periyot türü
        public RecurrenceType Recurrence { get; set; }
    }
}