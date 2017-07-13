using System;
using System.Collections.Generic;
using System.Text;

namespace Plugin.FirebasePushNotification.Abstractions
{
    //This just applies for iOS on Android is always set as default when used
    public enum NotificationActionType
    {
        Default,
        AuthenticationRequired,
        Foreground,
        Destructive
    }
}
