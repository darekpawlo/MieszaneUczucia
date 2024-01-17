using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Notifications.Android;

public static class Notifications
{
    // Static constructor to set up the notification channel
    static Notifications()
    {
        string channelID = "channel_id";
        if(PlayerPrefs.HasKey("ID_klienta"))
        {
            channelID = PlayerPrefs.GetString("ID_klienta");
        }

        AndroidNotificationChannel channel = new AndroidNotificationChannel()
        {
            Id = channelID,
            Name = "Notifications Channel",
            Importance = Importance.High,
            Description = "Generic Notifications",
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);
    }

    // Static method to send a custom notification
    public static void SendCustomNotification(string title, string text, string idKlienta)
    {
        AndroidNotification notification = new AndroidNotification()
        {
            Title = title,
            Text = text,
            LargeIcon = "icon_0",
            SmallIcon = "icon_1",
            ShowTimestamp = true,
            FireTime = System.DateTime.Now.AddSeconds(10)
        };

        var identifier = AndroidNotificationCenter.SendNotification(notification, idKlienta);

        if (AndroidNotificationCenter.CheckScheduledNotificationStatus(identifier) == NotificationStatus.Scheduled)
        {
            AndroidNotificationCenter.CancelAllNotifications();
            AndroidNotificationCenter.SendNotification(notification, idKlienta);
        }
    }
}
