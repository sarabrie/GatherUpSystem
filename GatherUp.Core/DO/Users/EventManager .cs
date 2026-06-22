using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;
using GatherUp.Core.Enums;

namespace GatherUp.Core.DO.Users
{
    public class EventManager : Person
    {
        [SetsRequiredMembers]
        public EventManager() { }

        [XmlElement]
        public NotificationPreferences NotificationSettings { get; set; } = NotificationPreferences.None;
    }
}
