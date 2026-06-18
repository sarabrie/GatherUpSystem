using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GatherUp.Core.Enums;


namespace GatherUp.Core.DO.Users
{
    
    public class Participant : Person
    {
        public bool? IsAttending { get; set; } = false;
        public bool HasPaid { get; set; }=false;
        public decimal AmountContributed { get; set; }=Decimal.Zero;
        public List<MailingPreference> MailingPreferences { get; set; } = new List<MailingPreference>();
        public NotificationPreferences NotificationSettings { get; set; } = 
        NotificationPreferences.EventChanges | NotificationPreferences.NewPolls | NotificationPreferences.AdminMessages;
        
    }
}
