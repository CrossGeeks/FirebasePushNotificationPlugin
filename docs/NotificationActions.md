## Notification Actions

You can initialize the plugin with notification user categories to provide button options within the notification once receiving one with one of the notification user categories configured.

Initialize using a User Category actions

Android on Application class:

```csharp

  FirebasePushNotificationManager.Initialize(this,true,new NotificationUserCategory[] {
                new NotificationUserCategory("message",new List<NotificationUserAction> {
                    new NotificationUserAction("Reply","Reply",NotificationActionType.Foreground),
                    new NotificationUserAction("Forward","Forward",NotificationActionType.Foreground)

                }),
                new NotificationUserCategory("request",new List<NotificationUserAction> {
                    new NotificationUserAction("Accept","Accept","check"),
                    new NotificationUserAction("Reject","Reject","cancel")
                })

```

iOS on AppDelegate FinishLaunching:

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
Can use **click_action** key to use notification actions.

You will get the identifier of the action that was clicked on **OnNotificationOpened** event:

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


