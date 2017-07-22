## Firebase Push Notifications

On Firebase Cloud Messaging there are two types of messages that you can send to clients:

**Notification messages** - Delivered when the application is in background. These messages trigger the onMessageReceived() callback only when your app is in foreground. Firebase will provide the ui for the notification shown on Android device.

```json
{
   "notification" : 
   {
    "body" : "hello",
    "title": "firebase",
    "sound": "default"
   },
    "registration_ids" : ["eoPr8fUIPns:APA91bEVYODiWaL9a9JidumLRcFjVrEC4iHY80mHSE1e-udmPB32RjMiYL2P2vWTIUgYCJYVOx9SGSN_R4Ksau3vFX4WvkDj4ZstxqmcBhM3K-NMrX8P6U0sdDEqDpr-lD_N5JWBLCoV"]
}
```

**Data messages** - Handled by the client app. These messages trigger the onMessageReceived() callback even if your app is in foreground/background/killed. When using this type of message you are the one providing the UI and handling when push notification is received on an Android device.

```json
{
    "data": {
        "message" : "my_custom_value",
        "other_key" : true,
        "body":"test"
     },
     "priority": "high",
     "condition": "'general' in topics"
}
```
**Important:** 

iOS :

- Data message won't display a notification on your device, should use a notification message for that instead.
- For receiving push notifications on the background on should include content_available = true inside the notification key. 

Example:

If application on background receives push notification and display a notification.
```json
{
    "data": {
        "message" : "my_custom_value",
        "other_key" : true,
        "body":"test"
     },
     "notification": {
       "body" : "hello",
       "title": "firebase",
       "sound": "default",
        "content_available" : true
     },
     "priority": "high",
     "condition": "'general' in topics"
}
```

If application on background receives push notification doesn't display any notification.
```json
{
    "data": {
        "message" : "my_custom_value",
        "other_key" : true,
        "body":"test"
     },
     "notification": {
        "content_available" : true
     },
     "priority": "high",
     "condition": "'general' in topics"
}
```

If  "content_available" : true is not sent then you have to tap on the notification for it to be received on the application.

Android:

- Data messages let's you customize notifications ui while notification messages don't (ui is renderered by firebase)


For more information: 

https://firebase.google.com/docs/cloud-messaging/concept-options

https://firebase.google.com/docs/cloud-messaging/http-server-ref

### Subscribing/Unsubscribing topics

Firebase provide the ability to group devices by using topics. When you send push notifications to a topic only the devices subscribed to this topic will get the notification.

```csharp
//Subscribing to single topic
CrossFirebasePushNotification.Current.Subscribe("general");

//Subscribing to multiple topics
CrossFirebasePushNotification.Current.Subscribe(new string[]{"baseball","football"});

//Unsubscribing to single topic
CrossFirebasePushNotification.Current.Unsubscribe("general");

//Unsubscribing from multiple topics
CrossFirebasePushNotification.Current.Unsubscribe(new string[]{"food","music"});

//Unsubscribing from all topics
CrossFirebasePushNotification.Current.UnsubscribeAll();

```

Note: On iOS you don't need to set the topic as /topics/{topic name} that is already handled internally so topics are set equally as on Android just specify the topic name {topic name}.

### Notification Events

**OnNotificationReceived**
```csharp

  CrossFirebasePushNotification.Current.OnNotificationReceived += (s,p) =>
  {
 
        System.Diagnostics.Debug.WriteLine("Received");
    
  };

```

**OnNotificationOpened**
```csharp
  
  CrossFirebasePushNotification.Current.OnNotificationOpened += (s,p) =>
  {
                System.Diagnostics.Debug.WriteLine("Opened");
                foreach(var data in p.Data)
                {
                    System.Diagnostics.Debug.WriteLine($"{data.Key} : {data.Value}");
                }

                if(!string.IsNullOrEmpty(p.Identifier))
                {
                    System.Diagnostics.Debug.WriteLine($"ActionId: {p.Identifier}");
                }
             
 };
```

### Push Notification Handler

**Default Push Notification Handler**

On Android by default the plugin uses a default push notification handler to create the notification ui & actions support when sending **Data messages**.

* There are a few things you can configure in Android project using the following static properties from FirebasePushNotificationManager class:
    ```csharp
    //The sets the key associated with the value will be used to show the title for the notification
    public static string NotificationContentTitleKey { get; set; }
   
    //The sets the key associated with the value will be used to show the text for the notification
    public static string NotificationContentTextKey { get; set; }

    //The sets the resource id for the icon will be used for the notification
    public static int IconResource { get; set; }

    //The sets the sound  uri will be used for the notification
    public static Android.Net.Uri SoundUri { get; set; }

   ```
* If no NotificationContentTitleKey and NotificationContentTextKey is set, by default displays a notification looking for the key <i><b>title</b></i> to display notification title and <i><b>message</b></i> to display notification message. If <i><b>title</b></i>  key not present will use the application name.
* If you send a key called <i><b>silent</b></i> with value true it won't display a notification.
* For notification with actions will look for **action_click** key value as the match. More information here:  [Notification Actions](../NotificationActions.md)

**Custom Push Notification Handler**

You might want to customize your notifications or handle events on your native iOS and Android project. For that you can implement the following interface on your iOS/Android project:

```csharp
public interface IPushNotificationHandler
{
        //Method triggered when an error occurs
        void OnError(string error);
        //Method triggered when a notification is opened
        void OnOpened(NotificationResponse response);
        //Method triggered when a notification is received
        void OnReceived(IDictionary<string, string> parameters);
}
```

An example of a custom handler use is the [DefaultPushNotificationHandler](../src/Plugin.FirebasePushNotification.Android/DefaultPushNotificationHandler.cs) which is the plugin default implementation to render the push notification ui when sending data messages and supporting notification actions on Android.

### Initialize using a PushHandler on Application class on Android and AppDelegate on iOS:

Application class **OnCreate** on Android:

```csharp
    #if DEBUG
      FirebasePushNotificationManager.Initialize(this,true,new CustomPushHandler());
    #else
      FirebasePushNotificationManager.Initialize(this,false,new CustomPushHandler());
    #endif
```

AppDelegate **FinishLaunching** on iOS:
```csharp
      FirebasePushNotificationManager.Initialize(options,true,new CustomPushHandler());
```

After this you should receive push notifications events in this implementation on your iOS/Android projects.

<= Back to [Table of Contents](../README.md)

