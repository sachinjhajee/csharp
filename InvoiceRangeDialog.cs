using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SalesDemo_DevGr_A
{
    class InvoiceRangeDialog : Dialog
    {
        public delegate void ExecuteInvoiceEventHandler(string startIntv, string endIntv);
        public event ExecuteInvoiceEventHandler OnExecuteInvoice;
        public string startInterval { get; set; }
        public string endInterval { get; set; }
        string stDef = "";
        public string startDefaultVal
        {
            get
            {
                return stDef;
            }
            set
            {
                stDef = value;
                FindViewById<EditText>(Resource.Id.txtFirstNumber).Text = stDef;
                FindViewById<EditText>(Resource.Id.txtEndNumber).Text = (int.Parse(stDef) + 10).ToString();
            }
        }
        public InvoiceRangeDialog(Activity activity) : base(activity)
        {
            
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            RequestWindowFeature((int)WindowFeatures.NoTitle);
            SetContentView(Resource.Layout.InvoiceRange);

            Button setInterval = (Button)FindViewById(Resource.Id.btnSetInterval);
            setInterval.Click += (e, a) =>
                {
                    startInterval = FindViewById<EditText>(Resource.Id.txtFirstNumber).Text;
                    endInterval = FindViewById<EditText>(Resource.Id.txtEndNumber).Text;
                    OnExecuteInvoice(startInterval, endInterval);
                    Dismiss();
                };

            EditText txtFirstNumber = FindViewById<EditText>(Resource.Id.txtFirstNumber);
            txtFirstNumber.TextChanged += TxtFirstNumber_TextChanged;
            EditText txtEndNumber = FindViewById<EditText>(Resource.Id.txtEndNumber);
            txtEndNumber.TextChanged += TxtEndNumber_TextChanged;

        }

        

        private void TxtEndNumber_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            endInterval = FindViewById<EditText>(Resource.Id.txtEndNumber).Text;
        }

        private void TxtFirstNumber_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            startInterval = FindViewById<EditText>(Resource.Id.txtFirstNumber).Text;
        }

    }

}