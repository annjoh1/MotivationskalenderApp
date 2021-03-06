﻿using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;
using System;
using Java.Text;
using Java.Sql;
using System.Collections.Generic;
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
    [Activity(Label = "Motivationskalender", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        public static bool sound;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            CreateNotificationChannel();
            Context context = ApplicationContext;
            SetContentView(Resource.Layout.Main);
            var calenderView = FindViewById<CalendarView>(Resource.Id.calendarView);
            var txtDisplay = FindViewById<TextView>(Resource.Id.txtDisplay);
            Button statisticsButton = FindViewById<Button>(Resource.Id.statisticsButton);
            ImageButton settingsButton = FindViewById<ImageButton>(Resource.Id.settingsButton);
            Button sumButton = FindViewById<Button>(Resource.Id.sumButton);
            Button closeButton = FindViewById<Button>(Resource.Id.closeButton);
            CheckBox workoutCheckbox = FindViewById<CheckBox>(Resource.Id.workoutCheckBox);
            CheckBox physicalTherapyCheckbox = FindViewById<CheckBox>(Resource.Id.physicalTherapyCheckBox);
            CheckBox vegoCheckbox = FindViewById<CheckBox>(Resource.Id.vegoCheckBox);
            CheckBox alcoholFreeCheckbox = FindViewById<CheckBox>(Resource.Id.alcoholFreeCheckBox);
            CheckBox fruitsCheckbox = FindViewById<CheckBox>(Resource.Id.fruitsCheckBox);
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
            int nbOfWeeks = 0;
            bool lastDayOfMonth = (day == DateTime.DaysInMonth(year, month));
            String selectedDate = sdf.Format(today);
            DateTime selectedDateTime = new DateTime(year, month, day, 0, 0, 0);
            List<string> compliments = new List<string>();
            AssetManager assets = this.Assets;
            using (StreamReader sr = new StreamReader(assets.Open("Compliments.txt")))
            {
                string compliment = sr.ReadLine();
                while (compliment != null)
                {
                    compliments.Add(compliment);
                    compliment = sr.ReadLine();
                }
            }

            Random rnd = new Random();
            MediaPlayer player;
            player = MediaPlayer.Create(this, Resource.Raw.achievement);

            var savedWorkoutMain = Application.Context.GetSharedPreferences("SavedWorkout", FileCreationMode.Private);
            var savedPhysicalTherapyMain = Application.Context.GetSharedPreferences("SavedPhysicalTherapy", FileCreationMode.Private);
            var savedVegoMain = Application.Context.GetSharedPreferences("SavedVego", FileCreationMode.Private);
            var savedAlcoholFreeMain = Application.Context.GetSharedPreferences("SavedAlcoholFree", FileCreationMode.Private);
            var savedFruitsMain = Application.Context.GetSharedPreferences("SavedFruits", FileCreationMode.Private);
            var savedLocksMain = Application.Context.GetSharedPreferences("SavedLocks", FileCreationMode.Private);
            var savedHealthMain = Application.Context.GetSharedPreferences("SavedHealth", FileCreationMode.Private);
            var savedSettings = Application.Context.GetSharedPreferences("SavedSettings", FileCreationMode.Private);
            var savedSettingsEdit = savedSettings.Edit();

            bool workoutBool = savedWorkoutMain.GetBoolean(selectedDate, false);
            bool physicalTherapyBool = savedPhysicalTherapyMain.GetBoolean(selectedDate, false);
            bool vegoBool = savedVegoMain.GetBoolean(selectedDate, false);
            bool alcoholFreeBool = savedAlcoholFreeMain.GetBoolean(selectedDate, false);
            bool fruitsBool = savedFruitsMain.GetBoolean(selectedDate, false);
            bool lockBool = savedLocksMain.GetBoolean(selectedDate, false);
            int health = savedHealthMain.GetInt(selectedDate, 0);
            string mail = savedSettings.GetString("mail", "");
            sound = savedSettings.GetBoolean("sound", true);

            int fruitWorth = 20;
            int physicalTherapyWorth = 20;
            int workoutWorth = 50;
            int vegoWorth = 50;
            int alcoWorth = 200;

            Alarm.CheckAlarm(this);

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
                healthSeekBar.Progress = health;
                healthTextView.Text = string.Format("Välmående: {0}", health);
            }

            healthSeekBar.ProgressChanged += (object sender, SeekBar.ProgressChangedEventArgs e) =>
            {
                if (e.FromUser)
                {
                    healthTextView.Text = string.Format("Välmående: {0}", e.Progress);
                    int healthValue = healthSeekBar.Progress;
                    var savedHealth = Application.Context.GetSharedPreferences("SavedHealth", FileCreationMode.Private);
                    var savedHealthEdit = savedHealth.Edit();
                    savedHealthEdit.PutInt(selectedDate, healthValue);
                    savedHealthEdit.Commit();
                    if (sound)
                        player.Start();
                }
            };

            workoutCheckbox.Click += delegate
            {
                bool workout = workoutCheckbox.Checked;
                showCompliment(workout);
                var savedWorkout = Application.Context.GetSharedPreferences("SavedWorkout", FileCreationMode.Private);
                var savedWorkoutEdit = savedWorkout.Edit();
                savedWorkoutEdit.PutBoolean(selectedDate, workout);
                savedWorkoutEdit.Commit();
            };

            physicalTherapyCheckbox.Click += delegate
            {
                bool physicalTherapy = physicalTherapyCheckbox.Checked;
                showCompliment(physicalTherapy);
                var savedPhysicalTherapy = Application.Context.GetSharedPreferences("SavedPhysicalTherapy", FileCreationMode.Private);
                var savedPhysicalTherapyEdit = savedPhysicalTherapy.Edit();
                savedPhysicalTherapyEdit.PutBoolean(selectedDate, physicalTherapy);
                savedPhysicalTherapyEdit.Commit();
            };

            vegoCheckbox.Click += delegate
            {
                bool vego = vegoCheckbox.Checked;
                showCompliment(vego);
                var savedVego = Application.Context.GetSharedPreferences("SavedVego", FileCreationMode.Private);
                var savedVegoEdit = savedVego.Edit();
                savedVegoEdit.PutBoolean(selectedDate, vego);
                savedVegoEdit.Commit();
            };

            alcoholFreeCheckbox.Click += delegate
            {
                bool alcoholFree = alcoholFreeCheckbox.Checked;
                showCompliment(alcoholFree);
                var savedAlcoholFree = Application.Context.GetSharedPreferences("SavedAlcoholFree", FileCreationMode.Private);
                var savedAlcoholFreeEdit = savedAlcoholFree.Edit();
                savedAlcoholFreeEdit.PutBoolean(selectedDate, alcoholFree);
                savedAlcoholFreeEdit.Commit();
            };

            fruitsCheckbox.Click += delegate
            {
                bool fruits = fruitsCheckbox.Checked;
                showCompliment(fruits);
                var savedFruits = Application.Context.GetSharedPreferences("SavedFruits", FileCreationMode.Private);
                var savedFruitsEdit = savedFruits.Edit();
                savedFruitsEdit.PutBoolean(selectedDate, fruits);
                savedFruitsEdit.Commit();
            };

            statisticsButton.Click += (sender, e) =>
            {
                //Bundle valuesSend = new Bundle();
                //valuesSend.PutString("sendContent","Testa notification");
                var intent = new Intent(this, typeof(StatisticsActivity));
                //intent.PutExtras(valuesSend);
                StartActivity(intent);
            };

            void CreateNotificationChannel()
            {

                if (Build.VERSION.SdkInt < BuildVersionCodes.O)
                {
                    // Notification channels are new in API 26 (and not a part of the
                    // support library). There is no need to create a notification
                    // channel on older versions of Android.
                    return;
                }


                var name = "Reminder";
                var description = "Reminder";
                var channel = new NotificationChannel("reminder_notification", name, NotificationImportance.Default)
                {
                    Description = description
                };
                var notificationManager = (NotificationManager)GetSystemService(NotificationService);
                notificationManager.CreateNotificationChannel(channel);

            }

            settingsButton.Click += (sender, e) =>
            {
                Bundle valuesSend = new Bundle();
                valuesSend.PutString("sendContent", "Testa notification");
                var intent = new Intent(this, typeof(SettingsActivity));
                intent.PutExtras(valuesSend);
                StartActivity(intent);
            };

            sumButton.Click += delegate
            {
                string msg = sum();
                AlertDialog.Builder alert = new AlertDialog.Builder(this);
                alert.SetTitle("Summering");
                alert.SetMessage(msg + "\n" + "Vill du skicka iväg summeringen?");
                alert.SetIcon(Resource.Drawable.Calculator40);
                alert.SetPositiveButton("Ja", (senderAlert, args) =>
                {
                    mail = savedSettings.GetString("mail", "");
                    var email = new Intent(Android.Content.Intent.ActionSend);
                    email.PutExtra(Android.Content.Intent.ExtraEmail,
              new string[] { mail });
              //email.PutExtra(Android.Content.Intent.ExtraCc,
              //new string[] { "person3@xamarin.com" });
              email.PutExtra(Android.Content.Intent.ExtraSubject, "Resultat Motivationskalender " + selectedDate);
                    email.PutExtra(Android.Content.Intent.ExtraText, msg);
                    email.SetType("message/rfc822");
                    StartActivity(email);
                });

                alert.SetNegativeButton("Avbryt", (senderAlert, args) =>
                {
                    Toast.MakeText(this, "Ej skickad", ToastLength.Short).Show();
                });

                Dialog dialog = alert.Create();
                dialog.Show();
            };

            closeButton.Click += delegate
            {
                this.FinishAffinity();
            };

            void showCompliment(bool show)
            {
                if (show)
                {
                    int index = rnd.Next(compliments.Count);
                    Toast.MakeText(this, compliments[index], ToastLength.Short).Show();
                    if (sound)
                        player.Start();
                }
            };

            string sum()
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
                int alcoMoney = getAlcoholFreeMoney();
                money = workoutCtr * workoutWorth + physTherCtr * physicalTherapyWorth + vegoCtr * vegoWorth + fruitCtr * fruitWorth + alcoMoney;

                int getAlcoholFreeMoney()
                {
                    nbOfWeeks = 0;
                    if (alcoCtr == DateTime.DaysInMonth(year, month)) nbOfWeeks = 5;
                    else
                    {
                        int ctr = 0;
                        foreach (DateTime date in AllDatesInMonth(year, month))
                        {
                            dateString = date.ToString("d'/'M'/'yyyy");
                            if (savedAlcoholFreeMain.GetBoolean(dateString, false)) ctr += 1;
                            else ctr = 0;
                            if (ctr == 7)
                            {
                                nbOfWeeks += 1;
                                ctr = 0;
                            }
                        }
                    }
                    return alcoWorth * nbOfWeeks;
                }

                string result = $"Träningspass: {workoutCtr} st ({workoutCtr * workoutWorth} kr)  \n" +
                                $"Sjukgymn.: {physTherCtr} st ({physTherCtr * physicalTherapyWorth} kr)\n" +
                                $"Köttfri: {vegoCtr} st ({vegoCtr * vegoWorth} kr)\n" +
                                $"Alkoholfri: {alcoCtr}d / {nbOfWeeks}v ({alcoMoney} kr)\n" +
                                $"5 frukt och grönt: {fruitCtr} st ({fruitCtr * fruitWorth} kr)\n" +
                                $"Genomsnittligt välmående: {healthCtr / DateTime.DaysInMonth(year, month)}\n" +
                                $"Summa: {money} kr";
                return result;
            }
        }


        public static void ToggleSound(ImageButton button)
        {
            var savedSettings = Application.Context.GetSharedPreferences("SavedSettings", FileCreationMode.Private);
            var savedSettingsEdit = savedSettings.Edit();
            sound = savedSettings.GetBoolean("sound", true);
            if (sound)
            {
                button.SetImageResource(Resource.Drawable.NoAudio30);
                sound = false;
            }
            else
            {
                button.SetImageResource(Resource.Drawable.Audio30);
                sound = true;
            }
            savedSettingsEdit.PutBoolean("sound", sound);
            savedSettingsEdit.Commit();
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

