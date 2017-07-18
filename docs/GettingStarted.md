# Getting Started

## Setup
* NuGet: [Plugin.FirebasePushNotification](http://www.nuget.org/packages/Plugin.FirebasePushNotification) [![NuGet](https://img.shields.io/nuget/v/Plugin.FirebasePushNotification.svg?label=NuGet)](https://www.nuget.org/packages/Plugin.FirebasePushNotification/)
* `PM> Install-Package Plugin.FirebasePushNotification`
* Install into ALL of your projects, include client projects.

## Using FirebasePushNotification APIs
It is drop dead simple to gain access to the FirebasePushNotification APIs in any project. All you need to do is get a reference to the current instance of IFirebasePushNotification via `CrossFirebasePushNotification.Current`:

## Initialize

### Android Initialization

Edit AndroidManifest.xml (under Properties in the Solution Explorer) and insert the following **<receiver>** elements into the **<application>** section:

```xml
<receiver 
    android:name="com.google.firebase.iid.FirebaseInstanceIdInternalReceiver" 
    android:exported="false" />
<receiver 
    android:name="com.google.firebase.iid.FirebaseInstanceIdReceiver" 
    android:exported="true" 
    android:permission="com.google.android.c2dm.permission.SEND">
    <intent-filter>
        <action android:name="com.google.android.c2dm.intent.RECEIVE" />
        <action android:name="com.google.android.c2dm.intent.REGISTRATION" />
        <category android:name="${applicationId}" />
    </intent-filter>
</receiver>
```

On MainApplication OnCreate

```csharp
 FirebasePushNotificationManager.Initialize(this);

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

FirebasePushNotificationManager.Initialize(options);

```
 Note: When using Xamarin Forms do it just after LoadApplication call.

Also should override these methods and make the following calls:
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



Once token is refreshed you will get it on **OnTokenRefresh** event.



#### Events in FirebasePushNotification

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

Token event usage sample:
```csharp

  CrossFirebasePushNotification.Current.OnTokenRefresh += (s,p) =>
  {
        System.Diagnostics.Debug.WriteLine($"TOKEN : {p.Token}");
  };

```

Push message received event usage sample:
```csharp

  CrossFirebasePushNotification.Current.OnNotificationReceived += (s,p) =>
  {
 
        System.Diagnostics.Debug.WriteLine("Received");
    
  };

```

Push message opened event usage sample:
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



<= Back to [Table of Contents](../README.md)
