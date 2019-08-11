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
using Microcharts;
using Microcharts.Droid;
using SkiaSharp;

namespace Motivationskalender
{
    [Activity(Label = "Statistik")]
    public class StatisticsActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.StatisticsActivity);


            var entries = new[]
            {
                new Entry(200)
                {
                    Label = "January",
                    ValueLabel = "200",
                    Color = SKColor.Parse("#266489")
                },
                new Entry(400)
                {
                    Label = "February",
                    ValueLabel = "400",
                    Color = SKColor.Parse("#68B9C0")
                },
                new Entry(-100)
                {
                    Label = "March",
                    ValueLabel = "-100",
                    Color = SKColor.Parse("#90D585")
                 }
            };
            var chart = new LineChart() { Entries = entries };
            // or: var chart = new PointChart() { Entries = entries };
            // or: var chart = new LineChart() { Entries = entries };
            // or: var chart = new DonutChart() { Entries = entries };
            // or: var chart = new RadialGaugeChart() { Entries = entries };
            // or: var chart = new RadarChart() { Entries = entries };

            var chartView = FindViewById<ChartView>(Resource.Id.chartView);
            chartView.Chart = chart;
        }
    }
}