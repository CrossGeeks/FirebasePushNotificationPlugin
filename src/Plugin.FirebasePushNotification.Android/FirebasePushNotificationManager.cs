using Plugin.FirebasePushNotification.Abstractions;
using System;
using Firebase.Iid;
using Firebase.Messaging;
using System.Collections.Generic;
using Android.Content;
using Android.OS;
using Firebase;
using System.Collections.ObjectModel;
using System.Linq;
using Android.Gms.Tasks;
using System.Threading;
using Android.App;
using Android.Content.PM;
using Android.Graphics;

namespace Plugin.FirebasePushNotification
{
    /// <summary>
    /// Implementation for Feature
    /// </summary>
    public class FirebasePushNotificationManager : IFirebasePushNotification
    {
        static NotificationResponse delayedNotificationResponse = null;
        internal const string KeyGroupName = "Plugin.FirebasePushNotification";
        internal const string FirebaseTopicsKey = "FirebaseTopicsKey";
        internal const string FirebaseTokenKey = "FirebaseTokenKey";
        internal const string AppVersionCodeKey = "AppVersionCodeKey";
        internal const string AppVersionNameKey = "AppVersionNameKey";
        internal const string AppVersionPackageNameKey = "AppVersionPackageNameKey";
        static ICollection<string> currentTopics = Android.App.Application.Context.GetSharedPreferences(KeyGroupName, FileCreationMode.Private).GetStringSet(FirebaseTopicsKey, new Collection<string>());
        static IList<NotificationUserCategory> userNotificationCategories = new List<NotificationUserCategory>();
        public static string NotificationContentTitleKey { get; set; }
        public static string NotificationContentTextKey { get; set; }
        public static string NotificationContentDataKey { get; set; }
        public static int IconResource { get; set; }
        public static Android.Net.Uri SoundUri { get; set; }
        public static Color? Color { get; set; }

        internal static PushNotificationActionReceiver ActionReceiver = null;
        static Context _context;
        public static void ProcessIntent(Intent intent, bool enableDelayedResponse = true)
        {
            Bundle extras = intent?.Extras;
            if (extras != null && !extras.IsEmpty)
            {
                var parameters = new Dictionary<string, object>();
                foreach (var key in extras.KeySet())
                {
                    if (!parameters.ContainsKey(key) && extras.Get(key) != null)
                        parameters.Add(key, $"{extras.Get(key)}");
                }

                NotificationManager manager = _context.GetSystemService(Context.NotificationService) as NotificationManager;
                var notificationId = extras.GetInt(DefaultPushNotificationHandler.ActionNotificationIdKey, -1);
                if (notificationId != -1)
                {
                    var notificationTag = extras.GetString(DefaultPushNotificationHandler.ActionNotificationTagKey, string.Empty);
                    if(notificationTag == null)
                        manager.Cancel(notificationId);
                    else
                        manager.Cancel(notificationTag,notificationId);
                }

            
                var response = new NotificationResponse(parameters, extras.GetString(DefaultPushNotificationHandler.ActionIdentifierKey,string.Empty));

                if (_onNotificationOpened == null && enableDelayedResponse)
                    delayedNotificationResponse = response;
                else
                    _onNotificationOpened?.Invoke(CrossFirebasePushNotification.Current, new FirebasePushNotificationResponseEventArgs(response.Data, response.Identifier, response.Type));
               
              
                CrossFirebasePushNotification.Current.NotificationHandler?.OnOpened(response);
            }
        }
        public static void Initialize(Context context, bool resetToken)
        {
            FirebaseApp.InitializeApp(context);
            
            _context = context;
            
            CrossFirebasePushNotification.Current.NotificationHandler = CrossFirebasePushNotification.Current.NotificationHandler ?? new DefaultPushNotificationHandler();

            ThreadPool.QueueUserWorkItem(state =>
            {

                var packageName = Application.Context.PackageManager.GetPackageInfo(Application.Context.PackageName, PackageInfoFlags.MetaData).PackageName;
                var versionCode = Application.Context.PackageManager.GetPackageInfo(Application.Context.PackageName, PackageInfoFlags.MetaData).VersionCode;
                var versionName = Application.Context.PackageManager.GetPackageInfo(Application.Context.PackageName, PackageInfoFlags.MetaData).VersionName;
                var prefs = Android.App.Application.Context.GetSharedPreferences(FirebasePushNotificationManager.KeyGroupName, FileCreationMode.Private);

                try
                {
                        
                        var storedVersionName = prefs.GetString(FirebasePushNotificationManager.AppVersionNameKey, string.Empty);
                        var storedVersionCode = prefs.GetString(FirebasePushNotificationManager.AppVersionCodeKey, string.Empty);
                        var storedPackageName = prefs.GetString(FirebasePushNotificationManager.AppVersionPackageNameKey, string.Empty);

                        
                        if (resetToken  || (!string.IsNullOrEmpty(storedPackageName) && (!storedPackageName.Equals(packageName, StringComparison.CurrentCultureIgnoreCase) || !storedVersionName.Equals(versionName, StringComparison.CurrentCultureIgnoreCase) || !storedVersionCode.Equals($"{versionCode}", StringComparison.CurrentCultureIgnoreCase))))
                        {
                            CleanUp();
                            
                        }

                }
                catch (Exception ex)
                {
                    _onNotificationError?.Invoke(CrossFirebasePushNotification.Current, new FirebasePushNotificationErrorEventArgs(ex.ToString()));
                }
                finally
                {
                    var editor = prefs.Edit();
                    editor.PutString(FirebasePushNotificationManager.AppVersionNameKey, $"{versionName}");
                    editor.PutString(FirebasePushNotificationManager.AppVersionCodeKey, $"{versionCode}");
                    editor.PutString(FirebasePushNotificationManager.AppVersionPackageNameKey, $"{packageName}");
                    editor.Commit();
                }


                var token = FirebaseInstanceId.Instance.Token;
                if (!string.IsNullOrEmpty(token))
                {

                    SaveToken(token);
                }


            });

            System.Diagnostics.Debug.WriteLine(CrossFirebasePushNotification.Current.Token);
        }
        public static void Initialize(Context context, NotificationUserCategory[] notificationCategories,bool resetToken)
        {

                Initialize(context,resetToken);
                RegisterUserNotificationCategories(notificationCategories);

        }
        public static void Reset()
        {
            try
            {
                ThreadPool.QueueUserWorkItem(state =>
                {
                    CleanUp();
                });
            }
            catch (Exception ex)
            {
                _onNotificationError?.Invoke(CrossFirebasePushNotification.Current, new FirebasePushNotificationErrorEventArgs(ex.ToString()));
            }

           
        }

        static void CleanUp()
        {
            CrossFirebasePushNotification.Current.UnsubscribeAll();
            FirebaseInstanceId.Instance.DeleteInstanceId();
            SaveToken(string.Empty);
        }


        public static void Initialize(Context context,IPushNotificationHandler pushNotificationHandler,bool resetToken)
        {
            CrossFirebasePushNotification.Current.NotificationHandler = pushNotificationHandler;
            Initialize(context,resetToken);
        }

        public static void ClearUserNotificationCategories()
        {
            userNotificationCategories.Clear();
            /*if (actionReceiver != null)
            {
                _context.UnregisterReceiver(actionReceiver);
            }*/
        }

        public string Token { get { return Android.App.Application.Context.GetSharedPreferences(KeyGroupName, FileCreationMode.Private).GetString(FirebaseTokenKey, string.Empty); } }

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
        

        public IPushNotificationHandler NotificationHandler { get; set; }

        public string[] SubscribedTopics { get
        {
                IList<string> topics = new List<string>();

                foreach (var t in currentTopics)
                {

                    topics.Add(t);
                }

                return topics.ToArray();
            }
        }
        static FirebasePushNotificationResponseEventHandler _onNotificationOpened;
        public event FirebasePushNotificationResponseEventHandler OnNotificationOpened
        {
            add
            {
                _onNotificationOpened += value;
                if(delayedNotificationResponse != null && _onNotificationOpened == null)
                {
                    var tmpParams = delayedNotificationResponse;
                    _onNotificationOpened?.Invoke(CrossFirebasePushNotification.Current,new FirebasePushNotificationResponseEventArgs(tmpParams.Data,tmpParams.Identifier,tmpParams.Type));
                    delayedNotificationResponse = null;
                }
            }
            remove
            {
                _onNotificationOpened -= value;
            }
        }

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


        public void SendDeviceGroupMessage(IDictionary<string, string> parameters, string groupKey, string messageId, int timeOfLive)
        {
            RemoteMessage.Builder message = new RemoteMessage.Builder(groupKey);
            message.SetData(parameters);
            message.SetMessageId(messageId);
            message.SetTtl(timeOfLive);
            FirebaseMessaging.Instance.Send(message.Build());
        }

        public NotificationUserCategory[] GetUserNotificationCategories()
        {
            return userNotificationCategories?.ToArray();
        }
        public static void RegisterUserNotificationCategories(NotificationUserCategory[] notificationCategories)
        {
            if (notificationCategories != null && notificationCategories.Length > 0)
            {
                ClearUserNotificationCategories();
                
                foreach (var userCat in notificationCategories)
                {
                    userNotificationCategories.Add(userCat);
                }
                
            }
            else
            {
                ClearUserNotificationCategories();
            }
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

            if (!currentTopics.Contains((topic)))
            {
                FirebaseMessaging.Instance.SubscribeToTopic(topic);
                currentTopics.Add(topic);
                var editor = Android.App.Application.Context.GetSharedPreferences(KeyGroupName, FileCreationMode.Private).Edit();
                editor.PutStringSet(FirebaseTopicsKey, currentTopics);
                editor.Commit();
            }
        }

        public void Unsubscribe(string[] topics)
        {
            foreach (var t in topics)
            {
                Unsubscribe(t);
            }
        }

        public void UnsubscribeAll()
        {
            foreach (var t in currentTopics)
            {
                Unsubscribe(t);
            }
        }

        public void Unsubscribe(string topic)
        {
            if (currentTopics.Contains(topic))
            {
                FirebaseMessaging.Instance.UnsubscribeFromTopic(topic);
                currentTopics.Remove(topic);

                var editor = Android.App.Application.Context.GetSharedPreferences(KeyGroupName, FileCreationMode.Private).Edit();
                editor.PutStringSet(FirebaseTopicsKey, currentTopics);
                editor.Commit();
            }
            
        }

        #region internal methods
        //Raises event for push notification token refresh
        internal static void RegisterToken(string token)
        {
            _onTokenRefresh?.Invoke(CrossFirebasePushNotification.Current, new FirebasePushNotificationTokenEventArgs(token));
        }
        internal static void RegisterData(IDictionary<string,object> data)
        {
            _onNotificationReceived?.Invoke(CrossFirebasePushNotification.Current, new FirebasePushNotificationDataEventArgs(data));
        }
        internal static void SaveToken(string token)
        {
            var editor = Android.App.Application.Context.GetSharedPreferences(FirebasePushNotificationManager.KeyGroupName, FileCreationMode.Private).Edit();
            editor.PutString(FirebasePushNotificationManager.FirebaseTokenKey, token);
            editor.Commit();
        }

        #endregion
    }
}