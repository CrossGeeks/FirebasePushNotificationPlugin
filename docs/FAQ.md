## FAQ

### Android

1. Getting <b>java.lang.IllegalStateException: Default FirebaseApp is not initialized in this process {your_package_name}</b>. 

	**Solution 1:**

	Make sure the google-services.json has the GoogleServicesJson build action

	**Solution 2:**

	Make sure your firebase android app package name is the same package name on your Android project.

2. Android initialization should be done on and Android <b>Application class</b> to be able to handle received notifications when application is closed. Since no activity exist when application is closed.

3. You won't receive any push notifications if application is stopped while <b>debugging</b>, should reopen and close again for notifications to work when app closed. This is due to the application being on an unstable state when stopped while debugging.

4. On some phones android background services might be blocked by some application. This is the case of the following phones:

	- On your Huawei device go to Settings > Protected apps > check "My App" where.
	- On your Xiaomi make sure your phone has the "Auto-start" property enabled for your app.
	- On your Asus make sure the <b>Auto-start manager</b> of your phone has the "Auto-start" property enabled for your app

   These phones disables background services starting by default. You need to make sure that your push notification service is not being blocked by the manufacturer application, since you won't receive push notifications when app is closed if so.

5. Must compile against 26+ as plugin is using API 26 specific things. Here is a great breakdown: http://redth.codes/such-android-api-levels-much-confuse-wow/ (Android project must be compiled using 8.0+ target framework)

6. The package name of your Android aplication must <b>start with lower case</b> or you will get the build error: <b><code>Installation error: INSTALL_PARSE_FAILED_MANIFEST_MALFORMED</code> </b>

7. Make sure you have updated your Android SDK Manager libraries:

![image](https://cloud.githubusercontent.com/assets/2547751/6440604/1b0afb64-c0b5-11e4-93b8-c496e2bfa588.png)

8. Topics are <b>case sensitive</b> so "test" and "Test" are different topics.

9. <b> Error 1589 NotificationService Not posting notification without small icon </b><br>
	It happen when the message is received, but the notification isn't displayed. If you got this error, it mean you need to tell which one is your app icon on <b>Android Project Properties > Android Manifest > application Icon</b> or in the <b>AndroidManifext.xml file and put android:icon="@drawable/{replace with your icon file name}"</b> in the

```xml	
	<application android:label="Test" android:icon="@drawable/{replace with your icon file name}">	
	...
	</application>
```

10. Not getting push notifications on Release but on Debug yes? Uninstall the debug version or increment the build number before making the release build.

### iOS

1. When subscribing to topics getting error: <b>Failed to subscribe to topic Error Domain=com.google.fcm Code=5 "(null)" </b>

Add this to your Info.plist:
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
