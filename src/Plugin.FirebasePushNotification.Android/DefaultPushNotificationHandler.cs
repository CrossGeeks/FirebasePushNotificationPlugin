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
using Plugin.FirebasePushNotification.Abstractions;
using Android.Media;
using Android.Support.V4.App;
using System.Collections.ObjectModel;
using Android.Content.PM;

namespace Plugin.FirebasePushNotification
{
    public class DefaultPushNotificationHandler : IPushNotificationHandler
    {
        public const string DomainTag = "DefaultPushNotificationHandler";
        /// <summary>
        /// Title
        /// </summary>
        public const string TitleKey = "title";
        /// <summary>
        /// Text
        /// </summary>
        public const string TextKey = "text";
        /// <summary>
        /// Subtitle
        /// </summary>
        public const string SubtitleKey = "subtitle";
        /// <summary>
        /// Message
        /// </summary>
        public const string MessageKey = "message";
        /// <summary>
        /// Message
        /// </summary>
        public const string BodyKey = "body";
        /// <summary>
        /// Alert
        /// </summary>
        public const string AlertKey = "alert";

        /// <summary>
        /// Id
        /// </summary>
        public const string IdKey = "id";

        /// <summary>
        /// Tag
        /// </summary>
        public const string TagKey = "tag";

        /// <summary>
        /// Action Click
        /// </summary>
        public const string ActionKey = "click_action";

        /// <summary>
        /// Category
        /// </summary>
        public const string CategoryKey = "category";

        /// <summary>
        /// Silent
        /// </summary>
        public const string SilentKey = "silent";

        /// <summary>
        /// ActionNotificationId
        /// </summary>
        public const string ActionNotificationIdKey = "action_notification_id";

        /// <summary>
        /// ActionNotificationTag
        /// </summary>
        public const string ActionNotificationTagKey = "action_notification_tag";

        /// <summary>
        /// ActionIdentifeir
        /// </summary>
        public const string ActionIdentifierKey = "action_identifier";


        public NotificationUserCategory[] NotificationUserCategories => new NotificationUserCategory[] { };

        public void OnOpened(NotificationResponse response)
        {
            System.Diagnostics.Debug.WriteLine($"{DomainTag} - OnOpened");
        }

        public void OnReceived(IDictionary<string, string> parameters)
        {

            System.Diagnostics.Debug.WriteLine($"{DomainTag} - OnReceived");

            if (parameters.ContainsKey(SilentKey) && (parameters[SilentKey] == "true" || parameters[SilentKey] == "1"))
            {
                return;
            }

            Context context = Android.App.Application.Context;

            int notifyId = 0;
            string title = context.ApplicationInfo.LoadLabel(context.PackageManager);
            string message = "";
            string tag = "";

            if (!string.IsNullOrEmpty(FirebasePushNotificationManager.NotificationContentTextKey) && parameters.ContainsKey(FirebasePushNotificationManager.NotificationContentTextKey))
            {
                message = parameters[FirebasePushNotificationManager.NotificationContentTextKey].ToString();
            }
            else if (parameters.ContainsKey(AlertKey))
            {
                message = $"{parameters[AlertKey]}";
            }
            else if (parameters.ContainsKey(BodyKey))
            {
                message = $"{parameters[BodyKey]}";
            }
            else if (parameters.ContainsKey(MessageKey))
            {
                message = $"{parameters[MessageKey]}";
            }
            else if (parameters.ContainsKey(SubtitleKey))
            {
                message = $"{parameters[SubtitleKey]}";
            }
            else if (parameters.ContainsKey(TextKey))
            {
                message = $"{parameters[TextKey]}";
            }

            if (!string.IsNullOrEmpty(FirebasePushNotificationManager.NotificationContentTitleKey) && parameters.ContainsKey(FirebasePushNotificationManager.NotificationContentTitleKey))
            {
                title = parameters[FirebasePushNotificationManager.NotificationContentTitleKey].ToString();

            }
            else if (parameters.ContainsKey(TitleKey))
            {

                if (!string.IsNullOrEmpty(message))
                {
                    title = $"{parameters[TitleKey]}";
                }
                else
                {
                    message = $"{parameters[TitleKey]}";
                }
            }


            
            if (parameters.ContainsKey(IdKey))
            {
                var str = parameters[IdKey].ToString();
                try
                {
                    notifyId = Convert.ToInt32(str);
                }
                catch (System.Exception ex)
                {
                    // Keep the default value of zero for the notify_id, but log the conversion problem.
                    System.Diagnostics.Debug.WriteLine("Failed to convert {0} to an integer", str);
                }
            }
            if (parameters.ContainsKey(TagKey))
            {
                tag = parameters[TagKey].ToString();
            }

            if (FirebasePushNotificationManager.SoundUri == null)
            {
                FirebasePushNotificationManager.SoundUri = RingtoneManager.GetDefaultUri(RingtoneType.Notification);
            }
            try
            {

                if (FirebasePushNotificationManager.IconResource == 0)
                {
                    FirebasePushNotificationManager.IconResource = context.ApplicationInfo.Icon;
                }
                else
                {
                    string name = context.Resources.GetResourceName(FirebasePushNotificationManager.IconResource);

                    if (name == null)
                    {
                        FirebasePushNotificationManager.IconResource = context.ApplicationInfo.Icon;

                    }
                }

            }
            catch (Android.Content.Res.Resources.NotFoundException ex)
            {
                FirebasePushNotificationManager.IconResource = context.ApplicationInfo.Icon;
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }


            Intent resultIntent = context.PackageManager.GetLaunchIntentForPackage(context.PackageName);

            //Intent resultIntent = new Intent(context, typeof(T));
            Bundle extras = new Bundle();
            foreach(var p in parameters)
            {
                extras.PutString(p.Key, p.Value);
            }
        
            if (extras != null)
            {
                extras.PutInt(ActionNotificationIdKey, notifyId);
                extras.PutString(ActionNotificationTagKey, tag);
                resultIntent.PutExtras(extras);
            }

            
             var pendingIntent = PendingIntent.GetActivity(context, 0, resultIntent, PendingIntentFlags.OneShot | PendingIntentFlags.UpdateCurrent);

             var notificationBuilder = new NotificationCompat.Builder(context)
                 .SetSmallIcon(FirebasePushNotificationManager.IconResource)
                 .SetContentTitle(title)
                 .SetSound(FirebasePushNotificationManager.SoundUri)
                 .SetVibrate(new long[] { 1000, 1000, 1000, 1000, 1000 })
                 .SetContentText(message)
                 .SetAutoCancel(true)
                 .SetContentIntent(pendingIntent);
             

            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.JellyBean)
            {
                // Using BigText notification style to support long message
                var style = new NotificationCompat.BigTextStyle();
                style.BigText(message);
                notificationBuilder.SetStyle(style);
            }
           
            string category = string.Empty;

            if (parameters.ContainsKey(CategoryKey))
            {
                category = parameters[CategoryKey];

            }

            if (parameters.ContainsKey(ActionKey))
            {
                category = parameters[ActionKey];
            }
            var notificationCategories = CrossFirebasePushNotification.Current?.GetUserNotificationCategories();
            if (notificationCategories != null && notificationCategories.Length > 0)
            {

                IntentFilter intentFilter = null;
                foreach (var userCat in notificationCategories)
                {
                    if (userCat != null && userCat.Actions != null && userCat.Actions.Count > 0)
                    {
                        
                        foreach (var action in userCat.Actions)
                        {
                            if (userCat.Category.Equals(category, StringComparison.CurrentCultureIgnoreCase))
                            {
                                Intent actionIntent = null;
                                PendingIntent pendingActionIntent = null;


                                if (action.Type == NotificationActionType.Foreground)
                                {
                                    actionIntent = context.PackageManager.GetLaunchIntentForPackage(context.PackageName);
                                    actionIntent.SetFlags( ActivityFlags.ClearTop | ActivityFlags.NewTask);
                                    actionIntent.SetAction($"{action.Id}");
                                    extras.PutString(ActionIdentifierKey, action.Id);
                                    actionIntent.PutExtras(extras);
                                    pendingActionIntent = PendingIntent.GetActivity(context, 0, actionIntent, PendingIntentFlags.OneShot | PendingIntentFlags.UpdateCurrent);

                                }
                                else
                                {
                                    actionIntent = new Intent();
                                    //actionIntent.SetAction($"{category}.{action.Id}");
                                    actionIntent.SetAction($"{Application.Context.PackageManager.GetPackageInfo(Application.Context.PackageName, PackageInfoFlags.MetaData).PackageName}.{action.Id}");
                                    extras.PutString(ActionIdentifierKey, action.Id);
                                    actionIntent.PutExtras(extras);
                                    pendingActionIntent = PendingIntent.GetBroadcast(context, 0, actionIntent, PendingIntentFlags.OneShot | PendingIntentFlags.UpdateCurrent);

                                }
                                
                                notificationBuilder.AddAction(FirebasePushNotificationManager.IconResource, action.Title, pendingActionIntent);
                            }


                            if (FirebasePushNotificationManager.ActionReceiver == null)
                            {
                                if(intentFilter == null)
                                {
                                    intentFilter = new IntentFilter();
                                }

                                if (!intentFilter.HasAction(action.Id))
                                {
                                    intentFilter.AddAction($"{Application.Context.PackageManager.GetPackageInfo(Application.Context.PackageName, PackageInfoFlags.MetaData).PackageName}.{action.Id}");
                                }
                                  
                            }
                               
                        }
                    }
                }
                if(intentFilter !=null)
                {

                    FirebasePushNotificationManager.ActionReceiver = new PushNotificationActionReceiver();
                    context.RegisterReceiver(FirebasePushNotificationManager.ActionReceiver, intentFilter);
                }
            }
    

            NotificationManager notificationManager = (NotificationManager)context.GetSystemService(Context.NotificationService);
            notificationManager.Notify(tag, notifyId, notificationBuilder.Build());
 
        }

        public void OnError(string error)
        {
            System.Diagnostics.Debug.WriteLine($"{DomainTag} - OnError - {error}");
        }

    }
}