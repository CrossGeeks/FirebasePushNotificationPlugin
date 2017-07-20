# Getting Started

## Setup
* NuGet: [Plugin.FirebasePushNotification](http://www.nuget.org/packages/Plugin.FirebasePushNotification) [![NuGet](https://img.shields.io/nuget/v/Plugin.FirebasePushNotification.svg?label=NuGet)](https://www.nuget.org/packages/Plugin.FirebasePushNotification/)
* `PM> Install-Package Plugin.FirebasePushNotification`
* Install into ALL of your projects, include client projects.

## Using Firebase Push Notification APIs
It is drop dead simple to gain access to the FirebasePushNotification APIs in any project. All you need to do is get a reference to the current instance of IFirebasePushNotification via `CrossFirebasePushNotification.Current`:

## Initial Configuration

### Android Configuration

Edit AndroidManifest.xml and insert the following receiver elements inside the application section:

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
Also add this permission:

```xml
<uses-permission android:name="android.permission.INTERNET" />
```


## Initialize

### Android Initialization

You should initialize the plugin on an Android Application class if you don't have one on your project, should create an application class. Then call **FirebasePushNotificationManager.Initialize** method on OnCreate.

There are 3 overrides to **FirebasePushNotificationManager.Initialize**:

- **FirebasePushNotificationManager.Initialize(Context context, bool resetToken)** : Default method to initialize plugin without supporting any user notification categories. Uses a DefaultPushHandler to provide the ui for the notification.

- **FirebasePushNotificationManager.Initialize(Context context, bool resetToken, NotificationUserCategory[] categories)**  : Initializes plugin using user notification categories. Uses a DefaultPushHandler to provide the ui for the notification supporting buttons based on the action_click send on the notification

- **FirebasePushNotificationManager.Initialize(Context context, bool resetToken,IPushNotificationHandler pushHandler)** : Initializes the plugin using a custom push notification handler to provide custom ui and behaviour notifications receipt and opening.

**Important: While debugging set resetToken parameter to true.**

Example of initialization:

```csharp

    [Application]
    public class MainApplication : Application
    {
        public MainApplication(IntPtr handle, JniHandleOwnership transer) :base(handle, transer)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();
            
            //If debug you should reset the token each time.
            #if DEBUG
              FirebasePushNotificationManager.Initialize(this,true);
            #else
              FirebasePushNotificationManager.Initialize(this,false);
            #endif

              //Handle notification when app is closed here
              CrossFirebasePushNotification.Current.OnNotificationReceived += (s,p) =>
              {


              };
         }
    }

```

On your main launcher activity OnCreate method

```csharp
 FirebasePushNotificationManager.ProcessIntent(Intent);
 ```

 **Note: When using Xamarin Forms do it just after LoadApplication call.**

### iOS Initialization

There are 3 overrides to **FirebasePushNotificationManager.Initialize**:

- **FirebasePushNotificationManager.Initialize(NSDictionary userInfo)** : Default method to initialize plugin without supporting any user notification categories.

- **FirebasePushNotificationManager.Initialize(NSDictionary userInfo, NotificationUserCategory[] categories)**  : Initializes plugin using user notification categories to support iOS notification actions.

- **FirebasePushNotificationManager.Initialize(NSDictionary userInfo,IPushNotificationHandler pushHandler)** : Initializes the plugin using a custom push notification handler to provide native feedback of notifications event on the native platform.


Call  **FirebasePushNotificationManager.Initialize** on AppDelegate FinishedLaunching
```csharp

FirebasePushNotificationManager.Initialize(options);

```
 **Note: When using Xamarin Forms do it just after LoadApplication call.**

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

## Events

Once token is registered/refreshed you will get it on **OnTokenRefresh** event.

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
