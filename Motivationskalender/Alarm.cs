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
using Java.Text;
using Java.Sql;
using System.Net.Mail;
using Android.Support.V4.App;
using TaskStackBuilder = Android.Support.V4.App.TaskStackBuilder;
using Motivationskalender.BroadCast;
using Android.Icu.Util;
using System.IO;
using Android.Content.Res;
using Android.Media;

namespace Motivationskalender
{
  static class Alarm
  {
    public static void CheckAlarm(Context context)
    {
      var savedSettings = Application.Context.GetSharedPreferences("SavedSettings", FileCreationMode.Private);
      var savedSettingsEdit = savedSettings.Edit();
      int hour = savedSettings.GetInt("hour", 20);
      int minute = savedSettings.GetInt("minute", 30);

      bool alarmUp = (PendingIntent.GetBroadcast(context, 0,
        new Intent(context, typeof(AlarmNotificationReceiver)),
        PendingIntentFlags.NoCreate) != null);
      bool alarmNeedsUpdate = savedSettings.GetBoolean("alarmNeedsUpdate", false);

      AlarmManager manager = (AlarmManager)context.ApplicationContext.GetSystemService(Context.AlarmService);
      Intent myIntent;
      PendingIntent pendingIntent;
      myIntent = new Intent(context, typeof(AlarmNotificationReceiver));
      pendingIntent = PendingIntent.GetBroadcast(context, 0, myIntent, 0);
      if (!alarmUp)
      {
        Java.Util.Calendar calendar = Java.Util.Calendar.Instance;
        calendar.Set(Java.Util.CalendarField.HourOfDay, hour);
        calendar.Set(Java.Util.CalendarField.Minute, minute);
        manager.SetRepeating(AlarmType.RtcWakeup, calendar.TimeInMillis, AlarmManager.IntervalDay, pendingIntent);
      }
      if (alarmNeedsUpdate)
      {
        manager.Cancel(pendingIntent);
        myIntent = new Intent(context, typeof(AlarmNotificationReceiver));
        pendingIntent = PendingIntent.GetBroadcast(context, 0, myIntent, 0);
        Java.Util.Calendar calendar = Java.Util.Calendar.Instance;
        calendar.Set(Java.Util.CalendarField.HourOfDay, hour);
        calendar.Set(Java.Util.CalendarField.Minute, minute);
        manager.SetRepeating(AlarmType.RtcWakeup, calendar.TimeInMillis, AlarmManager.IntervalDay, pendingIntent);
        savedSettingsEdit.PutBoolean("alarmNeedsUpdate", false);
        savedSettingsEdit.Commit();
      }
    }
  }
}