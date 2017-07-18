## Notification Actions



Initialize using a User Category actions


Android on Application class:

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
Can use category or click_action keys to use notification actions.

Android Notification Sample Payload:
```json
{
    "data": {
        "body" : "hello!",
        "title": "afruz",
        "category":"message"
     },
     
     "priority": "high",

     "condition": "'general' in topics"
}
```

iOS Notification Sample Payload:
```json
{
   "data": {
        "category":"message"
     },
  "notification" : {
    "body" : "hello!",
    "title": "afruz",
    "sound": "default"
  },
  
  "priority": "high",

  "condition": "'general' in topics"
}
```


