## Firebase Notifications

Message types

With FCM, you can send two types of messages to clients:

Notification messages - Delivered when the application is in background. These messages trigger the onMessageReceived() callback only when your app is in foreground

Data messages - Handled by the client app. Theses messages trigger the onMessageReceived() callback even if your app is in foreground/background/killed



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
        //Method triggered when notification user categories are requested to be setup
        NotificationUserCategory[] NotificationUserCategories { get; }
}
```

Also need to initialize with passing this implementation when initializing:

Initialize using a PushHandler on Application class on Android and AppDelegate on iOS:

```csharp

    FirebasePushNotificationManager.Initialize(this,new CustomPushHandler());

```

After this you should receive push notifications events in this implementation on your iOS/Android projects.

