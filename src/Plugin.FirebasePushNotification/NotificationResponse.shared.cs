using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.FirebasePushNotification
{
    public class NotificationResponse
    {
        public string Identifier { get; }

        public IDictionary<string,object> Data { get; }

        public NotificationCategoryType Type { get; }

        public NotificationResponse(IDictionary<string, object> data, string identifier = "", NotificationCategoryType type = NotificationCategoryType.Default)
        {
            Identifier = identifier;
            Data = data;
            Type = type;
        }
    }
}
