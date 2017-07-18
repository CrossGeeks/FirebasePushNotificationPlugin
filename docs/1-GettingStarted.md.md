# Getting Started

## Setup
* NuGet: [Plugin.FirebasePushNotification](http://www.nuget.org/packages/Plugin.FirebasePushNotification) [![NuGet](https://img.shields.io/nuget/v/Plugin.FirebasePushNotification.svg?label=NuGet)](https://www.nuget.org/packages/Plugin.FirebasePushNotification/)
* `PM> Install-Package Plugin.FirebasePushNotification`
* Install into ALL of your projects, include client projects.

## Firebase Cloud Messaging Setup

Login to https://console.firebase.google.com

## Using FirebasePushNotification APIs
It is drop dead simple to gain access to the FirebasePushNotification APIs in any project. All you need to do is get a reference to the current instance of IFirebasePushNotification via `CrossFirebasePushNotification.Current`:

```csharp
public bool DoIHaveInternet()
{
    return CrossFirebasePushNotification.Current.IsConnected;
}
```

There may be instances where you install a plugin into a platform that it isn't supported yet. This means you will have access to the interface, but no implementation exists. You can make a simple check before calling any API to see if it is supported on the platform where the code is running. This if nifty when unit testing:

```csharp
public bool DoIHaveInternet()
{
    if(!CrossFirebasePushNotification.IsSupported)
        return true;

    return CrossFirebasePushNotification.Current.IsConnected;
}
```

## Disposing of FirebasePushNotification Plugin
This plugin also implements IDisposable on all implementations. This ensure that all events are unregistered from the platform. Only dispose when you need to and are no longer listening to events. The next time you gain access to the `CrossFirebasePushNotification.Current` a new instance will be created.

```csharp
public bool DoIHaveInternet()
{
    if(!CrossFirebasePushNotification.IsSupported)
        return true;
        
    //Do this only if you need to and aren't listening to any other events as they will not fire.
    using(var FirebasePushNotification = CrossFirebasePushNotification.Current)
    {
        return FirebasePushNotification.IsConnected;
    }
}
```


## Permissions & Additional Setup Considerations

### Android:
The `ACCESS_NETWORK_STATE` and `ACCESS_WIFI_STATE` permissions are required and are automatically added to your Android Manifest when you compile. No need to add them manually!

By adding these permissions [Google Play will automatically filter out devices](http://developer.android.com/guide/topics/manifest/uses-feature-element.html#permissions-features) without specific hardware. You can get around this by adding the following to your AssemblyInfo.cs file in your Android project:

```csharp
[assembly: UsesFeature("android.hardware.wifi", Required = false)]
```

## Architecture

### What's with this .Current Global Variable? Why can't I use $FAVORITE_IOC_LIBARY
You totally can! Every plugin I create is based on an interface. The static singleton just gives you a super simple way of gaining access to the platform implementation. If you are looking to use Depenency Injector or Inversion of Control (IoC) you will need to gain access to a reference of `IFirebasePushNotification`. 

This is what your code may look like when using this approach:

```csharp
public MyViewModel()
{
    readonly IFirebasePushNotification FirebasePushNotification;
    public MyViewModel(IFirebasePushNotification FirebasePushNotification)
    {
        this.FirebasePushNotification = FirebasePushNotification;
    }

    public bool IsConnected => FirebasePushNotification.IsConnected;
}
```

Remember that the implementation of the plugin lives in the platform specific applications, which means you will need to register .Current (or instantiate your own CrossFirebasePushNotificationImplementation) in your IoC container as the implementation of IFirebasePushNotification on each platform. This registration must happen from your application binary, not from your portable/netstandard class library.

### What About Unit Testing?
To learn about unit testing strategies be sure to read my blog: [Unit Testing Plugins for Xamarin](http://motzcod.es/post/159267241302/unit-testing-plugins-for-xamarin)


<= Back to [Table of Contents](README.md)