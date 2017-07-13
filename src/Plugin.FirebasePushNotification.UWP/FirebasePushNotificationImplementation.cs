using Plugin.FirebasePushNotification.Abstractions;
using System;
using System.Collections.Generic;

namespace Plugin.FirebasePushNotification
{
    /// <summary>
    /// Implementation for Feature
    /// </summary>
    public class FirebasePushNotificationImplementation : IFirebasePushNotification
    {
        public string Token => throw new NotImplementedException();

        public IDictionary<string, string> UserActions => throw new NotImplementedException();

        public Action<NotificationResponse> OnUserAction => throw new NotImplementedException();

        public IPushNotificationHandler NotificationHandler { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Action<string> OnTokenRefresh => throw new NotImplementedException();

        public Action<IDictionary<string, string>> OnNotificationReceived => throw new NotImplementedException();

        public Action<IDictionary<string, string>> OnNotificationOpened => throw new NotImplementedException();

        public void RegisterUserActions(IDictionary<string, string> userActions)
        {
            throw new NotImplementedException();
        }

        public void SendDeviceGroupMessage(IDictionary<string, string> parameters, string groupKey, string messageId, int timeOfLive)
        {
            throw new NotImplementedException();
        }

        public void Subscribe(string topic)
        {
            
        }

        public void Unsubscribe(string topic)
        {
        
        }
    }
}