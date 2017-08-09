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
      ImageButton closeImageButton = FindViewById<ImageButton>(Resource.Id.closeImageButton);
      TimePicker timePicker = FindViewById<TimePicker>(Resource.Id.timePicker);
      timePicker.SetIs24HourView(Java.Lang.Boolean.True);
      var savedSettings = Application.Context.GetSharedPreferences("SavedSettings", FileCreationMode.Private);
      var savedSettingsEdit = savedSettings.Edit();

      int hour = savedSettings.GetInt("hour", 20);
      int minute = savedSettings.GetInt("minute", 30);
      string mail = savedSettings.GetString("mail", "googlemail@gmail.com");
      // Create your application here

      mailEditText.Text = mail;
      timePicker.Hour = hour;
      timePicker.Minute = minute;

      timePicker.TimeChanged += (s, e) =>
      {
        hour = e.HourOfDay;
        minute = e.Minute;
        savedSettingsEdit.PutInt("hour", hour);
        savedSettingsEdit.PutInt("minute", minute);
        savedSettingsEdit.Commit();
      };

      closeImageButton.Click += delegate
      {
        mail = mailEditText.Text;
        savedSettingsEdit.PutString("mail", mail);
        savedSettingsEdit.Commit();
        this.Finish();
      };
    }
  }
}