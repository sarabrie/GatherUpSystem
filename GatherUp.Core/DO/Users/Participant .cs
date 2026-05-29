using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatherUp.Core.DO.Users
{
    public enum MailingPreference
    {
        Email,
        Sms,
        None
    }
    internal class Participant : Person
    {
        public bool? IsAttending { get; set; }
        public bool HasPaid { get; set; }
        public decimal AmountContributed { get; set; }
        public List<MailingPreference> MailingPreferences { get; set; } = new List<MailingPreference>();
    }
}
