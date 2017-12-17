using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace FirebasePushSplashSample.Droid
{
    [Application]
    public class MainApplication : Application, Application.IActivityLifecycleCallbacks
    {
        public MainApplication(IntPtr handle, JniHandleOwnership transer): base(handle, transer)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();
            RegisterActivityLifecycleCallbacks(this);

            //If debug you should reset the token each time.
#if DEBUG
		    FirebasePushNotificationManager.Initialize(this,true);
#else
            FirebasePushNotificationManager.Initialize(this, false);
#endif
            //Handle notifications when app closed here
            CrossFirebasePushNotification.Current.OnNotificationReceived += (s, p) =>
            {
                System.Diagnostics.Debug.WriteLine("NOTIFICATION RECEIVED", p.Data);


            };


        }

    }
}