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


