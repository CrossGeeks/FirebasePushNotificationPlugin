using Plugin.FirebasePushNotification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FirebasePushSplashSample
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            CrossFirebasePushNotification.Current.OnNotificationReceived += (s, p) =>
            {
                try
                {
                    System.Diagnostics.Debug.WriteLine("Received");
                    if (p.Data.ContainsKey("body"))
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            textLabel.Text = $"{p.Data["body"]}";
                        });

                    }
                }
                catch (Exception ex)
                {

                }

            };

            CrossFirebasePushNotification.Current.OnNotificationOpened += (s, p) =>
            {
                //System.Diagnostics.Debug.WriteLine(p.Identifier);

                System.Diagnostics.Debug.WriteLine("Opened");
                foreach (var data in p.Data)
                {
                    System.Diagnostics.Debug.WriteLine($"{data.Key} : {data.Value}");
                }

                if (!string.IsNullOrEmpty(p.Identifier))
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        textLabel.Text = p.Identifier;
                    });
                }
                else if (p.Data.ContainsKey("aps.alert.title"))
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        textLabel.Text = $"{p.Data["aps.alert.title"]}";
                    });

                }
            };
        }
    }
}
