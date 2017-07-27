## Testing Push Notifications

Perform POST to https://fcm.googleapis.com/fcm/send

Using headers:

**Content-Type: application/json**

**Authorization: key={your-fcm-server-key}**


Sending to specific topic:

Android:
```json
{
  "to": "/topics/test",
  "data": {
    "message": "hello world"
   }
}
```

iOS:

```json
{
  "to" : "/topics/test",
  "priority" : "high",
  "notification" : {
    "body" : "Hello world",
    "title" : "FCM Message",
  }
}
```

Sending push to specific devices:

Single device

Android:
```json
{
  "to": "{device-token}",
  "data": {
    "message": "hello world"
   }
}
```

iOS:

```json
{
  "to" : "{device-token}",
  "priority" : "high",
  "notification" : {
    "body" : "Hello world",
    "title" : "FCM Message",
  }
}
```

Multiple devices:

Android:

```json
{
    "data": {
         "message": "hello world"
     },
    "registration_ids": ["{device-token}","{device2-token}"]
}
```

iOS:

```json
{
    "priority" : "high",
    "notification" : {
      "body" : "Hello world",
      "title" : "FCM Message"
    },
    "registration_ids": ["{device-token}","{device2-token}"]
}
```

Sending push to topic based on conditions:

Android:

```json
{
    "data": {
        "message": "hello world"
     },
     "priority": "high",
     "condition": "'test' in topics"
}
```

```json
{
  "condition": "'dogs' in topics || 'cats' in topics",
  "data": {
    "message": "This is a Firebase Cloud Messaging Topic Message!"
   }
}
```

iOS:

```json
{
    "priority" : "high",
    "notification" : {
      "body" : "Hello world",
      "title" : "FCM Message"
    },
    "condition": "'test' in topics"
}
```

```json
{
  "priority" : "high",
  "notification" : {
    "body" : "Hello world",
    "title" : "FCM Message"
  },
  "condition": "'dogs' in topics || 'cats' in topics"
}
```



More information here: https://firebase.google.com/docs/cloud-messaging/android/topic-messaging

### Using [FirebaseNet](https://www.nuget.org/packages/firebaseNet) to send push notifications:

#### Sending push notification to Android:

```csharp
 Task.Run(async() => {
 
            FCMClient client = new FCMClient("YOUR_APP_SERVER_KEY"); //as derived from https://console.firebase.google.com/project/
            
            
            var message = new Message()
            {
                To = "DEVICE_ID_OR_ANY_PARTICULAR_TOPIC", //topic example /topics/all
                Notification = new AndroidNotification()
                {
                    Body = "Hello World",
                    Title = "MyApp",
                },
                
                Data = new Dictionary<string, string>
                {
                    { "number", "1" },
                    { "count", "10" }
                }
            };
           
            var result = await client.SendMessageAsync(message);
            return result;
 });
```
#### Sending push notification to iOS:

```csharp
 Task.Run(async() => {
 
            FCMClient client = new FCMClient("YOUR_APP_SERVER_KEY"); //as derived from https://console.firebase.google.com/project/
            
            
            var message = new Message()
            {
                To = "DEVICE_ID_OR_ANY_PARTICULAR_TOPIC", //topic example /topics/all
                Notification = new iOSNotification()
                {
                    Body = "Hello World",
                    Title = "MyApp",
                }
            };
           
            var result = await client.SendMessageAsync(message);
            return result;
 });
```
More information here: https://github.com/tiagomtotti/firebaseNet        

<= Back to [Table of Contents](../README.md)
