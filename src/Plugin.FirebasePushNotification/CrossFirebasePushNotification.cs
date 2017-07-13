using Plugin.FirebasePushNotification.Abstractions;
using System;

namespace Plugin.FirebasePushNotification
{
  /// <summary>
  /// Cross platform FirebasePushNotification implemenations
  /// </summary>
  public class CrossFirebasePushNotification
  {
    static Lazy<IFirebasePushNotification> Implementation = new Lazy<IFirebasePushNotification>(() => CreateFirebasePushNotification(), System.Threading.LazyThreadSafetyMode.PublicationOnly);

    /// <summary>
    /// Current settings to use
    /// </summary>
    public static IFirebasePushNotification Current
    {
      get
      {
        var ret = Implementation.Value;
        if (ret == null)
        {
          throw NotImplementedInReferenceAssembly();
        }
        return ret;
      }
    }

    static IFirebasePushNotification CreateFirebasePushNotification()
    {
#if PORTABLE
        return null;
#else
        return new FirebasePushNotificationManager();
#endif
    }

    internal static Exception NotImplementedInReferenceAssembly()
    {
      return new NotImplementedException("This functionality is not implemented in the portable version of this assembly.  You should reference the NuGet package from your main application project in order to reference the platform-specific implementation.");
    }
  }
}
