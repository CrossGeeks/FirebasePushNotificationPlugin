# Getting Started

## Setup
* NuGet: [Plugin.FirebasePushNotification](http://www.nuget.org/packages/Plugin.FirebasePushNotification) [![NuGet](https://img.shields.io/nuget/v/Plugin.FirebasePushNotification.svg?label=NuGet)](https://www.nuget.org/packages/Plugin.FirebasePushNotification/)
* `PM> Install-Package Plugin.FirebasePushNotification`
* Install into ALL of your projects, include client projects.

## Getting Started Video

Here a step by step video to get started by setting up everything in your project:

<a href="https://youtu.be/FrxPEMLfV-o" target="_blank">Getting started with Firebase Push Notifications Plugin</a>

[![Getting Started Video](https://img.youtube.com/vi/FrxPEMLfV-o/0.jpg)](https://youtu.be/FrxPEMLfV-o)]


## Starting with Android

### Android Configuration

Also add this permission:

```xml
<uses-permission android:name="android.permission.INTERNET" />
```

Add google-services.json to Android project. Make sure build action is GoogleServicesJson

![ADD JSON](https://github.com/CrossGeeks/FirebasePushNotificationPlugin/blob/master/images/android-googleservices-json.png?raw=true)

Must compile against 26+ as plugin is using API 26 specific things. Here is a great breakdown: http://redth.codes/such-android-api-levels-much-confuse-wow/ (Android project must be compiled using 8.0+ target framework)

### Android Initialization

You should initialize the plugin on an Android Application class if you don't have one on your project, should create an application class. Then call **FirebasePushNotificationManager.Initialize** method on OnCreate.

There are 3 overrides to **FirebasePushNotificationManager.Initialize**:

- **FirebasePushNotificationManager.Initialize(Context context, bool resetToken,bool autoRegistration)** : Default method to initialize plugin without supporting any user notification categories. Uses a DefaultPushHandler to provide the ui for the notification.

- **FirebasePushNotificationManager.Initialize(Context context, NotificationUserCategory[] categories, bool resetToken,bool autoRegistration)**  : Initializes plugin using user notification categories. Uses a DefaultPushHandler to provide the ui for the notification supporting buttons based on the action_click send on the notification

- **FirebasePushNotificationManager.Initialize(Context context,IPushNotificationHandler pushHandler, bool resetToken,bool autoRegistration)** : Initializes the plugin using a custom push notification handler to provide custom ui and behaviour notifications receipt and opening.

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
	    
	    //Set the default notification channel for your app when running Android Oreo
	    if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
	    {
		 //Change for your default notification channel id here
	         FirebasePushNotificationManager.DefaultNotificationChannelId = "FirebasePushNotificationChannel";

		 //Change for your default notification channel name here
		 FirebasePushNotificationManager.DefaultNotificationChannelName = "General";
	    }

            
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

By default the plugin launches the activity where **ProcessIntent** method is called when you tap at a notification, but you can change this behaviour by setting the type of the activity you want to be launch on **FirebasePushNotificationManager.NotificationActivityType**

If you set **FirebasePushNotificationManager.NotificationActivityType** then put the following call on the **OnCreate** of activity of the type set. If not set then put it on your main launcher activity **OnCreate** method (On the Activity you got MainLauncher= true set)

```csharp
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

			//Other initialization stuff

            FirebasePushNotificationManager.ProcessIntent(this,Intent);
        }

 ```

**Note: When using Xamarin Forms do it just after LoadApplication call.**

By default the plugin launches the activity when you tap at a notification with activity flags: **ActivityFlags.ClearTop | ActivityFlags.SingleTop**.

You can change this behaviour by setting **FirebasePushNotificationManager.NotificationActivityFlags**. 
 
If you set **FirebasePushNotificationManager.NotificationActivityFlags** to ActivityFlags.SingleTop  or using default plugin behaviour then make this call on **OnNewIntent** method of the same activity on the previous step.
       
 ```csharp
	    protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
            FirebasePushNotificationManager.ProcessIntent(this,intent);
        }
 ```

 More information on **FirebasePushNotificationManager.NotificationActivityType** and **FirebasePushNotificationManager.NotificationActivityFlags** and other android customizations here:

 [Android Customization](../docs/AndroidCustomization.md)

## Starting with iOS 

### iOS Configuration

 Add GoogleService-Info.plist to iOS project. Make sure build action is BundleResource

![ADD Plist](https://github.com/CrossGeeks/FirebasePushNotificationPlugin/blob/master/images/iOS-googleservices-plist.png?raw=true)

On Info.plist enable remote notification background mode

![Remote notifications](https://github.com/CrossGeeks/FirebasePushNotificationPlugin/blob/master/images/iOS-enable-remote-notifications.png?raw=true)

Add FirebaseAppDelegateProxyEnabled in the app’s Info.plist file and set it to No 

![Disable Swizzling](https://github.com/CrossGeeks/FirebasePushNotificationPlugin/blob/master/images/iOS-disable-swizzling.png?raw=true)


### iOS Initialization

There are 3 overrides to **FirebasePushNotificationManager.Initialize**:

- **FirebasePushNotificationManager.Initialize(NSDictionary options,bool autoRegistration)** : Default method to initialize plugin without supporting any user notification categories. Auto registers for push notifications if second parameter is true.

- **FirebasePushNotificationManager.Initialize(NSDictionary options, NotificationUserCategory[] categories,bool autoRegistration)**  : Initializes plugin using user notification categories to support iOS notification actions.

- **FirebasePushNotificationManager.Initialize(NSDictionary options,IPushNotificationHandler pushHandler,bool autoRegistration)** : Initializes the plugin using a custom push notification handler to provide native feedback of notifications event on the native platform.


Call  **FirebasePushNotificationManager.Initialize** on AppDelegate FinishedLaunching
```csharp

FirebasePushNotificationManager.Initialize(options,true);

```
 **Note: When using Xamarin Forms do it just after LoadApplication call.**

Also should override these methods and make the following calls:
```csharp
        public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
        {
              FirebasePushNotificationManager.DidRegisterRemoteNotifications(deviceToken);
        }

        public override void FailedToRegisterForRemoteNotifications(UIApplication application, NSError error)
        {
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

			completionHandler (UIBackgroundFetchResult.NewData);
        }
```


## Using Firebase Push Notification APIs
It is drop dead simple to gain access to the FirebasePushNotification APIs in any project. All you need to do is get a reference to the current instance of IFirebasePushNotification via `CrossFirebasePushNotification.Current`:


### On Demand Permission Registration

When plugin initializes by default auto registers the device permissions for push notifications. If needed you can do on demand registration by turning off auto registration when initializing the plugin.

Use the following method for on demand permission registration:

```csharp
   CrossFirebasePushNotification.Current.RegisterForPushNotifications();
```


### Events

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

             
 };
```

Push message action tapped event usage sample:
**OnNotificationAction**
```csharp
  
  CrossFirebasePushNotification.Current.OnNotificationAction += (s,p) =>
  {
                System.Diagnostics.Debug.WriteLine("Action");
           
                if(!string.IsNullOrEmpty(p.Identifier))
                {
                    System.Diagnostics.Debug.WriteLine($"ActionId: {p.Identifier}");
				    foreach(var data in p.Data)
					{
						System.Diagnostics.Debug.WriteLine($"{data.Key} : {data.Value}");
					}

                }
             
 };
```


Push message deleted event usage sample: (Android Only)
```csharp

  CrossFirebasePushNotification.Current.OnNotificationDeleted+= (s,p) =>
  {
 
        System.Diagnostics.Debug.WriteLine("Deleted");
    
  };

```

Plugin by default provides some notification customization features for each platform. Check out the [Android Customization](../docs/AndroidCustomization.md) and [iOS Customization](../docs/iOSCustomization.md) sections.


<= Back to [Table of Contents](../README.md)
