## FAQ

<b> 1. Getting java.lang.IllegalStateException: Default FirebaseApp is not initialized in this process {your_package_name}. Make sure to call FirebaseApp.initializeApp(Context) first.</b>

Make sure the google-services.json has the GoogleServicesJson build action. If you have that set, then clean and build again, this is a known issue when using Firebase Component. More info and fix here: https://bugzilla.xamarin.com/show_bug.cgi?id=56108

<b> 2. Android initialization should be done on and Android Application class to be able to handle received notifications when application is closed. Since no activity exist when application is closed.</b>

<b> 3. You won't receive any push notifications if application is stopped while debugging, should reopen and close again for notifications to work when app closed. This is due to the application being on an unstable state when stopped while debugging.</b>

<b> 4. On some phones android background services might be blocked by some application. This is the case of ASUS Zenfone 3 that has an Auto-start manager, which disables background services by default. You need to make sure that your push notification service is not being blocked by some application like this one, since you won't receive push notifications when app is closed if so.</b>
