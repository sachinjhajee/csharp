using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Threading;
using Android;
using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using FiscalPrinterSDK;
using Java.Util;
using SalesDemo_DevGr_A.ListView;

namespace SalesDemo_DevGr_A
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true, ScreenOrientation = ScreenOrientation.Portrait)]
    
    public class MainActivity : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener
    {
        public static FiscalPrinterSDK.FDGROUP_A_BGR fiscal;
        private const int LocationPermissionsRequestCode = 1000;
        private static readonly string[] LocationPermissions =
        {
            Manifest.Permission.AccessCoarseLocation,
            Manifest.Permission.AccessFineLocation
        };

        private static MainActivity _instance;
        
        public static MainActivity GetInstance()
        {
            return _instance;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            _instance = this;
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);


            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            Android.Support.V7.App.ActionBarDrawerToggle toggle = new Android.Support.V7.App.ActionBarDrawerToggle(this, drawer, toolbar, Resource.String.navigation_drawer_open, Resource.String.navigation_drawer_close);
            drawer.AddDrawerListener(toggle);
            toggle.SyncState();
            
            NavigationView navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            navigationView.SetNavigationItemSelectedListener(this);

            NavigateTo(itemID);
            var coarseLocationPermissionGranted =
                ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessCoarseLocation);
            var fineLocationPermissionGranted =
                ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation);

            if (coarseLocationPermissionGranted != Permission.Denied ||
                fineLocationPermissionGranted == Permission.Denied)
                ActivityCompat.RequestPermissions(this, LocationPermissions, LocationPermissionsRequestCode);

        }

        public static void ShowMessage(string msg, string title)
        {
                new Android.App.AlertDialog.Builder(GetInstance())
                    .SetPositiveButton("OK", (sender, args) =>
                    {
                        return;
                    })
                    .SetMessage(msg)
                    .SetTitle(title)
                    .Show();
        }

        public override void OnBackPressed()
        {
            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            if(drawer.IsDrawerOpen(GravityCompat.Start))
            {
                drawer.CloseDrawer(GravityCompat.Start);
            }
            else
            {
                base.OnBackPressed();
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

       

        //private void FabOnClick(object sender, EventArgs eventArgs)
        //{
        //    View view = (View) sender;
        //    Snackbar.Make(view, "Replace with your own action", Snackbar.LengthLong)
        //        .SetAction("Action", (Android.Views.View.IOnClickListener)null).Show();
        //}

        private void ExecuteFragmentNotConnected()
        {
            var notConnectionf = SupportFragmentManager.FindFragmentById(Resource.Id.linearLayoutNotConnected) as DeviceNotConnectedFragment;
            if (notConnectionf == null)
            {
                // Make new fragment to show this selection.
                var newfr = DeviceNotConnectedFragment.NewInstance();

                // Execute a transaction, replacing any existing
                // fragment with this one inside the frame.
                var ft = SupportFragmentManager.BeginTransaction();
                ft.Add(Resource.Id.fragment_container, new DeviceNotConnectedFragment());
                ft.Replace(Resource.Id.fragment_container, newfr);
                //ft.SetTransition((int)FragmentTransit.FragmentFade);
                ft.Commit();
                ft.Show(new DeviceNotConnectedFragment());
            }
        }
        private static int itemID = 0;

        private void NavigateTo(int id)
        {
            if (id == Resource.Id.nav_connection || id == 0)
            {
                var myConnection = SupportFragmentManager.FindFragmentById(Resource.Id.linearLayoutConnection) as ConnectionFragment;
                if (myConnection == null)
                {
                    // Make new fragment to show this selection.
                    var newfr = ConnectionFragment.NewInstance();

                    // Execute a transaction, replacing any existing
                    // fragment with this one inside the frame.
                    var ft = SupportFragmentManager.BeginTransaction();
                    ft.Add(Resource.Id.fragment_container, new ConnectionFragment());
                    ft.Replace(Resource.Id.fragment_container, newfr);
                    //ft.SetTransition((int)FragmentTransit.FragmentFade);
                    ft.Commit();
                    ft.Show(new ConnectionFragment());
                }

            }
            else if (id == Resource.Id.nav_usb_connection )
            {
                var myConnection = SupportFragmentManager.FindFragmentById(Resource.Id.linearLayoutUsbConnection) as UsbConnectionFragment;
                if (myConnection == null)
                {
                    // Make new fragment to show this selection.
                    var newfr = UsbConnectionFragment.NewInstance();

                    // Execute a transaction, replacing any existing
                    // fragment with this one inside the frame.
                    var ft = SupportFragmentManager.BeginTransaction();
                    ft.Add(Resource.Id.fragment_container, new UsbConnectionFragment());
                    ft.Replace(Resource.Id.fragment_container, newfr);
                    //ft.SetTransition((int)FragmentTransit.FragmentFade);
                    ft.Commit();
                    ft.Show(new UsbConnectionFragment());
                }

            }

            else if (id == Resource.Id.nav_sales)
            {
                if (fiscal == null)
                {
                    ExecuteFragmentNotConnected();
                    return;
                }
                if (!fiscal.device_Connected)
                {
                    ExecuteFragmentNotConnected();
                    return;
                }
                var myReceipts = SupportFragmentManager.FindFragmentById(Resource.Id.linearLayoutrec) as ReceiptsReportsFragment;
                if (myReceipts == null)
                {
                    // Make new fragment to show this selection.
                    var newfr = ReceiptsReportsFragment.NewInstance();

                    // Execute a transaction, replacing any existing
                    // fragment with this one inside the frame.
                    var ft = SupportFragmentManager.BeginTransaction();

                    ft.Add(Resource.Id.fragment_container, new ReceiptsReportsFragment());
                    ft.Replace(Resource.Id.fragment_container, newfr);
                    //ft.SetTransition((int)FragmentTransit.FragmentFade);
                    ft.Commit();
                    ft.Show(new ReceiptsReportsFragment());
                }
            }

            else if (id == Resource.Id.nav_storno)
            {
                if (fiscal == null)
                {
                    ExecuteFragmentNotConnected();
                    return;
                }
                if (!fiscal.device_Connected)
                {
                    ExecuteFragmentNotConnected();
                    return;
                }

                var myStorno = SupportFragmentManager.FindFragmentById(Resource.Id.relativeLayoutStorno) as StornoFragment;
                if (myStorno == null)
                {
                    // Make new fragment to show this selection.
                    var newfr = StornoFragment.NewInstance();

                    // Execute a transaction, replacing any existing
                    // fragment with this one inside the frame.
                    var ft = SupportFragmentManager.BeginTransaction();

                    ft.Add(Resource.Id.fragment_container, new StornoFragment());
                    ft.Replace(Resource.Id.fragment_container, newfr);
                    //ft.SetTransition((int)FragmentTransit.FragmentFade);
                    ft.Commit();
                    ft.Show(new StornoFragment());
                }
            }
            else if (id == Resource.Id.nav_statuses)
            {
                if (fiscal == null)
                {
                    ExecuteFragmentNotConnected();
                    return;
                }
                if (!fiscal.device_Connected)
                {
                    ExecuteFragmentNotConnected();
                    return;
                }
                var myStatuses = SupportFragmentManager.FindFragmentById(Resource.Id.linearLayoutStatuses) as StatusBytesFragment;
                if (myStatuses == null)
                {
                    // Make new fragment to show this selection.
                    var newfr = StatusBytesFragment.NewInstance();

                    // Execute a transaction, replacing any existing
                    // fragment with this one inside the frame.
                    var ft = SupportFragmentManager.BeginTransaction();

                    ft.Add(Resource.Id.fragment_container, new StatusBytesFragment());
                    ft.Replace(Resource.Id.fragment_container, newfr);
                    //ft.SetTransition((int)FragmentTransit.FragmentFade);
                    ft.Commit();
                    ft.Show(new StatusBytesFragment());
                }
            }
        }

        public bool OnNavigationItemSelected(IMenuItem item)
        {
            int id = item.ItemId;

            NavigateTo(id);

            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            drawer.CloseDrawer(GravityCompat.Start);
            itemID = id;
            return true;
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}

