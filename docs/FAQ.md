## FAQ

### Android

<b> 1. Getting java.lang.IllegalStateException: Default FirebaseApp is not initialized in this process {your_package_name}. Make sure to call FirebaseApp.initializeApp(Context) first.</b>

Make sure the google-services.json has the GoogleServicesJson build action. If you have that set, then clean and build again, this is a known issue when using Firebase Component. More info and fix here: https://bugzilla.xamarin.com/show_bug.cgi?id=56108

<b> 2. Android initialization should be done on and Android Application class to be able to handle received notifications when application is closed. Since no activity exist when application is closed.</b>

<b> 3. You won't receive any push notifications if application is stopped while debugging, should reopen and close again for notifications to work when app closed. This is due to the application being on an unstable state when stopped while debugging.</b>

<b> 4. On some phones android background services might be blocked by some application. This is the case of ASUS Zenfone 3 that has an Auto-start manager, which disables background services by default. You need to make sure that your push notification service is not being blocked by some application like this one, since you won't receive push notifications when app is closed if so.</b>

<b> 5. Must compile against 21+ as plugin is using API 21 specific things. Here is a great breakdown: http://redth.codes/such-android-api-levels-much-confuse-wow/</b>

<b> 6. The package name of your Android aplication must <b>start with lower case</b> or you will get the build error: <code>Installation error: INSTALL_PARSE_FAILED_MANIFEST_MALFORMED</code> </b>

<b> 7. Make sure you have updated your Android SDK Manager libraries:</b>

![image](https://cloud.githubusercontent.com/assets/2547751/6440604/1b0afb64-c0b5-11e4-93b8-c496e2bfa588.png)


### iOS

<b> 1. When subscribing to topics getting error: Failed to subscribe to topic Error Domain=com.google.fcm Code=5 "(null)" </b>

Add this to you Info.plist:
```xml
<key>CFBundleURLTypes</key>
	<array>
		<dict>
			<key>CFBundleTypeRole</key>
			<string>Editor</string>
			<key>CFBundleURLSchemes</key>
			<array>
				<string>com.google.fcm</string>
			</array>
		</dict>
</array>
``` 

Some references:


https://stackoverflow.com/questions/37549717/cannot-receive-push-notification-on-ios-from-firebase-3-2-0-topics
https://stackoverflow.com/questions/37711082/how-to-handle-notification-when-app-in-background-in-firebase
https://stackoverflow.com/questions/41102932/updating-firebase-push-notification-when-app-is-in-background
https://github.com/xamarin/GooglePlayServicesComponents/issues/20
https://stackoverflow.com/questions/42158239/getting-exception-using-firebase-in-xamarin-android/42159446#42159446
https://stackoverflow.com/questions/37372806/firebase-cloud-messaging-and-multiple-topic-subscription-from-ios-fails
