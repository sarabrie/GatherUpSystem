using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace GatherUp.Core.Interfaces
{
    public interface IMailNotificationBridge
    {
        event Action<int, string> OnParticipantAction;
        event Action<int, string> OnEventAction;
        event Action<int, string> OnNewPoll;

        void TriggerParticipantAction(int eventId, string actionType);
        void TriggerEventAction(int eventId, string actionType);
        void TriggerNewPoll(int eventId, string pollTitle);
    }
}
