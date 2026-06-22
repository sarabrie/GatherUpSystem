using System;
using GatherUp.Core.Interfaces;

namespace GatherUp.BL.Services
{
    public class SimpleNotificationBridge : IMailNotificationBridge
    {
        public event Action<int, string> OnParticipantAction;
        public event Action<int, string> OnEventAction;
        public event Action<int, string> OnNewPoll;

        public void TriggerParticipantAction(int eventId, string actionType)
            => OnParticipantAction?.Invoke(eventId, actionType);

        public void TriggerEventAction(int eventId, string actionType)
            => OnEventAction?.Invoke(eventId, actionType);

        public void TriggerNewPoll(int eventId, string pollTitle)
            => OnNewPoll?.Invoke(eventId, pollTitle);
    }
}
