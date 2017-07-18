## Testing Push Notifications

Perform a POST to the following URL:

POST https://fcm.googleapis.com/fcm/send

Using headers:

Content-Type: application/json

Authorization: {your-fcm-server-key}


Sending to specific topic on iOS:
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
