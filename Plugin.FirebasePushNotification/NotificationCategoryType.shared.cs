using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.FirebasePushNotification
{
    //This just applies for iOS on Android is always set as default when used
    public enum NotificationCategoryType
    {
        Default,
        Custom,
        Dismiss
    }
}
