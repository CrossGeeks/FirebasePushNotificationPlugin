## Firebase Push Notification Plugin for Xamarin iOS and Android
Simple cross platform plugin for handling firebase push notifications.

### Setup
* Available on NuGet: http://www.nuget.org/packages/Plugin.FirebasePushNotification [![NuGet](https://img.shields.io/nuget/v/Plugin.FirebasePushNotification.svg?label=NuGet)](https://www.nuget.org/packages/Plugin.FirebasePushNotification/)
* Install into your PCL project and Client projects.

**Platform Support**

|Platform|Version|
| ------------------- | :------------------: |
|Xamarin.iOS|iOS 8+|
|Xamarin.Android|API 15+|

### API Usage

Call **CrossFirebasePushNotification.Current** from any project or PCL to gain access to APIs.

## Features

- Receive firebase push notifications
- Subscribing/Unsubcribing to topics
- Support for push notification category actions
- Customize push notifications

## Documentation

Here you will find detailed documentation on setting up and using the Firebase Push Notification Plugin for Xamarin

* [Firebase Setup](docs/FirebaseSetup.md)
* [Getting Started](docs/GettingStarted.md)
* [Receiving Push Notifications](docs/FirebaseNotifications.md)
* [Testing Push Notifications](docs/TestingPushNotifications.md)
* [Notifications Actions](docs/NotificationActions.md)
* [FAQ](docs/FAQ.md)
