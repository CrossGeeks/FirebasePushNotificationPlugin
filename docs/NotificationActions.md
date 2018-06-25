## Notification Category Actions

You can initialize the plugin with notification user categories to provide button options within the notification. Depending on the notification category received you can provide different button options.

<p align="center">
<img src="https://github.com/CrossGeeks/FirebasePushNotificationPlugin/blob/master/Gifs/CrossGeeks%20Push%20Notification%20-%20Category%20Actions%20Android.gif?raw=true"  title="Android"/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
<img src="https://github.com/CrossGeeks/FirebasePushNotificationPlugin/blob/master/Gifs/CrossGeeks%20Push%20Notification%20-%20Category%20Actions%20iOS.gif?raw=true" height="380" title="iOS"/>
</p>




### Notification User Category:

Each notification user category can have it's own options

```csharp
public class NotificationUserCategory
{
   public string Category { get; }
   
   public List<NotificationUserAction> Actions { get; }

   public NotificationCategoryType Type { get; }

   public NotificationUserCategory(string category, List<NotificationUserAction> actions, NotificationCategoryType type = NotificationCategoryType.Default)
   {
      Category = category;
      Actions = actions;
      Type = type;
   }
}
```

```csharp
  //This just applies for iOS on Android is always set as default when used
    public enum NotificationCategoryType
    {
        Default,
        Custom,
        Dismiss
    }
```

### Notification User Action:

Each user action represents a button option of the notification user category

```csharp
public class NotificationUserAction
{
   public string Id { get; } // Action Identifier
   
   public string Title { get; } //Title to be displayed for the option
   
   public NotificationActionType Type { get; } //Determines the behaviour when action is executed
   
   public string Icon { get; } //Applies only for Android
   
   public NotificationUserAction(string id, string title, NotificationActionType type = NotificationActionType.Default, string icon = "")
   {
      Id = id;
      Title = title;
      Type = type;
      Icon = icon;
   }
}
```
There are a few types of notification user actions which determines the behaviour of it when the button option is tapped:

```csharp
public enum NotificationActionType
{
        Default,  
        AuthenticationRequired, //Only applies for iOS
        Foreground,
        Destructive  //Only applies for iOS
}
```

**Default** : When a button with this notification action type is tapped will handle the notification on background won't bring the application to foreground. Action will take place without launching the application.

**Foreground** : When a button with this notification action type is tapped will bring the application to foreground and receive the notification once application is in foreground.

**AuthenticationRequired** : If set the user needs to insert the unlock code to launch the action in background. This action is iOS specific will be ignored on Android.

**Destructive** : Action button will have a red color. This action is iOS specific will be ignored on Android.

### Initialize using a User Category Notifications

Android on **Application** class **OnCreate** method:

```csharp
#if DEBUG
  FirebasePushNotificationManager.Initialize(this,
                new NotificationUserCategory[]
                {
                    new NotificationUserCategory("message",new List<NotificationUserAction> {
                        new NotificationUserAction("Reply","Reply", NotificationActionType.Foreground),
                        new NotificationUserAction("Forward","Forward", NotificationActionType.Foreground)

                    }),
                    new NotificationUserCategory("request",new List<NotificationUserAction> {
                    new NotificationUserAction("Accept","Accept", NotificationActionType.Default, "check"),
                    new NotificationUserAction("Reject","Reject", NotificationActionType.Default, "cancel")
                    })
                }, true);
#else
  FirebasePushNotificationManager.Initialize(this,
                new NotificationUserCategory[]
                {
                    new NotificationUserCategory("message",new List<NotificationUserAction> {
                        new NotificationUserAction("Reply","Reply", NotificationActionType.Foreground),
                        new NotificationUserAction("Forward","Forward", NotificationActionType.Foreground)

                    }),
                    new NotificationUserCategory("request",new List<NotificationUserAction> {
                    new NotificationUserAction("Accept","Accept", NotificationActionType.Default, "check"),
                    new NotificationUserAction("Reject","Reject", NotificationActionType.Default, "cancel")
                    })
                }, false);
#endif

```

iOS on **AppDelegate** FinishLaunching:

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
Should use **click_action** key to when sending notification with categories. The value for this key will be the category.


You will get the identifier of the action that was tapped on **OnNotificationAction** event:

```csharp
CrossFirebasePushNotification.Current.OnNotificationAction += (s,p) =>
{
                System.Diagnostics.Debug.WriteLine("Notification action tapped");
              

                if(!string.IsNullOrEmpty(p.Identifier))
                {
                    System.Diagnostics.Debug.WriteLine($"ActionId: {p.Identifier}");
                }
                
                foreach(var data in p.Data)
                {
                    System.Diagnostics.Debug.WriteLine($"{data.Key} : {data.Value}");
                }
             
 };
```

Android Notification Sample Payload:
```json
{
    "data": {
        "body" : "hello!",
        "title": "firebase",
        "click_action":"message"
     },
     
     "priority": "high",

     "condition": "'general' in topics"
}
```

iOS Notification Sample Payload:
```json
{
  "notification" : {
    "body" : "hello!",
    "title": "firebase",
    "click_action":"message",
    "sound": "default"
  },
  
  "priority": "high",
  "condition": "'general' in topics"
}
```
**Note: To receive push notifications using these payload samples you must be subscribed to "general" topic.**

<= Back to [Table of Contents](../README.md)


