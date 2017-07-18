## FAQ

1. Getting java.lang.IllegalStateException: Default FirebaseApp is not initialized in this process {your_package_name}. Make sure to call FirebaseApp.initializeApp(Context) first.

Make sure the google-services.json has the GoogleServicesJson build action. If you have that set, then clean and build again, this is a known issue when using Firebase Component. More info and fix here: https://bugzilla.xamarin.com/show_bug.cgi?id=56108

2. Android initialization should be done on and Android Application class to be able to handle received notifications when application is closed. Since no activity exist when application is closed.
