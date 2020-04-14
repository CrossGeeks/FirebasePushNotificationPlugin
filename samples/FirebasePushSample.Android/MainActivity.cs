using Android.App;
using Android.Widget;
using Android.OS;
using Android.Gms.Common;
using Plugin.FirebasePushNotification;
using Android.Content.PM;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Android.Content;

namespace FirebasePushSample.Droid
{
    [Activity(Label = "FirebasePushSample", Icon = "@drawable/icon", Theme = "@style/MainTheme",LaunchMode = LaunchMode.SingleTop,  ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);
            global::Xamarin.Forms.Forms.Init(this, bundle);
            LoadApplication(new App());

            FirebasePushNotificationManager.ProcessIntent(this,Intent);

        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
            FirebasePushNotificationManager.ProcessIntent(this,intent);
        }

    }
}

