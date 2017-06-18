using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;
using System;
using Java.Text;
using Java.Sql;
using System.Collections.Generic;
using System.Net.Mail;

namespace Motivationskalender
{
  [Activity(Label = "Motivationskalender", MainLauncher = true, Icon = "@drawable/icon")]
  public class MainActivity : Activity
  {
    protected override void OnCreate(Bundle bundle)
    {
      base.OnCreate(bundle);

      // Set our view from the "main" layout resource
      SetContentView (Resource.Layout.Main);
      var calenderView = FindViewById<CalendarView>(Resource.Id.calendarView);
      var txtDisplay = FindViewById<TextView>(Resource.Id.txtDisplay);
      Button saveButton = FindViewById<Button>(Resource.Id.saveButton);
      CheckBox workoutCheckbox = FindViewById<CheckBox>(Resource.Id.workoutCheckBox);
      CheckBox physicalTherapyCheckbox = FindViewById<CheckBox>(Resource.Id.physicalTherapyCheckBox);
      CheckBox vegoCheckbox = FindViewById<CheckBox>(Resource.Id.vegoCheckBox);
      CheckBox alcoholFreeCheckbox = FindViewById<CheckBox>(Resource.Id.alcoholFreeCheckBox);
      CheckBox fruitsCheckbox = FindViewById<CheckBox>(Resource.Id.fruitsCheckBox);
      Switch lockSwitch = FindViewById<Switch>(Resource.Id.lockSwitch);
      TextView healthTextView = FindViewById<TextView>(Resource.Id.healthTextView);
      SeekBar healthSeekBar = FindViewById<SeekBar>(Resource.Id.healthSeekBar);
      SimpleDateFormat sdf = new SimpleDateFormat("d/M/yyyy");
      var toDay = new SimpleDateFormat("d");
      var toMonth = new SimpleDateFormat("M");
      var toYear = new SimpleDateFormat("yyyy");
      Date today = new Date(calenderView.Date);
      int year = Int32.Parse(toYear.Format(today));
      int month = Int32.Parse(toMonth.Format(today));
      int day = Int32.Parse(toDay.Format(today));
      checkButtonVisibility(year, month, day);
      String selectedDate = sdf.Format(today);
      DateTime selectedDateTime = new DateTime(year, month, day, 0, 0, 0);

      var savedWorkoutMain = Application.Context.GetSharedPreferences("SavedWorkout", FileCreationMode.Private);
      var savedPhysicalTherapyMain = Application.Context.GetSharedPreferences("SavedPhysicalTherapy", FileCreationMode.Private);
      var savedVegoMain = Application.Context.GetSharedPreferences("SavedVego", FileCreationMode.Private);
      var savedAlcoholFreeMain = Application.Context.GetSharedPreferences("SavedAlcoholFree", FileCreationMode.Private);
      var savedFruitsMain = Application.Context.GetSharedPreferences("SavedFruits", FileCreationMode.Private);
      var savedLocksMain = Application.Context.GetSharedPreferences("SavedLocks", FileCreationMode.Private);
      var savedHealthMain = Application.Context.GetSharedPreferences("SavedHealth", FileCreationMode.Private);

      bool workoutBool = savedWorkoutMain.GetBoolean(selectedDate, false);
      bool physicalTherapyBool = savedPhysicalTherapyMain.GetBoolean(selectedDate, false);
      bool vegoBool = savedVegoMain.GetBoolean(selectedDate, false);
      bool alcoholFreeBool = savedAlcoholFreeMain.GetBoolean(selectedDate, false);
      bool fruitsBool = savedFruitsMain.GetBoolean(selectedDate, false);
      bool lockBool = savedLocksMain.GetBoolean(selectedDate, false);
      int health = savedHealthMain.GetInt(selectedDate, 0);
      updateViews();

      calenderView.DateChange += (s, e) =>
      {
        year = e.Year;
        month = e.Month + 1;
        day = e.DayOfMonth;
        updateViews();
      };

      void updateViews()
      {
        selectedDate = day.ToString() + "/" + month.ToString() + "/" + year.ToString();
        selectedDateTime = new DateTime(year, month, day, 0, 0, 0);
        checkButtonVisibility(year, month, day);

        txtDisplay.Text = selectedDate;
        workoutBool = savedWorkoutMain.GetBoolean(selectedDate, false);
        physicalTherapyBool = savedPhysicalTherapyMain.GetBoolean(selectedDate, false);
        vegoBool = savedVegoMain.GetBoolean(selectedDate, false);
        alcoholFreeBool = savedAlcoholFreeMain.GetBoolean(selectedDate, false);
        fruitsBool = savedFruitsMain.GetBoolean(selectedDate, false);
        lockBool = savedLocksMain.GetBoolean(selectedDate, false);
        health = savedHealthMain.GetInt(selectedDate, 5);

        workoutCheckbox.Checked = workoutBool ? true : false;
        physicalTherapyCheckbox.Checked = physicalTherapyBool ? true : false;
        vegoCheckbox.Checked = vegoBool ? true : false;
        alcoholFreeCheckbox.Checked = alcoholFreeBool ? true : false;
        fruitsCheckbox.Checked = fruitsBool ? true : false;
        lockSwitch.Checked = lockBool ? true : false;
        healthSeekBar.Progress = health;
        healthTextView.Text = string.Format("Välmående: {0}", health);
        bool locks = lockSwitch.Checked;
        if (locks)
        {
          workoutCheckbox.Enabled = false;
          physicalTherapyCheckbox.Enabled = false;
          vegoCheckbox.Enabled = false;
          alcoholFreeCheckbox.Enabled = false;
          fruitsCheckbox.Enabled = false;
          healthSeekBar.Enabled = false;
        }
        else
        {
          workoutCheckbox.Enabled = true;
          physicalTherapyCheckbox.Enabled = true;
          vegoCheckbox.Enabled = true;
          alcoholFreeCheckbox.Enabled = true;
          fruitsCheckbox.Enabled = true;
          healthSeekBar.Enabled = true;
        }
      }

      healthSeekBar.ProgressChanged += (object sender, SeekBar.ProgressChangedEventArgs e) => {
        if (e.FromUser)
        {
          healthTextView.Text = string.Format("Välmående: {0}", e.Progress);
          int healthValue = healthSeekBar.Progress;
          var savedHealth = Application.Context.GetSharedPreferences("SavedHealth", FileCreationMode.Private);
          var savedHealthEdit = savedHealth.Edit();
          savedHealthEdit.PutInt(selectedDate, healthValue);
          savedHealthEdit.Commit();
        }
      };

      workoutCheckbox.Click += delegate
      {
        bool workout = workoutCheckbox.Checked;
        var savedWorkout = Application.Context.GetSharedPreferences("SavedWorkout", FileCreationMode.Private);
        var savedWorkoutEdit = savedWorkout.Edit();
        savedWorkoutEdit.PutBoolean(selectedDate, workout);
        savedWorkoutEdit.Commit();
      };

      physicalTherapyCheckbox.Click += delegate
      {
        bool physicalTherapy = physicalTherapyCheckbox.Checked;
        var savedPhysicalTherapy = Application.Context.GetSharedPreferences("SavedPhysicalTherapy", FileCreationMode.Private);
        var savedPhysicalTherapyEdit = savedPhysicalTherapy.Edit();
        savedPhysicalTherapyEdit.PutBoolean(selectedDate, physicalTherapy);
        savedPhysicalTherapyEdit.Commit();
      };

      vegoCheckbox.Click += delegate
      {
        bool vego = vegoCheckbox.Checked;
        var savedVego = Application.Context.GetSharedPreferences("SavedVego", FileCreationMode.Private);
        var savedVegoEdit = savedVego.Edit();
        savedVegoEdit.PutBoolean(selectedDate, vego);
        savedVegoEdit.Commit();
      };

      alcoholFreeCheckbox.Click += delegate
      {
        bool alcoholFree = alcoholFreeCheckbox.Checked;
        var savedAlcoholFree = Application.Context.GetSharedPreferences("SavedAlcoholFree", FileCreationMode.Private);
        var savedAlcoholFreeEdit = savedAlcoholFree.Edit();
        savedAlcoholFreeEdit.PutBoolean(selectedDate, alcoholFree);
        savedAlcoholFreeEdit.Commit();
      };

      fruitsCheckbox.Click += delegate
      {
        bool fruits = fruitsCheckbox.Checked;
        var savedFruits = Application.Context.GetSharedPreferences("SavedFruits", FileCreationMode.Private);
        var savedFruitsEdit = savedFruits.Edit();
        savedFruitsEdit.PutBoolean(selectedDate, fruits);
        savedFruitsEdit.Commit();
      };

      lockSwitch.Click += delegate
      {
        bool locks = lockSwitch.Checked;
        var savedLocks = Application.Context.GetSharedPreferences("SavedLocks", FileCreationMode.Private);
        var savedLocksEdit = savedLocks.Edit();
        savedLocksEdit.PutBoolean(selectedDate, locks);
        savedLocksEdit.Commit();
        if (locks)
        {
          workoutCheckbox.Enabled = false;
          physicalTherapyCheckbox.Enabled = false;
          vegoCheckbox.Enabled = false;
          alcoholFreeCheckbox.Enabled = false;
          fruitsCheckbox.Enabled = false;
          healthSeekBar.Enabled = false;
        }
        else
        {
          workoutCheckbox.Enabled = true;
          physicalTherapyCheckbox.Enabled = true;
          vegoCheckbox.Enabled = true;
          alcoholFreeCheckbox.Enabled = true;
          fruitsCheckbox.Enabled = true;
          healthSeekBar.Enabled = true;
        }
      };

      saveButton.Click += delegate
      {
        string dateString;
        int workoutCtr = 0;
        int physTherCtr = 0;
        int vegoCtr = 0;
        int alcoCtr = 0;
        int fruitCtr = 0;
        int healthCtr = 0;
        int money = 0;
        foreach (DateTime date in AllDatesInMonth(year, month))
        {
          dateString = date.ToString("d'/'M'/'yyyy");
          if (savedWorkoutMain.GetBoolean(dateString, false)) workoutCtr += 1;
          if (savedPhysicalTherapyMain.GetBoolean(dateString, false)) physTherCtr += 1;
          if (savedVegoMain.GetBoolean(dateString, false)) vegoCtr += 1;
          if (savedAlcoholFreeMain.GetBoolean(dateString, false)) alcoCtr += 1;
          if (savedFruitsMain.GetBoolean(dateString, false)) fruitCtr += 1;
          healthCtr += savedHealthMain.GetInt(dateString, 5);
        }
        money = workoutCtr * 50 + physTherCtr * 20 + vegoCtr * 50 + fruitCtr * 20 + getAlcoholFreeMoney();

        int getAlcoholFreeMoney()
        {
          if (alcoCtr == DateTime.DaysInMonth(year, month)) return 1000;
          else
          {
            int nbOfWeeks = 0;
            var firstDayOfMonth = new DateTime(year, month, 1);
            var dayOfWeek = firstDayOfMonth.DayOfWeek;
            if (dayOfWeek == DayOfWeek.Monday)
            {
              //foreach (DateTime date in AllDatesInMonth(year, month))
              //{
              //  int ctr = 0;
              //  if (savedAlcoholFreeMain.GetBoolean(dateString, false)) ctr += 1;

              //}
            }
            return 200*nbOfWeeks;
          }
        }

        string result = $"Träningspass: {workoutCtr} \nSjukgymnastik pass: {physTherCtr} \n" +
                         $"Köttfritt: {vegoCtr} \nAlkoholfritt: {alcoCtr} \n" +
                         $"5 frukt och grönt: {fruitCtr} \nGenomsnittligt välmående: {healthCtr / DateTime.DaysInMonth(year, month)}";

        AlertDialog.Builder alert = new AlertDialog.Builder(this);
        alert.SetTitle("Summering");
        alert.SetMessage(result + "\n" + "Vill du skicka iväg summeringen?");
        

        
        alert.SetPositiveButton("Ja", (senderAlert, args) => {
          var email = new Intent(Android.Content.Intent.ActionSend);
          email.PutExtra(Android.Content.Intent.ExtraEmail,
          new string[] { "annjohansson87@gmail.com" });
          //email.PutExtra(Android.Content.Intent.ExtraCc,
          //new string[] { "person3@xamarin.com" });
          email.PutExtra(Android.Content.Intent.ExtraSubject, "Resultat Motivationskalender " + selectedDate);
          email.PutExtra(Android.Content.Intent.ExtraText, result);
          email.SetType("message/rfc822");
          StartActivity(email);
        });

        alert.SetNegativeButton("Abryt", (senderAlert, args) => {
          Toast.MakeText(this, "Ej skickad", ToastLength.Short).Show();
        });


        Dialog dialog = alert.Create();
        dialog.Show();
      };

      void checkButtonVisibility(int y, int m, int d) {
        if (d == DateTime.DaysInMonth(y, m))
          saveButton.Visibility = Android.Views.ViewStates.Visible;
        else
          saveButton.Visibility = Android.Views.ViewStates.Invisible;
      }
    }

    public static IEnumerable<DateTime> AllDatesInMonth(int year, int month)
    {
      int days = DateTime.DaysInMonth(year, month);
      for (int day = 1; day <= days; day++)
      {
        yield return new DateTime(year, month, day);
      }
    }    
  }
}

