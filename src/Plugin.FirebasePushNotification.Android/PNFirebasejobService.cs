using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.App.Job;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Plugin.FirebasePushNotification
{
    public class PNFirebaseJobService : JobService
    {
        public override bool OnStartJob(JobParameters @params)
        {
            return false;
        }

        public override bool OnStopJob(JobParameters @params)
        {
            return false;
        }
    }
}