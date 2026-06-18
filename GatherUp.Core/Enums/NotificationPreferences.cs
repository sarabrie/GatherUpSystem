using System;
using System.Collections.Generic;
using System.Linq;


namespace GatherUp.Core.Enums
{
    [Flags] // האטרביוט הזה אומר ל-C# להתייחס לזה כמסיכת ביטים
    public enum NotificationPreferences
    {
        None = 0,
        EventChanges = 1,    // בינארית: 001
        AdminMessages = 2,   // בינארית: 010
        NewPolls = 4         // בינארית: 100
    }
}