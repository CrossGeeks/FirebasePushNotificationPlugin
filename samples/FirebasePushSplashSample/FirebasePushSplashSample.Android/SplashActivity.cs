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
    [Activity(Theme = "@style/MainTheme.Splash", MainLauncher = true, NoHistory = true, ScreenOrientation = global::Android.Content.PM.ScreenOrientation.Portrait)]

    public class SplashActivity : AppCompatActivity
    {

        public override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            StartActivity(new Intent(Application.Context, typeof(MainActivity)));
        }

        protected override void OnResume()
        {
            base.OnResume();
        }

    }
}