## Firebase Setup

1. Login to https://console.firebase.google.com and create a new project

![Creating Project](https://github.com/CrossGeeks/FirebasePushNotificationPlugin/blob/master/images/firebase-portal-create-project.png?raw=true)


2. After project is created you should see options to add Firebase to your iOS and Android apps

![Firebase Overview](https://github.com/CrossGeeks/FirebasePushNotificationPlugin/blob/master/images/firebase-overview.png?raw=true)

#### Android Firebase Setup

3. Let's start adding firebase to our Android application. Package name must match your Android app package name.

![Add Firebase to Android](https://github.com/CrossGeeks/FirebasePushNotificationPlugin/blob/master/images/firebase-portal-create-android-app.png?raw=true)

4. Download the file google-services.json

![GooglePlayJson](https://github.com/CrossGeeks/FirebasePushNotificationPlugin/blob/master/images/firebase-portal-android-json.png?raw=true)

5. Add google-services.json to Android project. Make sure build action is GoogleServicesJson

![ADD JSON](https://github.com/CrossGeeks/FirebasePushNotificationPlugin/blob/master/images/android-googleservices-json.png?raw=true)


#### iOS Firebase Setup

6. Add Firebase to iOS App

![Add Firebase to iOS](https://github.com/CrossGeeks/FirebasePushNotificationPlugin/blob/master/images/firebase-portal-add-ios-app.png?raw=true)

![Add Firebase to iOS](https://github.com/CrossGeeks/FirebasePushNotificationPlugin/blob/master/images/firebase-portal-create-ios-app.png?raw=true)

7. Download GoogleService-Info.plist

![Plist iOS](https://github.com/CrossGeeks/FirebasePushNotificationPlugin/blob/master/images/firebase-portal-ios-plist.png?raw=true)

![Apps added](https://github.com/CrossGeeks/FirebasePushNotificationPlugin/blob/master/images/firebase-portal-apps.png?raw=true)

8. Add GoogleService-Info.plist to iOS project. Make sure build action is BundleResource

![ADD Plist](https://github.com/CrossGeeks/FirebasePushNotificationPlugin/blob/master/images/iOS-googleservices-plist.png?raw=true)

9. Add FirebaseAppDelegateProxyEnabled in the app’s Info.plist file and set it to No 

Also you'll need to create an APNs SSL Certificate or authentication token, then upload it to Firebase and finally register the app for remote notifications. Here some references:

[Communicate with APNs using a TLS certificate](http://help.apple.com/xcode/mac/current/#/dev11b059073?sub=dev1eb5dfe65)

[Communicate with APNs using authentication tokens](http://help.apple.com/xcode/mac/current/#/dev54d690a66?sub=dev1eb5dfe65)

<= Back to [Table of Contents](../README.md)
