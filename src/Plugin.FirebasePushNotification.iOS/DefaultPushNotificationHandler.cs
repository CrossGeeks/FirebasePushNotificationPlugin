using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using Plugin.FirebasePushNotification.Abstractions;
using UserNotifications;
using System.Collections.ObjectModel;

namespace Plugin.FirebasePushNotification
{
    public class DefaultPushNotificationHandler : IPushNotificationHandler
    {
        public const string DomainTag = "DefaultPushNotificationHandler";

        public void OnError(string error)
        {
            System.Diagnostics.Debug.WriteLine($"{DomainTag} - OnError - {error}");
        }

        public void OnOpened(NotificationResponse response)
        {
            System.Diagnostics.Debug.WriteLine($"{DomainTag} - OnOpened");
        }

        public void OnReceived(IDictionary<string, string> parameters)
        {
            System.Diagnostics.Debug.WriteLine($"{DomainTag} - OnReceived");
        }
    }
}