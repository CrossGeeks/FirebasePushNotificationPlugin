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
using Firebase.Messaging;
using Plugin.FirebasePushNotification.Abstractions;
using Android.Support.V4.Util;
using Java.Lang;
using Android.Support.V4.App;

namespace Plugin.FirebasePushNotification
{
    [Service]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    public class PNFirebaseMessagingService : FirebaseMessagingService
    {
        public override void OnMessageReceived(RemoteMessage message)
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            var notification = message.GetNotification();
            if (notification != null)
            {
                if (!string.IsNullOrEmpty(notification.Body))
                    parameters.Add("body",notification.Body);

                if (!string.IsNullOrEmpty(notification.Title))
                    parameters.Add("title", notification.Title);

                if (!string.IsNullOrEmpty(notification.Tag))
                    parameters.Add("tag", notification.Tag);

                if (!string.IsNullOrEmpty(notification.Sound))
                    parameters.Add("sound", notification.Sound);

                if (!string.IsNullOrEmpty(notification.Icon))
                    parameters.Add("icon", notification.Icon);

                if (notification.Link!=null)
                    parameters.Add("link_path", notification.Link.Path);

                if (!string.IsNullOrEmpty(notification.ClickAction))
                    parameters.Add("click_action", notification.ClickAction);

                if(!string.IsNullOrEmpty(notification.Color))
                    parameters.Add("color", notification.Color);
            }
            foreach(var d in message.Data)
            {
                if(!parameters.ContainsKey(d.Key))
                    parameters.Add(d.Key, d.Value);
            }

            FirebasePushNotificationManager.RegisterData(parameters);
            CrossFirebasePushNotification.Current.NotificationHandler?.OnReceived(parameters);
        }
    }
}