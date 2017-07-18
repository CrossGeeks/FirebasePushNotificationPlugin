# Getting Started

## Setup
* NuGet: [Plugin.FirebasePushNotification](http://www.nuget.org/packages/Plugin.FirebasePushNotification) [![NuGet](https://img.shields.io/nuget/v/Plugin.FirebasePushNotification.svg?label=NuGet)](https://www.nuget.org/packages/Plugin.FirebasePushNotification/)
* `PM> Install-Package Plugin.FirebasePushNotification`
* Install into ALL of your projects, include client projects.

## Using FirebasePushNotification APIs
It is drop dead simple to gain access to the FirebasePushNotification APIs in any project. All you need to do is get a reference to the current instance of IFirebasePushNotification via `CrossFirebasePushNotification.Current`:

## Initialize

### Android Initialization

On MainApplication OnCreate

```csharp
 FirebasePushNotificationManager.Initialize(this,new NotificationUserCategory[] {
                new NotificationUserCategory("message",new List<NotificationUserAction> {
                    new NotificationUserAction("Reply","Reply",NotificationActionType.Foreground),
                    new NotificationUserAction("Forward","Forward",NotificationActionType.Foreground)

                }),
                new NotificationUserCategory("request",new List<NotificationUserAction> {
                    new NotificationUserAction("Accept","Accept"),
                    new NotificationUserAction("Reject","Reject")
                })

  });

  //Handle notification when app is closed here
  CrossFirebasePushNotification.Current.OnNotificationReceived += (s,p) =>
  {

                
  };

```

On your main launcher activity OnCreate method

```csharp
 FirebasePushNotificationManager.ProcessIntent(Intent);
 ```

 Note: When using Xamarin Forms do it just after LoadApplication call.

### iOS Initialization

On AppDelegate FinishedLaunching
```csharp

        FirebasePushNotificationManager.Initialize(options, new NotificationUserCategory[]
        {
                new NotificationUserCategory("message",new List<NotificationUserAction> {
                    new NotificationUserAction("Reply","Reply",NotificationActionType.Foreground)
                }),
                new NotificationUserCategory("request",new List<NotificationUserAction> {
                    new NotificationUserAction("Accept","Accept"),
                    new NotificationUserAction("Reject","Reject",NotificationActionType.Destructive)
                })

        });

```
 Note: When using Xamarin Forms do it just after LoadApplication call.

```csharp
        public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
        {
            #if DEBUG
                    FirebasePushNotificationManager.DidRegisterRemoteNotifications(deviceToken, FirebaseTokenType.Sandbox);
            #endif
            #if RELEASE
                    FirebasePushNotificationManager.DidRegisterRemoteNotifications(deviceToken,FirebaseTokenType.Production);
            #endif

        }

        public override void FailedToRegisterForRemoteNotifications(UIApplication application, NSError error)
        {
            base.FailedToRegisterForRemoteNotifications(application, error);
            FirebasePushNotificationManager.RemoteNotificationRegistrationFailed(error);

        }
        // To receive notifications in foregroung on iOS 9 and below.
        // To receive notifications in background in any iOS version
        public override void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
        {
            // If you are receiving a notification message while your app is in the background,
            // this callback will not be fired 'till the user taps on the notification launching the application.

            // If you disable method swizzling, you'll need to call this method. 
            // This lets FCM track message delivery and analytics, which is performed
            // automatically with method swizzling enabled.
            FirebasePushNotificationManager.DidReceiveMessage(userInfo);
            // Do your magic to handle the notification data
            System.Console.WriteLine(userInfo);
        }

        public override void OnActivated(UIApplication uiApplication)
        {
            FirebasePushNotificationManager.Connect();
            base.OnActivated(uiApplication);
           
        }
        public override void DidEnterBackground(UIApplication application)
        {
            // Use this method to release shared resources, save user data, invalidate timers and store the application state.
            // If your application supports background exection this method is called instead of WillTerminate when the user quits.
            FirebasePushNotificationManager.Disconnect();
        }
```

Enable background modes Remote notifications

#### Events in FirebasePushNotification

Once token is refreshed you will get the event here:
```csharp
   /// <summary>
   /// Event triggered when token is refreshed
   /// </summary>
    event FirebasePushNotificationTokenEventHandler OnTokenRefresh;
```

```csharp        
  /// <summary>
  /// Event triggered when a notification is received
  /// </summary>
  event FirebasePushNotificationResponseEventHandler OnNotificationReceived;
```


```csharp        
  /// <summary>
  /// Event triggered when a notification is opened
  /// </summary>
  event FirebasePushNotificationResponseEventHandler OnNotificationOpened;
```

```csharp        
  /// <summary>
  /// Event triggered when there's an error
  /// </summary>
  event FirebasePushNotificationErrorEventHandler OnNotificationError;
```


<= Back to [Table of Contents](README.md)
