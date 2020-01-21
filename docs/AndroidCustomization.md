## Android Specific Customization

You can set the activity to be launched when you tap on the notification on Android project by setting **FirebasePushNotificationManager.NotificationActivityType**

Usage sample:

```csharp
FirebasePushNotificationManager.NotificationActivityType = typeof(MainActivity);
```

**Note: Uses application main launcher activity if the above is not set.**

You can also set the flags for launching this activity with **FirebasePushNotificationManager.NotificationActivityFlags** by default is set to:

```csharp
FirebasePushNotificationManager.NotificationActivityFlags = ActivityFlags.ClearTop | ActivityFlags.SingleTop
```

### Static customization properties

If plugin is not initialized with a push handler on Android by default the plugin uses the default push notification handler to create the notification ui & actions support when sending **Data messages**.

By using the default push notification handler. There are a few things you can configure in Android project using the following static properties of **FirebasePushNotificationManager** class:

```csharp

    //Sets the key associated with the value will be look for in the notification payload to be used to show the title for the notification
    public static string NotificationContentTitleKey { get; set; }
   
    //Sets the key associated with the value will look for in the notification payload to be used to show the text for the notification 
    public static string NotificationContentTextKey { get; set; }

    //Sets the resource id for the icon will be used for the notification
    public static int IconResource { get; set; }

    //Sets the sound  uri will be used for the notification
    public static Android.Net.Uri SoundUri { get; set; }

    //Sets the color will be used for the notification
    public static Color? Color { get; set; }

    //Sets the default notification channel id for Android O
    public static string DefaultNotificationChannelId { get; set; } = "PushNotificationChannel";
    
    //Sets the default notification channel name for Android O
    public static string DefaultNotificationChannelName { get; set; } = "General";
    
    //Sets the default notification channel importance for Android O
    public static NotificationImportance DefaultNotificationChannelImportance { get; set; } = NotificationImportance.Default;

```

If **FirebasePushNotificationManager.IconResource** not set will use default application icon.

If **FirebasePushNotificationManager.SoundUri** not set will use the default notification ringtone.
   
If **NotificationContentTitleKey** not set will look for **title** key value in the notification payload to set the title. If no title key present will use the application name as the notification title.

If **NotificationContentTextKey** not set will look for one of the following keys value in the notification payload using the priority order shown below to set the message for the notification:
            
1. **alert**
2. **body**
3. **message**
4. **subtitle**
5. **text**
6. **title**

Once one of the above keys is found on the notification data message payload will show it's value as the notification message.


### Payload Keys 

There are also some keys you can set on the payload:

* **id** : Sets the notification id
* **tag** : Sets the notification tag
* **priority** : Sets the notification priority
* **sound** : Sets the notification sound
* **icon** : Sets the notification icon
* **click_action** : Sets name for notification action
* **channel_id** : Sets id for the notification channel that will be used when notification is delivered

If **sound** or **icon** keys present have priority over the **FirebasePushNotificationManager.SoundUri** and **FirebasePushNotificationManager.IconResource** static customization properties mentioned above.

#####  Notification Id

* **id** key is set as the notification id if present (integer value).


 Payload sample with id

```json
{
   "data" : {
     "title": "hello",
     "body": "firebase",
     "id": 1
  }
}
```

#####  Notification Tag

* **tag** key is set as the notification tag if present.

 Payload sample with id and tag

```json
{
   "data" : {
     "title": "hello",
     "body": "firebase",
     "id": 1,
     "tag" : "msg"

  }
}


```
#####  Notification without UI

* If you send a key called **silent** with value true it won't display a notification.

 Silent notification payload sample

```json
{
   "data" : {
     "title": "hello",
     "body": "firebase",
     "silent":"true"
  }
}
```

#####  Notification Priority

* Depending on the value of **priority** key in your data payload. It will set the notification priority. Posible values are: "max", "high","default","low","min".

The behaviour for these values:

**MAX** - Use for critical and urgent notifications that alert the user to a condition that is time-critical or needs to be resolved before they can continue with a particular task.

**HIGH** - Use primarily for important communication, such as message or chat events with content that is particularly interesting for the user. High-priority notifications trigger the heads-up notification display.

**DEFAULT** - Use for all notifications that don't fall into any of the other priorities described here and if the application does not prioritize its own notifications

**LOW** - Use for notifications that you want the user to be informed about, but that are less urgent. Low-priority notifications tend to show up at the bottom of the list, which makes them a good choice for things like public or undirected social updates: The user has asked to be notified about them, but these notifications should never take precedence over urgent or direct communication.

**MIN** - Use for contextual or background information such as weather information or contextual location information. Minimum-priority notifications do not appear in the status bar. The user discovers them on expanding the notification shade.

If no priority is set then "priority" is default.

For heads-up notification send inside your payload data key "priority" : "high" within your other keys:

Sample payload with priority

```json
{
   "data" : {
     "title": "hello",
     "body": "firebase",
     "priority":"high"
  }
}
```
#####  Notification Sound

* You can send sound by using **sound** key, a sound with the value set should be in your *Resources/raw* folder.

Payload sample with sound
```json
{
   "data" : {
     "title": "hello",
     "body": "firebase",
     "priority":"high",
     "sound":"test"
  }
}
```
If sound not set will set the **FirebasePushNotificationManager.SoundUri** value if not set either will use the default notification ringtone.

#####  Notification Icon

* You can send the icon to be displayed on the notification by using **icon** key, an icon with the value set should be in your *Resources/drawable* folder.

Payload sample with icon

```json
{
   "data" : {
     "title": "hello",
     "body": "firebase",
     "priority":"high",
     "icon":"test"
  }
}
```

Payload sample with icon and sound

```json
{
   "data" : {
     "title": "hello",
     "body": "firebase",
     "priority":"high",
     "icon":"test",
     "sound":"test"
  }
}
```

If icon not set will set the **FirebasePushNotificationManager.IconResource** value if not set either will use the default application icon.

#####  Notification Actions

* For notification with actions will look for **click_action** key value as the match. More information here:  [Notification Actions](NotificationActions.md)

#####  Notification Channel Id

* **channel_id** key is set as the notification channel id if present will use that specified notification channel for this notification.

 Payload sample with channel id

```json
{
   "data" : {
     "title": "hello",
     "body": "firebase",
     "channel_id" : "PushNotificationChannel"

  }
}
```

<= Back to [Table of Contents](../README.md)

