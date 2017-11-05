## Localized Firebase Push Notifications

When having an application available in multiple countries is very common to have multilingual support to provide the best user experience posible. That's why we provided this guide in case you need support for localization on your push notifications.

### iOS -  Push Notifications Localization

On iOS push notifications are associated with iOS default platform localization.

> If you use a consistent set of messages for your notifications, you can store localized versions of the message text in your app bundle and use the loc-key and loc-args keys in your payload to specify which message to display. The loc-key and loc-args keys define the message content of the notification. When present, the local system searches the app’s Localizable.strings files for a key string matching the value in loc-key. It then uses the corresponding value from the strings file as the basis for the message text, replacing any placeholder values with the strings specified by the loc-args key. (You can also specify a title string for the notification using the title-loc-key and title-loc-args keys.). [Apple documentation](https://developer.apple.com/library/content/documentation/NetworkingInternet/Conceptual/RemoteNotificationsPG/CreatingtheNotificationPayload.html#//apple_ref/doc/uid/TP40008194-CH10-SW9)

So you can use iOS default platform localization by following this brief guide:

On the iOS project you have to add **Localizable.strings** (Build action for these files should be Bundle Resource) for each language you need to support. This is the file where you will define your keys and values based on the language.

These files are organized by language, in specially named directories with an **.lproj** suffix.

**Base.lproj** is the directory that contains resources for the default language. It is often located in the project root (but can also be placed in the Resources folder).

**.lproj** directories are created for each supported language, usually in the Resources folder.

Once you add this files your structure should look similar to this:

![image](https://user-images.githubusercontent.com/2547751/32307919-f2d3131e-bf37-11e7-8f14-041585071342.png)

This is an example Base.lproj/Localizable.strings (default language) file:

```
"NOTIFICATION_TITLE" = "Hello World";
"NOTIFICATION_MESSAGE" = "This is a message";
```


This is an example es.lproj/Localizable.strings (ie. Spanish) file:

```
"NOTIFICATION_TITLE" = "Hola Mundo";
"NOTIFICATION_MESSAGE" = "Esto es un mensaje";
```

On the payload instead of using **title**  and **message** keys, we will use **title-loc-key** to represent the key for our localized title and **loc-key** to represent the key for our localized message.

Finally, when sending the payload would look like this:

```json
{
     "notification": {
            "title-loc-key" : "NOTIFICATION_TITLE",
            "loc-key" : "NOTIFICATION_MESSAGE"
     },
     "priority": "high",
     "condition": "'general' in topics"
}
```

That works out just fine for a static message but if you have variable parameters in the content then you will need some extra keys on your payloads and define the value for your Localizable.strings differently. Let's say my payload have variable content on the title and message, then there are two additional keys we need to add **title-loc-args** (represents the variable parameters for the title) and **loc-args** (represents the variable parameters for the message).

```json
{
     "notification": {
            "title-loc-key" : "NOTIFICATION_TITLE",
            "loc-key" : "NOTIFICATION_MESSAGE",
            "title-loc-args" : ["Dominican Republic"],
            "loc-args" : ["Rendy"]
     },
     "priority": "high",
     "condition": "'general' in topics"
}
```
Each of this two payload keys is a string array so you can have as many string arguments as you need. Here's how should **Localizable.strings** files look like when using arguments:

This is an example Base.lproj/Localizable.strings (default language) file:

```
"NOTIFICATION_TITLE" = "Hello %@";
"NOTIFICATION_MESSAGE" = "This is a message for %@";
```


This is an example es.lproj/Localizable.strings (ie. Spanish) file:

```
"NOTIFICATION_TITLE" = "Hola %@";
"NOTIFICATION_MESSAGE" = "Esto es un mensaje para %@";
```
Each %@ represents a parameter or parameters if more than one.


More information on iOS localization here:

[Apple Remote Notifications Localization](https://developer.apple.com/library/content/documentation/NetworkingInternet/Conceptual/RemoteNotificationsPG/CreatingtheNotificationPayload.html#//apple_ref/doc/uid/TP40008194-CH10-SW9)

[Xamarin iOS Localization](https://developer.xamarin.com/guides/ios/advanced_topics/localization_and_internationalization/)


### Android -  Push Notifications Localization

On Android the plugin has localization support by default using Android platform localization. Uses specific keys to be sent on the payload that will then be resolved with values on string files depending on the android language set to the device.

First, you have to add a **strings.xml** (Build action for these files should be Android Resource) for each language you need to support. This is the file where you will define your keys and values based on the language.

These files are organized by language, in specially named directories with an **values** and a language suffix (For example values-en, values-fr, values-es). Naming must conform specifications on [AlternativeResources](https://developer.android.com/guide/topics/resources/providing-resources.html#AlternativeResources).

 **values**: without any suffix is the directory that contains resources for the default language. Placed in the Resources folder.

 **values-{language code}**: directories are created for each supported language, placed in the Resources folder. Examples : **values-ja**, **values-fr**, **values-es**.

 Notes:

 - If device set to a language that has a strings.xml file (ex. Resources/values-es/strings.xml) and have the requested key defined in this file then will use the value for this file.
 - If device set to a language that doesn't have strings.xml file then will fallback to default string.xml on **values** without suffix.
 - If device set to a language that has a strings.xml file (ex. Resources/values-es/strings.xml) but doesn't have a specific key requested then will fallback to default string.xml on **values** without suffix to look for this key/value.

 
Example:

Let's say we need to support 2 languages: English, Spanish. In this case, you could create two alternative strings.xml files, each stored in a locale-specific resource directory:

1. Resources/values/strings.xml (default file)

```xml
<string name="notification_title_string">Hello world</string>
<string name="notification_message_string">This is a message</string>
```
2. Resources/values-es/strings.xml (Spanish strings file)

```xml
<string name="notification_title_string">Hola mundo</string>
<string name="notification_message_string">Esto es un mensaje</string>
```
On the payload instead of using **title**  and **message** / **body**  keys, we will use **title_loc_key** to represent the key for our localized title and **body_loc_key** to represent the key for our localized message.

When sending the payload would look like this:

```json
{
    "data": {
        "title_loc_key": "notification_title_string",
		"body_loc_key": "notification_message_string"
     },
     "priority": "high",
     "condition": "'test' in topics"
}
```

When using parameters in the content you will need some extra keys on your payload and define the value for your strings.xml differently. Let's say the payload have variable content on the title and message, then there are two additional keys we need to add **title_loc_args** (represents the variable parameters for the title) and **body_loc_args** (represents the variable parameters for the message).

```json
{
    "data": {
          "title_loc_key" : "notification_title_string",
          "body_loc_key" : "notification_message_string",
          "title_loc_args" : ["Dominican Republic"],
          "body_loc_args" : ["Rendy"]
     },
     "priority": "high",
     "condition": "'test' in topics"
}
```

Each of this two payload keys is a string array so you can have as many string arguments as you need.

Here's how should **strings.xml** files look like when using arguments:

1. Resources/values/strings.xml (default file)

```xml
<string name="notification_title_string">Hola %1$s</string>
<string name="notification_message_string">Esto es un mensaje para %1$s</string>
```

2. Resources/values-es/strings.xml (Spansih strings file)

```xml
<string name="notification_title_string">Hello %1$s</string>
<string name="notification_message_string">This is a message for %1$s</string>
```

The format for parameter replacement is: %[parameter index]$[format type].

- Parameter index: Starts at 1. For example, if you have 2 parameters: %1 and %2. The order you place them in the resource string doesn't matter, only the order that you supply the parameters.
- Format type: Most common types are : $s (string), $d (decimal integer), $f (floating point number)

More information on string format for parameter replacing here:

[Dyanmic string](https://stackoverflow.com/questions/3656371/dynamic-string-using-string-xml)
[Formatting Resource Strings](https://code.tutsplus.com/tutorials/android-sdk-quick-tip-formatting-resource-strings--mobile-1775)

More information on localization here:

[Android Localization](https://developer.android.com/guide/topics/resources/localization.html)

<= Back to [Table of Contents](../README.md)

