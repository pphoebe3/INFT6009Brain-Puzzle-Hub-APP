using Plugin.LocalNotification;
using Plugin.LocalNotification.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BrainPuzzleHub.Services;

//Unified management of local notifications:
//Currently, only "Daily Challenge Reminder" is available
public class NotificationService
{
    private const int DailyReminderId = 1001;

    //Request notification permission and schedule a daily reminder at a fixed time
    public async Task<bool> ScheduleDailyReminderAsync(int hour, int minute)
    {
        bool granted = await LocalNotificationCenter.Current.RequestNotificationPermission();
        if (!granted) return false;

        var request = new NotificationRequest
        {
            NotificationId = DailyReminderId,
            Title = "Brain Puzzle Hub",
            Description = "Today's challenge is ready ！",
            Schedule = new NotificationRequestSchedule
            {
                NotifyTime = GetNextOccurrence(hour, minute),
                RepeatType = NotificationRepeat.Daily
            }
        };

        await LocalNotificationCenter.Current.Show(request);
        return true;
    }

    public void CancelDailyReminder()
    {
        LocalNotificationCenter.Current.Cancel(DailyReminderId);
    }

    //Calculate the next trigger time:
    //If today's trigger time has already passed, schedule it for tomorrow
    private DateTime GetNextOccurrence(int hour, int minute)
    {
        var now = DateTime.Now;
        var scheduled = new DateTime(now.Year, now.Month, now.Day, hour, minute, 0);
        if (scheduled <= now) scheduled = scheduled.AddDays(1);
        return scheduled;
    }
}