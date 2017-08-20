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

namespace Motivationskalender
{
  [Activity(Label = "Inställningar")]
  public class SettingsActivity : Activity
  {
    protected override void OnCreate(Bundle savedInstanceState)
    {
      base.OnCreate(savedInstanceState);
      SetContentView(Resource.Layout.SettingsActivity);
      EditText mailEditText = FindViewById<EditText>(Resource.Id.mailEditText);
      ImageButton soundButton = FindViewById<ImageButton>(Resource.Id.soundButton);
      ImageButton closeImageButton = FindViewById<ImageButton>(Resource.Id.closeImageButton);
      TimePicker timePicker = FindViewById<TimePicker>(Resource.Id.timePicker);
      timePicker.SetIs24HourView(Java.Lang.Boolean.True);
      var savedSettings = Application.Context.GetSharedPreferences("SavedSettings", FileCreationMode.Private);
      var savedSettingsEdit = savedSettings.Edit();

      int hour = savedSettings.GetInt("hour", 20);
      int minute = savedSettings.GetInt("minute", 30);
      string mail = savedSettings.GetString("mail", "");
      bool alarmNeedsUpdate = savedSettings.GetBoolean("alarmNeedsUpdate", false);
      bool sound = savedSettings.GetBoolean("sound", true);
      if (sound)
      {
        soundButton.SetImageResource(Resource.Drawable.Audio30);
      }
      else
      {
        soundButton.SetImageResource(Resource.Drawable.NoAudio30);
      }

      mailEditText.Text = mail;
      timePicker.Hour = hour;
      timePicker.Minute = minute;

      timePicker.TimeChanged += (s, e) =>
      {
        hour = e.HourOfDay;
        minute = e.Minute;
        savedSettingsEdit.PutInt("hour", hour);
        savedSettingsEdit.PutInt("minute", minute);
        savedSettingsEdit.PutBoolean("alarmNeedsUpdate", true);
        savedSettingsEdit.Commit();
      };

      soundButton.Click += delegate 
      {
        MainActivity.ToggleSound(soundButton);
      };

      closeImageButton.Click += delegate
      {
        mail = mailEditText.Text;
        savedSettingsEdit.PutString("mail", mail);
        savedSettingsEdit.Commit();
        Context temp = new Motivationskalender.MainActivity();
        Alarm.CheckAlarm(this);
        this.Finish();
      };
    }
  }
}