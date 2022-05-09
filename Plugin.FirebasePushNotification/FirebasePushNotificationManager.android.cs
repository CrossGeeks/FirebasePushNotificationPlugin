using System;
using Firebase.Messaging;
using System.Collections.Generic;
using Android.Content;
using Android.OS;
using System.Collections.ObjectModel;
using System.Linq;
using Android.Gms.Tasks;
using System.Threading;
using Android.App;
using Android.Content.PM;
using Android.Graphics;
using Android.Media;
using System.Threading.Tasks;

namespace Plugin.FirebasePushNotification
{
    /// <summary>
    /// Implementation for Feature
    /// </summary>
    public class FirebasePushNotificationManager : Java.Lang.Object, IFirebasePushNotification, IOnCompleteListener
    {
        private static NotificationResponse delayedNotificationResponse = null;
        private static TaskCompletionSource<string> _tokenTcs;
        internal const string KeyGroupName = "Plugin.FirebasePushNotification";
        internal const string FirebaseTopicsKey = "FirebaseTopicsKey";
        internal const string FirebaseTokenKey = "FirebaseTokenKey";
        internal const string AppVersionCodeKey = "AppVersionCodeKey";
        internal const string AppVersionNameKey = "AppVersionNameKey";
        internal const string AppVersionPackageNameKey = "AppVersionPackageNameKey";

        // internal const string NotificationDeletedActionId = "Plugin.PushNotification.NotificationDeletedActionId";
        private static ICollection<string> currentTopics = new HashSet<string>(Android.App.Application.Context.GetSharedPreferences(KeyGroupName, FileCreationMode.Private).GetStringSet(FirebaseTopicsKey, new Collection<string>()));
        private static IList<NotificationUserCategory> userNotificationCategories = new List<NotificationUserCategory>();
        public static string NotificationContentTitleKey { get; set; }
        public static string NotificationContentTextKey { get; set; }
        public static string NotificationContentDataKey { get; set; }
        public static int IconResource { get; set; }
        public static int LargeIconResource { get; set; }
        public static bool ShouldShowWhen { get; set; } = true;
        public static Android.Net.Uri SoundUri { get; set; }
        public static Color? Color { get; set; }
        public static Type NotificationActivityType { get; set; }
        public static ActivityFlags? NotificationActivityFlags { get; set; } = ActivityFlags.ClearTop | ActivityFlags.SingleTop;

        public static string DefaultNotificationChannelId { get; set; } = "FirebasePushNotificationChannel";
        public static string DefaultNotificationChannelName { get; set; } = "General";
        public static NotificationImportance DefaultNotificationChannelImportance { get; set; } = NotificationImportance.Default;

        internal static Type DefaultNotificationActivityType { get; set; } = null;

        //internal static PushNotificationActionReceiver ActionReceiver = new PushNotificationActionReceiver();

        public static void ProcessIntent(Activity activity, Intent intent, bool enableDelayedResponse = true)
        {
            DefaultNotificationActivityType = activity.GetType();
            var extras = intent?.Extras;
            if (extras != null && !extras.IsEmpty)
            {
                var parameters = new Dictionary<string, object>();
                foreach (var key in extras.KeySet())
                {
                    if (!parameters.ContainsKey(key) && extras.Get(key) != null)
                    {
                        parameters.Add(key, $"{extras.Get(key)}");
                    }
                }

                if (parameters.Count > 0)
                {
                    var manager = Application.Context.GetSystemService(Context.NotificationService) as NotificationManager;
                    var notificationId = extras.GetInt(DefaultPushNotificationHandler.ActionNotificationIdKey, -1);
                    if (notificationId != -1)
                    {
                        var notificationTag = extras.GetString(DefaultPushNotificationHandler.ActionNotificationTagKey, string.Empty);
                        if (notificationTag == null)
                        {
                            manager.Cancel(notificationId);
                        }
                        else
                        {
                            manager.Cancel(notificationTag, notificationId);
                        }
                    }


                    var response = new NotificationResponse(parameters, extras.GetString(DefaultPushNotificationHandler.ActionIdentifierKey, string.Empty));


                    if (string.IsNullOrEmpty(response.Identifier))
                    {
                        if (_onNotificationOpened == null && enableDelayedResponse)
                        {
                            delayedNotificationResponse = response;
                        }
                        else
                        {
                            _onNotificationOpened?.Invoke(CrossFirebasePushNotification.Current, new FirebasePushNotificationResponseEventArgs(response.Data, response.Identifier, response.Type));
                        }
                    }
                    else
                    {
                        if (_onNotificationAction == null && enableDelayedResponse)
                        {
                            delayedNotificationResponse = response;
                        }
                        else
                        {
                            _onNotificationAction?.Invoke(CrossFirebasePushNotification.Current, new FirebasePushNotificationResponseEventArgs(response.Data, response.Identifier, response.Type));
                        }
                    }


                    CrossFirebasePushNotification.Current.NotificationHandler?.OnOpened(response);
                }

            }
        }
        public static void Initialize(Context context, bool resetToken, bool createDefaultNotificationChannel = true, bool autoRegistration = true)
        {
            CrossFirebasePushNotification.Current.NotificationHandler = CrossFirebasePushNotification.Current.NotificationHandler ?? new DefaultPushNotificationHandler();
            FirebaseMessaging.Instance.AutoInitEnabled = autoRegistration;
            if (autoRegistration)
            {
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


                        if (resetToken || (!string.IsNullOrEmpty(storedPackageName) && (!storedPackageName.Equals(packageName, StringComparison.CurrentCultureIgnoreCase) || !storedVersionName.Equals(versionName, StringComparison.CurrentCultureIgnoreCase) || !storedVersionCode.Equals($"{versionCode}", StringComparison.CurrentCultureIgnoreCase))))
                        {
                            CleanUp(false);

                        }

                    }
                    catch (Exception ex)
                    {
                        _onNotificationError?.Invoke(CrossFirebasePushNotification.Current, new FirebasePushNotificationErrorEventArgs(FirebasePushNotificationErrorType.UnregistrationFailed, ex.ToString()));
                    }
                    finally
                    {
                        var editor = prefs.Edit();
                        editor.PutString(FirebasePushNotificationManager.AppVersionNameKey, $"{versionName}");
                        editor.PutString(FirebasePushNotificationManager.AppVersionCodeKey, $"{versionCode}");
                        editor.PutString(FirebasePushNotificationManager.AppVersionPackageNameKey, $"{packageName}");
                        editor.Commit();
                    }


                    CrossFirebasePushNotification.Current.RegisterForPushNotifications();


                });
            }


            if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O && createDefaultNotificationChannel)
            {
                // Create channel to show notifications.
                var channelId = DefaultNotificationChannelId;
                var channelName = DefaultNotificationChannelName;
                var notificationManager = (NotificationManager)context.GetSystemService(Context.NotificationService);

                Android.Net.Uri defaultSoundUri = SoundUri != null ? SoundUri : RingtoneManager.GetDefaultUri(RingtoneType.Notification);
                AudioAttributes attributes = new AudioAttributes.Builder()
                    .SetUsage(AudioUsageKind.Notification)
                    .SetContentType(AudioContentType.Sonification)
                    .SetLegacyStreamType(Stream.Notification)
                    .Build();

                NotificationChannel notificationChannel = new NotificationChannel(channelId, channelName, DefaultNotificationChannelImportance);
                notificationChannel.EnableLights(true);
                notificationChannel.SetSound(defaultSoundUri, attributes);

                notificationManager.CreateNotificationChannel(notificationChannel);
            }

            System.Diagnostics.Debug.WriteLine(CrossFirebasePushNotification.Current.Token);
        }
        public static void Initialize(Context context, NotificationUserCategory[] notificationCategories, bool resetToken, bool createDefaultNotificationChannel = true, bool autoRegistration = true)
        {

            Initialize(context, resetToken, createDefaultNotificationChannel, autoRegistration);
            RegisterUserNotificationCategories(notificationCategories);

        }
    
        public static void Reset()
        {
            ThreadPool.QueueUserWorkItem(state =>
            {
                try
                {
                    CleanUp();
                }
                catch (Exception ex)
                {
                    _onNotificationError?.Invoke(CrossFirebasePushNotification.Current, new FirebasePushNotificationErrorEventArgs(FirebasePushNotificationErrorType.UnregistrationFailed, ex.ToString()));
                }
            });
        }

        public void RegisterForPushNotifications()
        {
            FirebaseMessaging.Instance.AutoInitEnabled = true;
            System.Threading.Tasks.Task.Run(async () =>
            {
                var token = await GetTokenAsync();
                if (!string.IsNullOrEmpty(token))
                {

                    SaveToken(token);
                }
            });

        }

        public async Task<string> GetTokenAsync()
        {
            _tokenTcs = new TaskCompletionSource<string>();
            FirebaseMessaging.Instance.GetToken().AddOnCompleteListener(this);

            string retVal = null;

            try
            {
                retVal = await _tokenTcs.Task;
            }
            catch (Exception ex)
            {
                _onNotificationError?.Invoke(CrossFirebasePushNotification.Current, new FirebasePushNotificationErrorEventArgs(FirebasePushNotificationErrorType.RegistrationFailed, $"{ex}"));
            }

            return retVal;


        }

        public void OnComplete(Android.Gms.Tasks.Task task)
        {

            try
            {
                if (task.IsSuccessful)
                {
                    var token = task.Result.ToString();
                    _tokenTcs?.TrySetResult(token);
                }
                else
                {
                    _tokenTcs?.TrySetException(task.Exception);
                }

            }
            catch (Exception ex)
            {
                _tokenTcs?.TrySetException(ex);
            }
        }

        public void UnregisterForPushNotifications()
        {
            FirebaseMessaging.Instance.AutoInitEnabled = false;
            Reset();
        }

        private static void CleanUp(bool clearAll = true)
        {
            if (clearAll)
            {
                CrossFirebasePushNotification.Current.UnsubscribeAll();
            }

            FirebaseMessaging.Instance.DeleteToken();
            SaveToken(string.Empty);
        }


        public static void Initialize(Context context, IPushNotificationHandler pushNotificationHandler, bool resetToken, bool createDefaultNotificationChannel = true, bool autoRegistration = true)
        {
            CrossFirebasePushNotification.Current.NotificationHandler = pushNotificationHandler;
            Initialize(context, resetToken, createDefaultNotificationChannel, autoRegistration);
        }

        public static void ClearUserNotificationCategories()
        {
            userNotificationCategories.Clear();
        }

        public string Token { get { return Android.App.Application.Context.GetSharedPreferences(KeyGroupName, FileCreationMode.Private).GetString(FirebaseTokenKey, string.Empty); } }

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


        public IPushNotificationHandler NotificationHandler { get; set; }

        public string[] SubscribedTopics
        {
            get
            {
                IList<string> topics = new List<string>();

                foreach (var t in currentTopics)
                {

                    topics.Add(t);
                }

                return topics.ToArray();
            }
        }

        private static FirebasePushNotificationResponseEventHandler _onNotificationOpened;
        public event FirebasePushNotificationResponseEventHandler OnNotificationOpened
        {
            add
            {
                var previousVal = _onNotificationOpened;
                _onNotificationOpened += value;
                if (delayedNotificationResponse != null && previousVal == null)
                {
                    var tmpParams = delayedNotificationResponse;
                    if (string.IsNullOrEmpty(tmpParams.Identifier))
                    {
                        _onNotificationOpened?.Invoke(CrossFirebasePushNotification.Current, new FirebasePushNotificationResponseEventArgs(tmpParams.Data, tmpParams.Identifier, tmpParams.Type));
                        delayedNotificationResponse = null;
                    }

                }

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
                var previousVal = _onNotificationAction;
                _onNotificationAction += value;
                if (delayedNotificationResponse != null && previousVal == null)
                {
                    var tmpParams = delayedNotificationResponse;
                    if (!string.IsNullOrEmpty(tmpParams.Identifier))
                    {
                        _onNotificationAction?.Invoke(CrossFirebasePushNotification.Current, new FirebasePushNotificationResponseEventArgs(tmpParams.Data, tmpParams.Identifier, tmpParams.Type));
                        delayedNotificationResponse = null;
                    }

                }
            }
            remove
            {
                _onNotificationAction -= value;
            }
        }

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


        public void SendDeviceGroupMessage(IDictionary<string, string> parameters, string groupKey, string messageId, int timeOfLive)
        {
            var message = new RemoteMessage.Builder(groupKey);
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
                if (currentTopics.Contains(t))
                {
                    FirebaseMessaging.Instance.UnsubscribeFromTopic(t);
                }
            }

            currentTopics.Clear();

            var editor = Android.App.Application.Context.GetSharedPreferences(KeyGroupName, FileCreationMode.Private).Edit();
            editor.PutStringSet(FirebaseTopicsKey, currentTopics);
            editor.Commit();
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
            SaveToken(token);
            _onTokenRefresh?.Invoke(CrossFirebasePushNotification.Current, new FirebasePushNotificationTokenEventArgs(token));
        }
        internal static void RegisterData(IDictionary<string, object> data)
        {
            _onNotificationReceived?.Invoke(CrossFirebasePushNotification.Current, new FirebasePushNotificationDataEventArgs(data));
        }
        internal static void RegisterAction(IDictionary<string, object> data, string identifier = "", NotificationCategoryType type = NotificationCategoryType.Default)
        {
            var response = new NotificationResponse(data, data.ContainsKey(DefaultPushNotificationHandler.ActionIdentifierKey) ? $"{data[DefaultPushNotificationHandler.ActionIdentifierKey]}" : string.Empty);

            _onNotificationAction?.Invoke(CrossFirebasePushNotification.Current, new FirebasePushNotificationResponseEventArgs(response.Data, response.Identifier, response.Type));
        }
        internal static void RegisterDelete(IDictionary<string, object> data)
        {
            _onNotificationDeleted?.Invoke(CrossFirebasePushNotification.Current, new FirebasePushNotificationDataEventArgs(data));
        }
        internal static void SaveToken(string token)
        {
            var editor = Android.App.Application.Context.GetSharedPreferences(FirebasePushNotificationManager.KeyGroupName, FileCreationMode.Private).Edit();
            editor.PutString(FirebasePushNotificationManager.FirebaseTokenKey, token);
            editor.Commit();
        }

        #endregion

        public void ClearAllNotifications()
        {
            var manager = Application.Context.GetSystemService(Context.NotificationService) as NotificationManager;
            manager.CancelAll();
        }

        public void RemoveNotification(int id)
        {
            var manager = Application.Context.GetSystemService(Context.NotificationService) as NotificationManager;
            manager.Cancel(id);
        }

        public void RemoveNotification(string tag, int id)
        {
            if (string.IsNullOrEmpty(tag))
            {
                RemoveNotification(id);
            }
            else
            {
                var manager = Application.Context.GetSystemService(Context.NotificationService) as NotificationManager;
                manager.Cancel(tag, id);
            }

        }
    }
}
