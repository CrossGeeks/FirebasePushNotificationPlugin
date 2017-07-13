

using System.Collections.Generic;

namespace Plugin.FirebasePushNotification.Abstractions
{
    public interface IPushNotificationHandler
    {
        //Method triggered when an error occurs
        void OnError(string error);
        //Method triggered when a notification is opened
        void OnOpened(NotificationResponse response);
        //Method triggered when a notification is received
        void OnReceived(IDictionary<string, string> parameters);
        //Method triggered when notification user categories are requested to be setup
        NotificationUserCategory[] NotificationUserCategories { get; }
    }
}