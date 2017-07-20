## Firebase Notifications

Message types

With FCM, you can send two types of messages to clients:

Notification messages - Delivered when the application is in background. These messages trigger the onMessageReceived() callback only when your app is in foreground.

```json
{
   "notification" : 
   {
    "body" : "hello!",
    "title": "afruz",
    "sound": "default"
   }
    "registration_ids" : ["eoPr8fUIPns:APA91bEVYODiWaL9a9JidumLRcFjVrEC4iHY80mHSE1e-udmPB32RjMiYL2P2vWTIUgYCJYVOx9SGSN_R4Ksau3vFX4WvkDj4ZstxqmcBhM3K-NMrX8P6U0sdDEqDpr-lD_N5JWBLCoV"]
}
```

Data messages - Handled by the client app. These messages trigger the onMessageReceived() callback even if your app is in foreground/background/killed. When using this type of message you are the one providing the UI and handling when push notification is received on an Android device.

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
For more information: 

https://firebase.google.com/docs/cloud-messaging/concept-options

https://firebase.google.com/docs/cloud-messaging/http-server-ref

### Subscribing/Unsubscribing topics

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

### Push Notification Handler

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

Also need to initialize with passing this implementation when initializing:

Initialize using a PushHandler on Application class on Android and AppDelegate on iOS:

Application class onAndroid:

```csharp
    #if DEBUG
      FirebasePushNotificationManager.Initialize(this,true,new CustomPushHandler());
    #else
      FirebasePushNotificationManager.Initialize(this,false,new CustomPushHandler());
    #endif
```

AppDelegate on iOS:
```csharp
      FirebasePushNotificationManager.Initialize(options,true,new CustomPushHandler());
```

After this you should receive push notifications events in this implementation on your iOS/Android projects.

