using Xamarin.Forms;

namespace FirebasePushSample
{
    public partial class MainPage : ContentPage
    {
        public string Message
        {
            get
            {
                return textLabel.Text;
            }
            set
            {
                textLabel.Text = value;
            }
        }
        public MainPage()
        {
            InitializeComponent();


        }
    }
}
