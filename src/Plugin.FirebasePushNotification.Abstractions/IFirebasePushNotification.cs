using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Plugin.FirebasePushNotification.Abstractions
{

    public delegate void FirebasePushNotificationTokenEventHandler(object source, FirebasePushNotificationTokenEventArgs e);

    public class FirebasePushNotificationTokenEventArgs : EventArgs
    {
        public string Token { get; }

        public FirebasePushNotificationTokenEventArgs(string token)
        {
            Token = token;
        }
        
    }

    public delegate void FirebasePushNotificationErrorEventHandler(object source, FirebasePushNotificationErrorEventArgs e);

    public class FirebasePushNotificationErrorEventArgs : EventArgs
    {
        public string Message { get; }

        public FirebasePushNotificationErrorEventArgs(string message)
        {
            Message = message;
        }

    }

    public delegate void FirebasePushNotificationDataEventHandler(object source, FirebasePushNotificationDataEventArgs e);

    public class FirebasePushNotificationDataEventArgs : EventArgs
    {
        public IDictionary<string, object> Data { get; }

        public FirebasePushNotificationDataEventArgs(IDictionary<string, object> data)
        {
            Data = data;
        }

    }


    public delegate void FirebasePushNotificationResponseEventHandler(object source, FirebasePushNotificationResponseEventArgs e);

    public class FirebasePushNotificationResponseEventArgs : EventArgs
    {
        public string Identifier { get; }

        public IDictionary<string, object> Data { get; }

        public NotificationCategoryType Type { get; }

        public FirebasePushNotificationResponseEventArgs(IDictionary<string, object> data, string identifier = "", NotificationCategoryType type = NotificationCategoryType.Default)
        {
            Identifier = identifier;
            Data = data;
            Type = type;
        }

    }

    /// <summary>
    /// Interface for FirebasePushNotification
    /// </summary>
    public interface IFirebasePushNotification
  {
        /// <summary>
        /// Get all user notification categories
        /// </summary>
        NotificationUserCategory[] GetUserNotificationCategories();
        /// <summary>
        /// Get all subscribed topics
        /// </summary>
        string[] SubscribedTopics { get; }
        /// <summary>
        /// Subscribe to multiple topics
        /// </summary>
        void Subscribe(string[] topics);
        /// <summary>
        /// Subscribe to one topic
        /// </summary>
        void Subscribe(string topic);
        /// <summary>
        /// Unsubscribe to one topic
        /// </summary>
        void Unsubscribe(string topic);
        /// <summary>
        /// Unsubscribe to multiple topics
        /// </summary>
        void Unsubscribe(string[] topics);
        /// <summary>
        /// Unsubscribe all topics
        /// </summary>
        void UnsubscribeAll();
        /// <summary>
        /// Notification handler to receive, customize notification feedback and provide user actions
        /// </summary>
        IPushNotificationHandler NotificationHandler { get; set; }

        /// <summary>
        /// Event triggered when token is refreshed
        /// </summary>
        event FirebasePushNotificationTokenEventHandler OnTokenRefresh;
        /// <summary>
        /// Event triggered when a notification is opened
        /// </summary>
        event FirebasePushNotificationResponseEventHandler OnNotificationOpened;
        /// <summary>
        /// Event triggered when a notification is received
        /// </summary>
        event FirebasePushNotificationDataEventHandler OnNotificationReceived;
        /// <summary>
        /// Event triggered when there's an error
        /// </summary>
        event FirebasePushNotificationErrorEventHandler OnNotificationError;
        /// <summary>
        /// Push notification token
        /// </summary>
        string Token { get; }
        /// <summary>
        /// Send device group message
        /// </summary>
        void SendDeviceGroupMessage(IDictionary<string, string> parameters,string groupKey,string messageId,int timeOfLive);
    }
}
