## Testing Push Notifications

Perform POST to https://fcm.googleapis.com/fcm/send

Using headers:

**Content-Type: application/json**

**Authorization: {your-fcm-server-key}**


Sending to specific topic:
```json
{
  "to": "/topics/test",
  "data": {
    "message": "hello world"
   }
}
```

Sending push to specific devices:
```json
{
    "data": {
         "message": "hello world"
     },
    "registration_ids": ["{device-token}","{device2-token}"]
}
```

Sending push to topic based on conditions:

```json
{
    "data": {
        "message": "hello world"
     },
     "priority": "high",
     "condition": "'test' in topics"
}
```

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
