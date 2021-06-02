using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Firebase.CloudMessaging;
using Firebase.Core;
using Firebase.InstanceID;
using Foundation;
using UIKit;
using UserNotifications;

namespace Plugin.FirebasePushNotification
{

    /// <summary>
    /// Implementation for FirebasePushNotification
    /// </summary>
    public class FirebasePushNotificationManager : NSObject, IFirebasePushNotification, IUNUserNotificationCenterDelegate, IMessagingDelegate
    {
        public static UNNotificationPresentationOptions CurrentNotificationPresentationOption { get; set; } = UNNotificationPresentationOptions.None;

        private static Queue<Tuple<string, bool>> pendingTopics = new Queue<Tuple<string, bool>>();
        private static bool hasToken = false;
        private static NSString FirebaseTopicsKey = new NSString("FirebaseTopics");
        private const string FirebaseTokenKey = "FirebaseToken";
        private static NSMutableArray currentTopics = (NSUserDefaults.StandardUserDefaults.ValueForKey(FirebaseTopicsKey) as NSArray ?? new NSArray()).MutableCopy() as NSMutableArray;
        public string Token { get { return string.IsNullOrEmpty(Messaging.SharedInstance.FcmToken) ? (NSUserDefaults.StandardUserDefaults.StringForKey(FirebaseTokenKey) ?? string.Empty) : Messaging.SharedInstance.FcmToken; } }

        private static IList<NotificationUserCategory> usernNotificationCategories = new List<NotificationUserCategory>();
        private static FirebasePushNotificationTokenEventHandler _onTokenRefresh;
        public event FirebasePushNotificationTokenEventHandler OnTokenRefresh
        {
            add
            {
                _onTokenRefresh += value;
            }
            remove
            {
                _onTokenRefresh -= value;
            }
        }

        private static FirebasePushNotificationDataEventHandler _onNotificationDeleted;
        public event FirebasePushNotificationDataEventHandler OnNotificationDeleted
        {
            add
            {
                _onNotificationDeleted += value;
            }
            remove
            {
                _onNotificationDeleted -= value;
            }
        }

        private static FirebasePushNotificationErrorEventHandler _onNotificationError;
        public event FirebasePushNotificationErrorEventHandler OnNotificationError
        {
            add
            {
                _onNotificationError += value;
            }
            remove
            {
                _onNotificationError -= value;
            }
        }

        private static FirebasePushNotificationResponseEventHandler _onNotificationOpened;
        public event FirebasePushNotificationResponseEventHandler OnNotificationOpened
        {
            add
            {
                _onNotificationOpened += value;
            }
            remove
            {
                _onNotificationOpened -= value;
            }
        }

        private static FirebasePushNotificationResponseEventHandler _onNotificationAction;
        public event FirebasePushNotificationResponseEventHandler OnNotificationAction
        {
            add
            {
                _onNotificationAction += value;
            }
            remove
            {
                _onNotificationAction -= value;
            }
        }


        public NotificationUserCategory[] GetUserNotificationCategories()
        {
            return usernNotificationCategories?.ToArray();
        }

        private static FirebasePushNotificationDataEventHandler _onNotificationReceived;
        public event FirebasePushNotificationDataEventHandler OnNotificationReceived
        {
            add
            {
                _onNotificationReceived += value;
            }
            remove
            {
                _onNotificationReceived -= value;
            }
        }


        public string[] SubscribedTopics
        {
            get
            {

                //Load all subscribed topics
                IList<string> topics = new List<string>();
                for (nuint i = 0; i < currentTopics.Count; i++)
                {
                    topics.Add(currentTopics.GetItem<NSString>(i));
                }
                return topics.ToArray();
            }

        }

        public IPushNotificationHandler NotificationHandler { get; set; }

        public static void Initialize(NSDictionary options, bool autoRegistration = true)
        {
            if (App.DefaultInstance == null)
            {
                App.Configure();
            }

            CrossFirebasePushNotification.Current.NotificationHandler = CrossFirebasePushNotification.Current.NotificationHandler ?? new DefaultPushNotificationHandler();
            Messaging.SharedInstance.AutoInitEnabled = autoRegistration;

            if (options?.ContainsKey(UIApplication.LaunchOptionsRemoteNotificationKey) ?? false)
            {
                var pushPayload = options[UIApplication.LaunchOptionsRemoteNotificationKey] as NSDictionary;
                if (pushPayload != null)
                {
                    var parameters = GetParameters(pushPayload);

                    var notificationResponse = new NotificationResponse(parameters, string.Empty, NotificationCategoryType.Default);


                    /*if (_onNotificationOpened == null && enableDelayedResponse)
                        delayedNotificationResponse = notificationResponse;
                    else*/
                    _onNotificationOpened?.Invoke(CrossFirebasePushNotification.Current, new FirebasePushNotificationResponseEventArgs(notificationResponse.Data, notificationResponse.Identifier, notificationResponse.Type));

                    CrossFirebasePushNotification.Current.NotificationHandler?.OnOpened(notificationResponse);
                }
            }

            if (autoRegistration)
            {
                CrossFirebasePushNotification.Current.RegisterForPushNotifications();
            }

        }
        public static void Initialize(NSDictionary options, IPushNotificationHandler pushNotificationHandler, bool autoRegistration = true)
        {
            CrossFirebasePushNotification.Current.NotificationHandler = pushNotificationHandler;
            Initialize(options, autoRegistration);
        }
        public static void Initialize(NSDictionary options, NotificationUserCategory[] notificationUserCategories, bool autoRegistration = true)
        {

            Initialize(options, autoRegistration);

            RegisterUserNotificationCategories(notificationUserCategories);

        }
        public static void RegisterUserNotificationCategories(NotificationUserCategory[] userCategories)
        {
            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
            {
                if (userCategories != null && userCategories.Length > 0)
                {
                    usernNotificationCategories.Clear();
                    IList<UNNotificationCategory> categories = new List<UNNotificationCategory>();
                    foreach (var userCat in userCategories)
                    {
                        IList<UNNotificationAction> actions = new List<UNNotificationAction>();

                        foreach (var action in userCat.Actions)
                        {

                            // Create action
                            var actionID = action.Id;
                            var title = action.Title;
                            var notificationActionType = UNNotificationActionOptions.None;
                            switch (action.Type)
                            {
                                case NotificationActionType.AuthenticationRequired:
                                    notificationActionType = UNNotificationActionOptions.AuthenticationRequired;
                                    break;
                                case NotificationActionType.Destructive:
                                    notificationActionType = UNNotificationActionOptions.Destructive;
                                    break;
                                case NotificationActionType.Foreground:
                                    notificationActionType = UNNotificationActionOptions.Foreground;
                                    break;

                            }


                            var notificationAction = UNNotificationAction.FromIdentifier(actionID, title, notificationActionType);

                            actions.Add(notificationAction);

                        }

                        // Create category
                        var categoryID = userCat.Category;
                        var notificationActions = actions.ToArray() ?? new UNNotificationAction[] { };
                        var intentIDs = new string[] { };
                        var categoryOptions = new UNNotificationCategoryOptions[] { };

                        var category = UNNotificationCategory.FromIdentifier(categoryID, notificationActions, intentIDs, userCat.Type == NotificationCategoryType.Dismiss ? UNNotificationCategoryOptions.CustomDismissAction : UNNotificationCategoryOptions.None);

                        categories.Add(category);

                        usernNotificationCategories.Add(userCat);

                    }

                    // Register categories
                    UNUserNotificationCenter.Current.SetNotificationCategories(new NSSet<UNNotificationCategory>(categories.ToArray()));

                }
            }

        }

        public void RegisterForPushNotifications()
        {

            Messaging.SharedInstance.AutoInitEnabled = true;

            Messaging.SharedInstance.Delegate = CrossFirebasePushNotification.Current as IMessagingDelegate;

            //Messaging.SharedInstance.ShouldEstablishDirectChannel = true;

            // Register your app for remote notifications.
            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
            {
                // iOS 10 or later
                var authOptions = UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound;

                // For iOS 10 display notification (sent via APNS)
                UNUserNotificationCenter.Current.Delegate = CrossFirebasePushNotification.Current as IUNUserNotificationCenterDelegate;

                UNUserNotificationCenter.Current.RequestAuthorization(authOptions, (granted, error) =>
                {
                    if (error != null)
                    {
                        _onNotificationError?.Invoke(CrossFirebasePushNotification.Current, new FirebasePushNotificationErrorEventArgs(FirebasePushNotificationErrorType.PermissionDenied, error.Description));
                    }
                    else if (!granted)
                    {
                        _onNotificationError?.Invoke(CrossFirebasePushNotification.Current, new FirebasePushNotificationErrorEventArgs(FirebasePushNotificationErrorType.PermissionDenied, "Push notification permission not granted"));
                    }
                    else
                    {
                        InvokeOnMainThread(() => UIApplication.SharedApplication.RegisterForRemoteNotifications());
                    }
                });

            }
            else
            {
                // iOS 9 or before
                var allNotificationTypes = UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound;
                var settings = UIUserNotificationSettings.GetSettingsForTypes(allNotificationTypes, null);
                UIApplication.SharedApplication.RegisterUserNotificationSettings(settings);

                UIApplication.SharedApplication.RegisterForRemoteNotifications();
            }

        }


        public void UnregisterForPushNotifications()
        {

            if (hasToken)
            {
                CrossFirebasePushNotification.Current.UnsubscribeAll();
                //Messaging.SharedInstance.ShouldEstablishDirectChannel = false;
                hasToken = false;
                // Disconnect();
            }

            Messaging.SharedInstance.AutoInitEnabled = false;
            UIApplication.SharedApplication.UnregisterForRemoteNotifications();
            NSUserDefaults.StandardUserDefaults.SetString(string.Empty, FirebaseTokenKey);
            InstanceId.SharedInstance.DeleteId((h) => { });
        }
        // To receive notifications in foreground on iOS 10 devices.
        [Export("userNotificationCenter:willPresentNotification:withCompletionHandler:")]
        public void WillPresentNotification(UNUserNotificationCenter center, UNNotification notification, Action<UNNotificationPresentationOptions> completionHandler)
        {
            // Do your magic to handle the notification data
            System.Console.WriteLine(notification.Request.Content.UserInfo);
            System.Diagnostics.Debug.WriteLine("WillPresentNotification");
            var parameters = GetParameters(notification.Request.Content.UserInfo);
            _onNotificationReceived?.Invoke(CrossFirebasePushNotification.Current, new FirebasePushNotificationDataEventArgs(parameters));
            CrossFirebasePushNotification.Current.NotificationHandler?.OnReceived(parameters);

            if ((parameters.TryGetValue("priority", out var priority) && ($"{priority}".ToLower() == "high" || $"{priority}".ToLower() == "max")))
            {
                if (!CurrentNotificationPresentationOption.HasFlag(UNNotificationPresentationOptions.Alert))
                {
                    CurrentNotificationPresentationOption |= UNNotificationPresentationOptions.Alert;

                }
            }
            else if ($"{priority}".ToLower() == "default" || $"{priority}".ToLower() == "low" || $"{priority}".ToLower() == "min")
            {
                if (CurrentNotificationPresentationOption.HasFlag(UNNotificationPresentationOptions.Alert))
                {
                    CurrentNotificationPresentationOption &= ~UNNotificationPresentationOptions.Alert;

                }
            }
            completionHandler(CurrentNotificationPresentationOption);
        }

        public static void DidReceiveMessage(NSDictionary data)
        {
            Messaging.SharedInstance.AppDidReceiveMessage(data);
            var parameters = GetParameters(data);

            _onNotificationReceived?.Invoke(CrossFirebasePushNotification.Current, new FirebasePushNotificationDataEventArgs(parameters));

            CrossFirebasePushNotification.Current.NotificationHandler?.OnReceived(parameters);
            System.Diagnostics.Debug.WriteLine("DidReceivedMessage");
        }
        [Obsolete("DidRegisterRemoteNotifications with these parameters is deprecated, please use the other override instead.")]
        public static void DidRegisterRemoteNotifications(NSData deviceToken, FirebaseTokenType type)
        {
            Messaging.SharedInstance.ApnsToken = deviceToken;
        }

        public static void DidRegisterRemoteNotifications(NSData deviceToken)
        {

            Messaging.SharedInstance.ApnsToken = deviceToken;
        }

        public static void RemoteNotificationRegistrationFailed(NSError error)
        {
            _onNotificationError?.Invoke(CrossFirebasePushNotification.Current, new FirebasePushNotificationErrorEventArgs(FirebasePushNotificationErrorType.RegistrationFailed, error.Description));
        }

        public void ApplicationReceivedRemoteMessage(RemoteMessage remoteMessage)
        {
            System.Console.WriteLine(remoteMessage.AppData);
            System.Diagnostics.Debug.WriteLine("ApplicationReceivedRemoteMessage");
            var parameters = GetParameters(remoteMessage.AppData);
            _onNotificationReceived?.Invoke(CrossFirebasePushNotification.Current, new FirebasePushNotificationDataEventArgs(parameters));
            CrossFirebasePushNotification.Current.NotificationHandler?.OnReceived(parameters);
        }

        private static IDictionary<string, object> GetParameters(NSDictionary data)
        {
            var parameters = new Dictionary<string, object>();

            var keyAps = new NSString("aps");
            var keyAlert = new NSString("alert");

            foreach (var val in data)
            {
                if (val.Key.Equals(keyAps))
                {
                    var aps = data.ValueForKey(keyAps) as NSDictionary;

                    if (aps != null)
                    {
                        foreach (var apsVal in aps)
                        {
                            if (apsVal.Value is NSDictionary)
                            {
                                if (apsVal.Key.Equals(keyAlert))
                                {
                                    foreach (var alertVal in apsVal.Value as NSDictionary)
                                    {
                                        parameters.Add($"aps.alert.{alertVal.Key}", $"{alertVal.Value}");
                                    }
                                }
                            }
                            else
                            {
                                parameters.Add($"aps.{apsVal.Key}", $"{apsVal.Value}");
                            }

                        }
                    }
                }
                else
                {
                    parameters.Add($"{val.Key}", $"{val.Value}");
                }

            }


            return parameters;
        }


        public void Subscribe(string[] topics)
        {
            foreach (var t in topics)
            {
                Subscribe(t);
            }
        }

        public void Subscribe(string topic)
        {
            if (!hasToken)
            {
                pendingTopics.Enqueue(new Tuple<string, bool>(topic, true));
                return;
            }

            if (!currentTopics.Contains(new NSString(topic)))
            {
                Messaging.SharedInstance.Subscribe($"{topic}");
                currentTopics.Add(new NSString(topic));
            }

            NSUserDefaults.StandardUserDefaults.SetValueForKey(currentTopics, FirebaseTopicsKey);
            NSUserDefaults.StandardUserDefaults.Synchronize();

        }

        public void UnsubscribeAll()
        {
            for (nuint i = 0; i < currentTopics.Count; i++)
            {
                Unsubscribe(currentTopics.GetItem<NSString>(i));
            }
        }

        public void Unsubscribe(string[] topics)
        {
            foreach (var t in topics)
            {
                Unsubscribe(t);
            }
        }

        public void Unsubscribe(string topic)
        {
            if (!hasToken)
            {
                pendingTopics.Enqueue(new Tuple<string, bool>(topic, false));
                return;
            }
            var deletedKey = new NSString($"{topic}");
            if (currentTopics.Contains(deletedKey))
            {
                Messaging.SharedInstance.Unsubscribe($"{topic}");
                var idx = (nint)currentTopics.IndexOf(deletedKey);
                if (idx != -1)
                {
                    currentTopics.RemoveObject(idx);

                }
            }
            NSUserDefaults.StandardUserDefaults.SetValueForKey(currentTopics, FirebaseTopicsKey);
            NSUserDefaults.StandardUserDefaults.Synchronize();

        }

        public void SendDeviceGroupMessage(IDictionary<string, string> parameters, string groupKey, string messageId, int timeOfLive)
        {
            if (hasToken)
            {
                using (var message = new NSMutableDictionary())
                {
                    foreach (var p in parameters)
                    {
                        message.Add(new NSString(p.Key), new NSString(p.Value));
                    }

                    Messaging.SharedInstance.SendMessage(message, groupKey, messageId, timeOfLive);
                }
                  
            }
        }

        [Export("userNotificationCenter:didReceiveNotificationResponse:withCompletionHandler:")]
        public void DidReceiveNotificationResponse(UNUserNotificationCenter center, UNNotificationResponse response, Action completionHandler)
        {

            var parameters = GetParameters(response.Notification.Request.Content.UserInfo);

            var catType = NotificationCategoryType.Default;
            if (response.IsCustomAction)
            {
                catType = NotificationCategoryType.Custom;
            }
            else if (response.IsDismissAction)
            {
                catType = NotificationCategoryType.Dismiss;
            }

            var ident = $"{response.ActionIdentifier}".Equals("com.apple.UNNotificationDefaultActionIdentifier", StringComparison.CurrentCultureIgnoreCase) ? string.Empty : $"{response.ActionIdentifier}";
            var notificationResponse = new NotificationResponse(parameters, ident, catType);

            if (string.IsNullOrEmpty(ident))
            {
                _onNotificationOpened?.Invoke(this, new FirebasePushNotificationResponseEventArgs(notificationResponse.Data, notificationResponse.Identifier, notificationResponse.Type));

                CrossFirebasePushNotification.Current.NotificationHandler?.OnOpened(notificationResponse);
            }
            else
            {
                _onNotificationAction?.Invoke(this, new FirebasePushNotificationResponseEventArgs(notificationResponse.Data, notificationResponse.Identifier, notificationResponse.Type));

                // CrossFirebasePushNotification.Current.NotificationHandler?.OnOpened(notificationResponse);
            }


            // Inform caller it has been handled
            completionHandler();
        }

        [Export("messaging:didReceiveRegistrationToken:")]
        public void DidReceiveRegistrationToken(Messaging messaging, string fcmToken)
        {
            // Note that this callback will be fired everytime a new token is generated, including the first
            // time. So if you need to retrieve the token as soon as it is available this is where that
            // should be done.
            var refreshedToken = fcmToken;
            if (!string.IsNullOrEmpty(refreshedToken))
            {
                _onTokenRefresh?.Invoke(CrossFirebasePushNotification.Current, new FirebasePushNotificationTokenEventArgs(refreshedToken));
                hasToken = true;
                while (pendingTopics.Count > 0)
                {
                    var pTopic = pendingTopics.Dequeue();
                    if (pTopic != null)
                    {
                        if (pTopic.Item2)
                        {
                            CrossFirebasePushNotification.Current.Subscribe(pTopic.Item1);
                        }
                        else
                        {
                            CrossFirebasePushNotification.Current.Unsubscribe(pTopic.Item1);
                        }
                    }
                }
            }

            NSUserDefaults.StandardUserDefaults.SetString(fcmToken ?? string.Empty, FirebaseTokenKey);
        }

        public void ClearAllNotifications()
        {
            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
            {
                UNUserNotificationCenter.Current.RemoveAllDeliveredNotifications(); // To remove all delivered notifications
            }
            else
            {
                UIApplication.SharedApplication.CancelAllLocalNotifications();
            }
        }
        public void RemoveNotification(string tag, int id)
        {
            RemoveNotification(id);
        }
        public async void RemoveNotification(int id)
        {
            var NotificationIdKey = "id";
            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
            {

                var deliveredNotifications = await UNUserNotificationCenter.Current.GetDeliveredNotificationsAsync();
                var deliveredNotificationsMatches = deliveredNotifications.Where(u => $"{u.Request.Content.UserInfo[NotificationIdKey]}".Equals($"{id}")).Select(s => s.Request.Identifier).ToArray();
                if (deliveredNotificationsMatches.Length > 0)
                {
                    UNUserNotificationCenter.Current.RemoveDeliveredNotifications(deliveredNotificationsMatches);

                }
            }
            else
            {
                var scheduledNotifications = UIApplication.SharedApplication.ScheduledLocalNotifications.Where(u => u.UserInfo[NotificationIdKey].Equals($"{id}"));
                //  var notification = notifications.Where(n => n.UserInfo.ContainsKey(NSObject.FromObject(NotificationKey)))  
                //         .FirstOrDefault(n => n.UserInfo[NotificationKey].Equals(NSObject.FromObject(id)));
                foreach (var notification in scheduledNotifications)
                {
                    UIApplication.SharedApplication.CancelLocalNotification(notification);
                }

            }
        }

        public async Task<string> GetTokenAsync()
        {
            var result = await InstanceId.SharedInstance.GetInstanceIdAsync();
            return result?.Token;
        }
    }


    public enum FirebaseTokenType
    {
        Sandbox,
        Production
    }
}
