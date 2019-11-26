using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Plugin.FirebasePushNotification
{
    [BroadcastReceiver]
    public class PushNotificationActionReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            IDictionary<string, object> parameters = new Dictionary<string, object>();
            var extras = intent.Extras;

            if (extras != null && !extras.IsEmpty)
            {
                foreach (var key in extras.KeySet())
                {
                    parameters.Add(key, $"{extras.Get(key)}");
                    System.Diagnostics.Debug.WriteLine(key, $"{extras.Get(key)}");
                }
            }
            FirebasePushNotificationManager.RegisterAction(parameters);

            NotificationManager manager = context.GetSystemService(Context.NotificationService) as NotificationManager;
            var notificationId = extras.GetInt(DefaultPushNotificationHandler.ActionNotificationIdKey, -1);
            if (notificationId != -1)
            {
                var notificationTag = extras.GetString(DefaultPushNotificationHandler.ActionNotificationTagKey, string.Empty);

                if (notificationTag == null)
                    manager.Cancel(notificationId);
                else
                    manager.Cancel(notificationTag, notificationId);

            }
        }
    }
}