using Firebase.Analytics;
using Firebase.CloudMessaging;
using Firebase.InstanceID;
using Foundation;
using Plugin.FirebasePushNotification.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using UIKit;
using UserNotifications;
using System.Threading.Tasks;
using Firebase.Core;

namespace Plugin.FirebasePushNotification
{

    /// <summary>
    /// Implementation for FirebasePushNotification
    /// </summary>
    public class FirebasePushNotificationManager : NSObject, IFirebasePushNotification, IUNUserNotificationCenterDelegate, IMessagingDelegate
    {
        public static UNNotificationPresentationOptions CurrentNotificationPresentationOption { get; set; } = UNNotificationPresentationOptions.None;
        static NSObject messagingConnectionChangeNotificationToken;
        static Queue<Tuple<string, bool>> pendingTopics = new Queue<Tuple<string, bool>>();
        static bool connected = false;
        static NSString FirebaseTopicsKey = new NSString("FirebaseTopics");
        const string FirebaseTokenKey = "FirebaseToken";
        static NSMutableArray currentTopics = (NSUserDefaults.StandardUserDefaults.ValueForKey(FirebaseTopicsKey) as NSArray ?? new NSArray()).MutableCopy() as NSMutableArray;
        public string Token { get { return string.IsNullOrEmpty(Messaging.SharedInstance.FcmToken) ? (NSUserDefaults.StandardUserDefaults.StringForKey(FirebaseTokenKey) ?? string.Empty) : Messaging.SharedInstance.FcmToken; } }

        static IList<NotificationUserCategory> usernNotificationCategories = new List<NotificationUserCategory>();


        static FirebasePushNotificationTokenEventHandler _onTokenRefresh;
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

        static FirebasePushNotificationDataEventHandler _onNotificationDeleted;
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

        static FirebasePushNotificationErrorEventHandler _onNotificationError;
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

        static FirebasePushNotificationResponseEventHandler _onNotificationOpened;
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


        public NotificationUserCategory[] GetUserNotificationCategories()
        {
            return usernNotificationCategories?.ToArray();
        }


        static FirebasePushNotificationDataEventHandler _onNotificationReceived;
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


        public string[] SubscribedTopics{
            get {

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
        
        public static async Task Initialize(NSDictionary options, bool autoRegistration = true)
        {
            App.Configure();

            CrossFirebasePushNotification.Current.NotificationHandler = CrossFirebasePushNotification.Current.NotificationHandler ?? new DefaultPushNotificationHandler();

            if(autoRegistration)
            {
                await CrossFirebasePushNotification.Current.Register();
            }
      

            /*if (options != null && options.Keys != null && options.Keys.Count() != 0 && options.ContainsKey(new NSString("UIApplicationLaunchOptionsRemoteNotificationKey")))
            {
                NSDictionary data = options.ObjectForKey(new NSString("UIApplicationLaunchOptionsRemoteNotificationKey")) as NSDictionary;

                // CrossFirebasePushNotification.Current.OnNotificationOpened(GetParameters(data));

            }*/
           
        }
        public static async void Initialize(NSDictionary options, IPushNotificationHandler pushNotificationHandler, bool autoRegistration = true)
        {
            CrossFirebasePushNotification.Current.NotificationHandler = pushNotificationHandler;
            await Initialize(options, autoRegistration);
        }
        public static async void Initialize(NSDictionary options,NotificationUserCategory[] notificationUserCategories,bool autoRegistration = true)
        {

            await Initialize(options, autoRegistration);
            RegisterUserNotificationCategories(notificationUserCategories);

        }
        public static void RegisterUserNotificationCategories(NotificationUserCategory[] userCategories)
        {
            if (userCategories != null && userCategories.Length > 0)
            {
                usernNotificationCategories.Clear();
                IList<UNNotificationCategory> categories = new List<UNNotificationCategory>();
                foreach (var userCat in userCategories)
                {
                    IList<UNNotificationAction> actions = new List<UNNotificationAction>();

                    foreach(var action in userCat.Actions)
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
                    var notificationActions = actions.ToArray()?? new UNNotificationAction[]{ };
                    var intentIDs = new string[] { };
                    var categoryOptions = new UNNotificationCategoryOptions[] { };

                    var category = UNNotificationCategory.FromIdentifier(categoryID, notificationActions, intentIDs,userCat.Type == NotificationCategoryType.Dismiss? UNNotificationCategoryOptions.CustomDismissAction:UNNotificationCategoryOptions.None);

                    categories.Add(category);

                    usernNotificationCategories.Add(userCat);

                }

                // Register categories
                UNUserNotificationCenter.Current.SetNotificationCategories(new NSSet<UNNotificationCategory>(categories.ToArray()));
         
            }

        }
        public async Task Register()
        {
            TaskCompletionSource<bool> permisionTask = new TaskCompletionSource<bool>();

            // Register your app for remote notifications.
            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
            {
                    // iOS 10 or later
                    var authOptions = UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound;
                    UNUserNotificationCenter.Current.RequestAuthorization(authOptions, (granted, error) =>
                    {
                        if (error != null)
                            _onNotificationError?.Invoke(CrossFirebasePushNotification.Current, new FirebasePushNotificationErrorEventArgs(error.Description));
                        else
                            System.Diagnostics.Debug.WriteLine(granted);

                        permisionTask.SetResult(granted);
                    });


                // For iOS 10 display notification (sent via APNS)
                UNUserNotificationCenter.Current.Delegate = CrossFirebasePushNotification.Current as IUNUserNotificationCenterDelegate;

                // For iOS 10 data message (sent via FCM)
                Messaging.SharedInstance.Delegate = CrossFirebasePushNotification.Current as IMessagingDelegate;
            }
            else
            {
                    // iOS 9 or before
                    var allNotificationTypes = UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound;
                    var settings = UIUserNotificationSettings.GetSettingsForTypes(allNotificationTypes, null);
                    UIApplication.SharedApplication.RegisterUserNotificationSettings(settings);
                    permisionTask.SetResult(true);
            }


                var permissonGranted = await permisionTask.Task;

                if (!permissonGranted)
                {
                    _onNotificationError?.Invoke(CrossFirebasePushNotification.Current, new FirebasePushNotificationErrorEventArgs("Push notification permission not granted"));

                }
                else
                {
                    UIApplication.SharedApplication.RegisterForRemoteNotifications();
                }
        }
        /*public static async void Register()
        {
            TaskCompletionSource<bool> permisionTask = new TaskCompletionSource<bool>();

            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
            {
      
                    // iOS 10 or later
                    var authOptions = UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound;
                    UNUserNotificationCenter.Current.RequestAuthorization(authOptions, (granted, error) =>
                    {
                        if (error != null)
                            _onNotificationError?.Invoke(CrossFirebasePushNotification.Current, new FirebasePushNotificationErrorEventArgs(error.Description));
                        else
                            System.Diagnostics.Debug.WriteLine(granted);

                        permisionTask.SetResult(granted);
                    });

            }
            else
            {
                    // iOS 9 or before
                    var allNotificationTypes = UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound;
                    var settings = UIUserNotificationSettings.GetSettingsForTypes(allNotificationTypes, null);
                    UIApplication.SharedApplication.RegisterUserNotificationSettings(settings);
                    permisionTask.SetResult(true);
            }



       
            var permissonGranted = await permisionTask.Task;

            if (!permissonGranted)
            {
               _onNotificationError?.Invoke(CrossFirebasePushNotification.Current, new FirebasePushNotificationErrorEventArgs("Push notification permission not granted"));

            }
            else
            {
                UIApplication.SharedApplication.RegisterForRemoteNotifications();
            }

             
        }*/
        /*public static void Unregister()
        {
            if (connected)
            { 
              CrossFirebasePushNotification.Current.UnsubscribeAll();
              Disconnect();
            }
            UIApplication.SharedApplication.UnregisterForRemoteNotifications();
            InstanceId.SharedInstance.DeleteId((h) => { });
        }*/

        public void Unregister()
        {
            if (connected)
            {
                CrossFirebasePushNotification.Current.UnsubscribeAll();
                Disconnect();
            }
            UIApplication.SharedApplication.UnregisterForRemoteNotifications();
            InstanceId.SharedInstance.DeleteId((h) => { });
        }
        /// <summary>
        /// Connects to Firebase Cloud Messaging
        /// </summary>
        public static void Connect()
        {
            if (messagingConnectionChangeNotificationToken != null)
            {
                NSNotificationCenter.DefaultCenter.RemoveObserver(messagingConnectionChangeNotificationToken);
            }

            Messaging.SharedInstance.ShouldEstablishDirectChannel = true;

            messagingConnectionChangeNotificationToken = NSNotificationCenter.DefaultCenter.AddObserver(Messaging.ConnectionStateChangedNotification, OnMessagingDirectChannelStateChanged);
        }
        static void OnMessagingDirectChannelStateChanged(NSNotification notification)
        {
            if (Messaging.SharedInstance.IsDirectChannelEstablished)
            {
                connected = true;
                while (pendingTopics.Count > 0)
                {
                    var pTopic = pendingTopics.Dequeue();

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
           /*else
            {
                _onNotificationError?.Invoke(CrossFirebasePushNotification.Current, new FirebasePushNotificationErrorEventArgs("Connection couldn't be established"));
            }*/
        }
        public static void Disconnect()
        {
            // Use this method to release shared resources, save user data, invalidate timers and store the application state.
            // If your application supports background exection this method is called instead of WillTerminate when the user quits.
            //Messaging.SharedInstance.Disconnect();
            Messaging.SharedInstance.ShouldEstablishDirectChannel = false;
            connected = false;

            if (messagingConnectionChangeNotificationToken != null)
            {
                NSNotificationCenter.DefaultCenter.RemoveObserver(messagingConnectionChangeNotificationToken);
                messagingConnectionChangeNotificationToken = null;
            }
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

            completionHandler(CurrentNotificationPresentationOption);
        }

        public static void DidReceiveMessage(NSDictionary data)
        {
            Messaging.SharedInstance.AppDidReceiveMessage(data);
            var parameters = GetParameters(data);

            _onNotificationReceived?.Invoke(CrossFirebasePushNotification.Current,new FirebasePushNotificationDataEventArgs(parameters));

            CrossFirebasePushNotification.Current.NotificationHandler?.OnReceived(parameters);
            System.Diagnostics.Debug.WriteLine("DidReceivedMessage");
        }
        public static void DidRegisterRemoteNotifications(NSData deviceToken,FirebaseTokenType type)
        {
            Messaging.SharedInstance.ApnsToken = deviceToken;
        }

        public static void DidRegisterRemoteNotifications(NSData deviceToken)
        {

            Messaging.SharedInstance.ApnsToken = deviceToken;
        }

        public static void RemoteNotificationRegistrationFailed(NSError error)
        {
            _onNotificationError?.Invoke(CrossFirebasePushNotification.Current, new FirebasePushNotificationErrorEventArgs(error.Description));
        }

        public void ApplicationReceivedRemoteMessage(RemoteMessage remoteMessage)
        {
            System.Console.WriteLine(remoteMessage.AppData);
            System.Diagnostics.Debug.WriteLine("ApplicationReceivedRemoteMessage");
            var parameters = GetParameters(remoteMessage.AppData);
            _onNotificationReceived?.Invoke(CrossFirebasePushNotification.Current, new FirebasePushNotificationDataEventArgs(parameters));
            CrossFirebasePushNotification.Current.NotificationHandler?.OnReceived(parameters);
        }

        static IDictionary<string, object> GetParameters(NSDictionary data)
        {
            var parameters = new Dictionary<string, object>();

            var keyAps = new NSString("aps");
            var keyAlert = new NSString("alert");

            foreach (var val in data)
            {
                if (val.Key.Equals(keyAps))
                {
                    NSDictionary aps = data.ValueForKey(keyAps) as NSDictionary;

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
           foreach(var t in topics)
           {
               Subscribe(t);
           }
       }

       public void Subscribe(string topic)
       {
            if(!connected)
            {
                pendingTopics.Enqueue(new Tuple<string,bool>(topic,true));
                return;
            }
            
            if (!currentTopics.Contains(new NSString(topic)))
            {
                Messaging.SharedInstance.Subscribe($"/topics/{topic}");
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
            if (!connected)
            {
                pendingTopics.Enqueue(new Tuple<string, bool>(topic, false));
                return;
            }
            var deletedKey = new NSString($"{topic}");
            if (currentTopics.Contains(deletedKey))
            {
                Messaging.SharedInstance.Unsubscribe($"/topics/{topic}");
                nint idx = (nint)currentTopics.IndexOf(deletedKey);
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
            if (connected)
            {
                NSMutableDictionary message = new NSMutableDictionary();
                foreach (var p in parameters)
                {
                    message.Add(new NSString(p.Key), new NSString(p.Value));
                }

                Messaging.SharedInstance.SendMessage(message, groupKey, messageId, timeOfLive);
            }
        }

        [Export("userNotificationCenter:didReceiveNotificationResponse:withCompletionHandler:")]
        public void DidReceiveNotificationResponse(UNUserNotificationCenter center, UNNotificationResponse response, Action completionHandler)
        {

            var parameters = GetParameters(response.Notification.Request.Content.UserInfo);
       
            NotificationCategoryType catType = NotificationCategoryType.Default;
            if (response.IsCustomAction)
                catType = NotificationCategoryType.Custom;
            else if (response.IsDismissAction)
                catType = NotificationCategoryType.Dismiss;
     
            var notificationResponse = new NotificationResponse(parameters, $"{response.ActionIdentifier}".Equals("com.apple.UNNotificationDefaultActionIdentifier",StringComparison.CurrentCultureIgnoreCase)?string.Empty:$"{response.ActionIdentifier}",catType);
            _onNotificationOpened?.Invoke(this,new FirebasePushNotificationResponseEventArgs(notificationResponse.Data,notificationResponse.Identifier,notificationResponse.Type));
           
            CrossFirebasePushNotification.Current.NotificationHandler?.OnOpened(notificationResponse);
            
            // Inform caller it has been handled
            completionHandler();
        }

        public void DidRefreshRegistrationToken(Messaging messaging, string fcmToken)
        {
            // Note that this callback will be fired everytime a new token is generated, including the first
            // time. So if you need to retrieve the token as soon as it is available this is where that
            // should be done.
            var refreshedToken = fcmToken;
            if (!string.IsNullOrEmpty(refreshedToken))
            {
                _onTokenRefresh?.Invoke(CrossFirebasePushNotification.Current, new FirebasePushNotificationTokenEventArgs(refreshedToken));
                Connect();
            }

            NSUserDefaults.StandardUserDefaults.SetString(fcmToken, FirebaseTokenKey);
        }
    }

    public enum FirebaseTokenType
    {
        Sandbox,
        Production
    }
}