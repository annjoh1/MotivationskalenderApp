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
using Android.Support.V7.App;
using TaskStackBuilder = Android.Support.V4.App.TaskStackBuilder;
using Motivationskalender.BroadCast;
using Android.Icu.Util;

namespace Motivationskalender.BroadCast
{
    [BroadcastReceiver(Enabled = true)]
    public class AlarmNotificationReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            // When the user clicks the notification, MainActivity will start up.
            Intent resultIntent = new Intent(context, typeof(MainActivity));

            // Construct a back stack for cross-task navigation:
            TaskStackBuilder stackBuilder = TaskStackBuilder.Create(context);
            stackBuilder.AddParentStack(Java.Lang.Class.FromType(typeof(MainActivity)));
            stackBuilder.AddNextIntent(resultIntent);

            // Create the PendingIntent with the back stack:            
            PendingIntent resultPendingIntent =
                stackBuilder.GetPendingIntent(0, (int)PendingIntentFlags.UpdateCurrent);


            DateTime thisDay = DateTime.Today;
            bool lastDayOfMonth = (thisDay.Day == DateTime.DaysInMonth(thisDay.Year, thisDay.Month));
            string notificationText;
            notificationText = "Fyll i!";
            if (lastDayOfMonth)
            {
                notificationText = "Fyll i och skicka iväg!";
            }

            NotificationCompat.Builder builder = new NotificationCompat.Builder(context, "reminder_notification");
            builder.SetAutoCancel(true)
              .SetDefaults((int)NotificationDefaults.All)
              .SetContentIntent(resultPendingIntent)
              .SetSmallIcon(Resource.Drawable.Icon)
              .SetContentTitle("Motivationskalender")
              .SetSmallIcon(Resource.Drawable.ic_launcher)
              .SetContentText(notificationText);

            NotificationManager manager = (NotificationManager)context.GetSystemService(Context.NotificationService);     
            manager.Notify(1, builder.Build());
        }
    }
}