using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Graphics;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Media;
using Android.Support.V4.App;
using System.Collections.ObjectModel;
using Android.Content.PM;
using Android.Content.Res;
using AndroidX.Core.App;
using Java.Util;
using static Android.App.ActivityManager;

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
        /// Notification Category
        /// </summary>
        public const string NotificationCategoryKey = "notification_category";

        /// <summary>
        /// Full Intent
        /// </summary>
        public const string UseFullIntentKey = "use_full_intent";

        /// <summary>
        /// OnGoing
        /// </summary>
        public const string OnGoingKey = "ongoing";

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
        /// ActionIdentifier
        /// </summary>
        public const string ActionIdentifierKey = "action_identifier";

        /// <summary>
        /// Color
        /// </summary>
        public const string ColorKey = "color";

        /// <summary>
        /// Icon
        /// </summary>
        public const string IconKey = "icon";

        /// <summary>
        /// Large Icon
        /// </summary>
        public const string LargeIconKey = "large_icon";

        /// <summary>
        /// Sound
        /// </summary>
        public const string SoundKey = "sound";


        /// <summary>
        /// Priority
        /// </summary>
        public const string PriorityKey = "priority";

        /// <summary>
        /// Channel id
        /// </summary>
        public const string ChannelIdKey = "channel_id";

        /// <summary>
        /// Show Timestamp Key
        /// </summary>
        public const string ShowWhenKey = "show_when";

        public virtual void OnOpened(NotificationResponse response)
        {
            System.Diagnostics.Debug.WriteLine($"{DomainTag} - OnOpened");
        }

        public virtual void OnReceived(IDictionary<string, object> parameters)
        {
            System.Diagnostics.Debug.WriteLine($"{DomainTag} - OnReceived");

            if ((parameters.TryGetValue(SilentKey, out var silent) && (silent.ToString() == "true" || silent.ToString() == "1")) || (IsInForeground() && (!(!parameters.ContainsKey(ChannelIdKey) && parameters.TryGetValue(PriorityKey, out var imp) && ($"{imp}" == "high" || $"{imp}" == "max")) || (!parameters.ContainsKey(PriorityKey) && !parameters.ContainsKey(ChannelIdKey) && FirebasePushNotificationManager.DefaultNotificationChannelImportance != NotificationImportance.High && FirebasePushNotificationManager.DefaultNotificationChannelImportance != NotificationImportance.Max))))
            {
                return;
            }

            var context = Application.Context;

            var notifyId = 0;
            var title = context.ApplicationInfo.LoadLabel(context.PackageManager);
            var message = string.Empty;
            var tag = string.Empty;
            var showWhenVisible = FirebasePushNotificationManager.ShouldShowWhen;
            var soundUri = FirebasePushNotificationManager.SoundUri;
            var largeIconResource = FirebasePushNotificationManager.LargeIconResource;
            var smallIconResource = FirebasePushNotificationManager.IconResource;
            var notificationColor = FirebasePushNotificationManager.Color;
            var chanId = FirebasePushNotificationManager.DefaultNotificationChannelId;

            if (!string.IsNullOrEmpty(FirebasePushNotificationManager.NotificationContentTextKey) && parameters.TryGetValue(FirebasePushNotificationManager.NotificationContentTextKey, out var notificationContentText))
            {
                message = notificationContentText.ToString();
            }
            else if (parameters.TryGetValue(AlertKey, out var alert))
            {
                message = $"{alert}";
            }
            else if (parameters.TryGetValue(BodyKey, out var body))
            {
                message = $"{body}";
            }
            else if (parameters.TryGetValue(MessageKey, out var messageContent))
            {
                message = $"{messageContent}";
            }
            else if (parameters.TryGetValue(SubtitleKey, out var subtitle))
            {
                message = $"{subtitle}";
            }
            else if (parameters.TryGetValue(TextKey, out var text))
            {
                message = $"{text}";
            }

            if (!string.IsNullOrEmpty(FirebasePushNotificationManager.NotificationContentTitleKey) && parameters.TryGetValue(FirebasePushNotificationManager.NotificationContentTitleKey, out var notificationContentTitle))
            {
                title = notificationContentTitle.ToString();
            }
            else if (parameters.TryGetValue(TitleKey, out var titleContent))
            {
                if (!string.IsNullOrEmpty(message))
                {
                    title = $"{titleContent}";
                }
                else
                {
                    message = $"{titleContent}";
                }
            }

            if (parameters.TryGetValue(IdKey, out var id))
            {
                try
                {
                    notifyId = Convert.ToInt32(id);
                }
                catch (Exception ex)
                {
                    // Keep the default value of zero for the notify_id, but log the conversion problem.
                    System.Diagnostics.Debug.WriteLine($"Failed to convert {id} to an integer {ex}");
                }
            }

            if (parameters.TryGetValue(ShowWhenKey, out var shouldShowWhen))
            {
                showWhenVisible = $"{shouldShowWhen}".ToLower() == "true";
            }

            if (parameters.TryGetValue(TagKey, out var tagContent))
            {
                tag = tagContent.ToString();
            }

            try
            {
                if (parameters.TryGetValue(SoundKey, out var sound))
                {
                    var soundName = sound.ToString();

                    var soundResId = context.Resources.GetIdentifier(soundName, "raw", context.PackageName);
                    if (soundResId == 0 && soundName.IndexOf(".") != -1)
                    {
                        soundName = soundName.Substring(0, soundName.LastIndexOf('.'));
                        soundResId = context.Resources.GetIdentifier(soundName, "raw", context.PackageName);
                    }

                    soundUri = new Android.Net.Uri.Builder()
                                .Scheme(ContentResolver.SchemeAndroidResource)
                                .Path($"{context.PackageName}/{soundResId}")
                                .Build();
                }
            }
            catch (Resources.NotFoundException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }

            if (soundUri == null)
            {
                soundUri = RingtoneManager.GetDefaultUri(RingtoneType.Notification);
            }

            try
            {
                if (parameters.TryGetValue(IconKey, out var icon) && icon != null)
                {
                    try
                    {
                        smallIconResource = context.Resources.GetIdentifier(icon.ToString(), "drawable", Application.Context.PackageName);
                        if (smallIconResource == 0)
                        {
                            smallIconResource = context.Resources.GetIdentifier($"{icon}", "mipmap", Application.Context.PackageName);
                        }
                    }
                    catch (Resources.NotFoundException ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex.ToString());
                    }
                }

                if (smallIconResource == 0)
                {
                    smallIconResource = context.ApplicationInfo.Icon;
                }
                else
                {
                    var name = context.Resources.GetResourceName(smallIconResource);
                    if (name == null)
                    {
                        smallIconResource = context.ApplicationInfo.Icon;
                    }
                }
            }
            catch (Resources.NotFoundException ex)
            {
                smallIconResource = context.ApplicationInfo.Icon;
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }

            try
            {
                if (parameters.TryGetValue(LargeIconKey, out var largeIcon) && largeIcon != null)
                {
                    largeIconResource = context.Resources.GetIdentifier($"{largeIcon}", "drawable", Application.Context.PackageName);
                    if (largeIconResource == 0)
                    {
                        largeIconResource = context.Resources.GetIdentifier($"{largeIcon}", "mipmap", Application.Context.PackageName);
                    }
                }

                if (largeIconResource > 0)
                {
                    var name = context.Resources.GetResourceName(largeIconResource);
                    if (name == null)
                    {
                        largeIconResource = 0;
                    }
                }
            }
            catch (Resources.NotFoundException ex)
            {
                largeIconResource = 0;
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }

            if (parameters.TryGetValue(ColorKey, out var color) && color != null)
            {
                try
                {
                    notificationColor = Color.ParseColor(color.ToString());
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"{DomainTag} - Failed to parse color {ex}");
                }
            }

            var resultIntent = typeof(Activity).IsAssignableFrom(FirebasePushNotificationManager.NotificationActivityType) ? new Intent(Application.Context, FirebasePushNotificationManager.NotificationActivityType) : (FirebasePushNotificationManager.DefaultNotificationActivityType == null ? context.PackageManager.GetLaunchIntentForPackage(context.PackageName) : new Intent(Application.Context, FirebasePushNotificationManager.DefaultNotificationActivityType));

            var extras = new Bundle();
            foreach (var p in parameters)
            {
                extras.PutString(p.Key, p.Value.ToString());
            }

            if (extras != null)
            {
                extras.PutInt(ActionNotificationIdKey, notifyId);
                extras.PutString(ActionNotificationTagKey, tag);
                resultIntent.PutExtras(extras);
            }

            if (FirebasePushNotificationManager.NotificationActivityFlags != null)
            {
                resultIntent.SetFlags(FirebasePushNotificationManager.NotificationActivityFlags.Value);
            }
            var requestCode = new Java.Util.Random().NextInt();

            var pendingIntent = PendingIntent.GetActivity(context, requestCode, resultIntent, PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable);

            if (parameters.TryGetValue(ChannelIdKey, out var channelId) && channelId != null)
            {
                chanId = $"{channelId}";
            }

            var notificationBuilder = new NotificationCompat.Builder(context, chanId)
                 .SetSmallIcon(smallIconResource)
                 .SetContentTitle(title)
                 .SetContentText(message)
                 .SetAutoCancel(true)
                 .SetWhen(Java.Lang.JavaSystem.CurrentTimeMillis())
                 .SetContentIntent(pendingIntent);

            if (Build.VERSION.SdkInt >= BuildVersionCodes.JellyBeanMr1)
            {
                notificationBuilder.SetShowWhen(showWhenVisible);
            }


            if (largeIconResource > 0)
            {
                var largeIconBitmap = BitmapFactory.DecodeResource(context.Resources, largeIconResource);
                notificationBuilder.SetLargeIcon(largeIconBitmap);
            }

            var deleteIntent = new Intent(context, typeof(PushNotificationDeletedReceiver));
            deleteIntent.PutExtras(extras);
            var pendingDeleteIntent = PendingIntent.GetBroadcast(context, requestCode, deleteIntent, PendingIntentFlags.CancelCurrent | PendingIntentFlags.Immutable);
            notificationBuilder.SetDeleteIntent(pendingDeleteIntent);

            if (Build.VERSION.SdkInt < Android.OS.BuildVersionCodes.O)
            {
                if (parameters.TryGetValue(PriorityKey, out var priority) && priority != null)
                {
                    var priorityValue = $"{priority}";
                    if (!string.IsNullOrEmpty(priorityValue))
                    {
                        switch (priorityValue.ToLower())
                        {
                            case "max":
                                notificationBuilder.SetPriority(NotificationCompat.PriorityMax);
                                notificationBuilder.SetVibrate(new long[] { 1000, 1000, 1000, 1000, 1000 });
                                break;
                            case "high":
                                notificationBuilder.SetPriority(NotificationCompat.PriorityHigh);
                                notificationBuilder.SetVibrate(new long[] { 1000, 1000, 1000, 1000, 1000 });
                                break;
                            case "default":
                                notificationBuilder.SetPriority(NotificationCompat.PriorityDefault);
                                notificationBuilder.SetVibrate(new long[] { 1000, 1000, 1000, 1000, 1000 });
                                break;
                            case "low":
                                notificationBuilder.SetPriority(NotificationCompat.PriorityLow);
                                break;
                            case "min":
                                notificationBuilder.SetPriority(NotificationCompat.PriorityMin);
                                break;
                            default:
                                notificationBuilder.SetPriority(NotificationCompat.PriorityDefault);
                                notificationBuilder.SetVibrate(new long[] { 1000, 1000, 1000, 1000, 1000 });
                                break;
                        }

                    }
                    else
                    {
                        notificationBuilder.SetVibrate(new long[] { 1000, 1000, 1000, 1000, 1000 });
                    }

                }
                else
                {
                    notificationBuilder.SetVibrate(new long[] { 1000, 1000, 1000, 1000, 1000 });
                }

                try
                {

                    notificationBuilder.SetSound(soundUri);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"{DomainTag} - Failed to set sound {ex}");
                }
            }



            // Try to resolve (and apply) localized parameters
            ResolveLocalizedParameters(notificationBuilder, parameters);

            if (notificationColor != null)
            {
                notificationBuilder.SetColor(notificationColor.Value);
            }

            if (Build.VERSION.SdkInt >= BuildVersionCodes.JellyBean)
            {
                // Using BigText notification style to support long message
                var style = new NotificationCompat.BigTextStyle();
                style.BigText(message);
                notificationBuilder.SetStyle(style);
            }

            var category = string.Empty;
            if (parameters.TryGetValue(CategoryKey, out var categoryContent))
            {
                category = categoryContent.ToString();
            }

            if (parameters.TryGetValue(ActionKey, out var actionContent))
            {
                category = actionContent.ToString();
            }

            var notificationCategories = CrossFirebasePushNotification.Current?.GetUserNotificationCategories();
            if (notificationCategories != null && notificationCategories.Length > 0)
            {
                foreach (var userCat in notificationCategories)
                {
                    if (userCat != null && userCat.Actions != null && userCat.Actions.Count > 0)
                    {
                        foreach (var action in userCat.Actions)
                        {
                            var aRequestCode = Guid.NewGuid().GetHashCode();

                            if (userCat.Category.Equals(category, StringComparison.CurrentCultureIgnoreCase))
                            {
                                Intent actionIntent = null;
                                PendingIntent pendingActionIntent = null;


                                if (action.Type == NotificationActionType.Foreground)
                                {
                                    actionIntent = typeof(Activity).IsAssignableFrom(FirebasePushNotificationManager.NotificationActivityType) ? new Intent(Application.Context, FirebasePushNotificationManager.NotificationActivityType) : (FirebasePushNotificationManager.DefaultNotificationActivityType == null ? context.PackageManager.GetLaunchIntentForPackage(context.PackageName) : new Intent(Application.Context, FirebasePushNotificationManager.DefaultNotificationActivityType));

                                    if (FirebasePushNotificationManager.NotificationActivityFlags != null)
                                    {
                                        actionIntent.SetFlags(FirebasePushNotificationManager.NotificationActivityFlags.Value);
                                    }

                                    extras.PutString(ActionIdentifierKey, action.Id);
                                    actionIntent.PutExtras(extras);
                                    pendingActionIntent = PendingIntent.GetActivity(context, aRequestCode, actionIntent, PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable);

                                }
                                else
                                {
                                    actionIntent = new Intent(context, typeof(PushNotificationActionReceiver));
                                    extras.PutString(ActionIdentifierKey, action.Id);
                                    actionIntent.PutExtras(extras);
                                    pendingActionIntent = PendingIntent.GetBroadcast(context, aRequestCode, actionIntent, PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable);

                                }

                                notificationBuilder.AddAction(new NotificationCompat.Action.Builder(context.Resources.GetIdentifier(action.Icon, "drawable", Application.Context.PackageName), action.Title, pendingActionIntent).Build());
                            }

                        }
                    }
                }


            }

            OnBuildNotification(notificationBuilder, parameters);

            var notificationManager = (NotificationManager)context.GetSystemService(Context.NotificationService);
            notificationManager.Notify(tag, notifyId, notificationBuilder.Build());



        }

        /// <summary>
        /// Resolves the localized parameters using the string resources, combining the key and the passed arguments of the notification.
        /// </summary>
        /// <param name="notificationBuilder">Notification builder.</param>
        /// <param name="parameters">Parameters.</param>
        private void ResolveLocalizedParameters(NotificationCompat.Builder notificationBuilder, IDictionary<string, object> parameters)
        {
            string getLocalizedString(string name, params string[] arguments)
            {
                var context = notificationBuilder.MContext;
                var resources = context.Resources;
                var identifier = resources.GetIdentifier(name, "string", context.PackageName);
                var sanitizedArgs = arguments?.Where(it => it != null).Select(it => new Java.Lang.String(it)).Cast<Java.Lang.Object>().ToArray();

                try { return resources.GetString(identifier, sanitizedArgs ?? new Java.Lang.Object[] { }); }
                catch (UnknownFormatConversionException ex)
                {
                    System.Diagnostics.Debug.WriteLine($"{DomainTag}.ResolveLocalizedParameters - Incorrect string arguments {ex}");
                    return null;
                }
            }

            // Resolve title localization
            if (parameters.TryGetValue("title_loc_key", out var titleKey))
            {
                parameters.TryGetValue("title_loc_args", out var titleArgs);

                var localizedTitle = getLocalizedString(titleKey.ToString(), titleArgs as string[]);
                if (localizedTitle != null)
                {
                    notificationBuilder.SetContentTitle(localizedTitle);
                }
            }

            // Resolve body localization

            if (parameters.TryGetValue("body_loc_key", out var bodyKey))
            {
                parameters.TryGetValue("body_loc_args", out var bodyArgs);

                var localizedBody = getLocalizedString(bodyKey.ToString(), bodyArgs as string[]);
                if (localizedBody != null)
                {
                    notificationBuilder.SetContentText(localizedBody);
                }
            }
        }

        public virtual void OnError(string error)
        {
            System.Diagnostics.Debug.WriteLine($"{DomainTag} - OnError - {error}");
        }

        /// <summary>
        /// Override to provide customization of the notification to build.
        /// </summary>
        /// <param name="notificationBuilder">Notification builder.</param>
        /// <param name="parameters">Notification parameters.</param>
        public virtual void OnBuildNotification(NotificationCompat.Builder notificationBuilder, IDictionary<string, object> parameters) { }

        private bool IsInForeground()
        {
            bool isInForeground;

            var myProcess = new RunningAppProcessInfo();
            ActivityManager.GetMyMemoryState(myProcess);
            isInForeground = myProcess.Importance == Android.App.Importance.Foreground;

            return isInForeground;
        }
    }
}
