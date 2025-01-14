using Android.App;
using Android.Content;
using Android.Hardware.Usb;
using Android.Media.TV;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using FiscalPrinterSDK;
using Java.Util;
using SalesDemo_DevGr_A.ListView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using static Android.Bluetooth.BluetoothClass;
using static Android.Content.ClipData;

namespace SalesDemo_DevGr_A
{
    public class UsbConnectionFragment : Android.Support.V4.App.Fragment
    {
        private View fView;
        public UsbDevice SelectedDevice;
        public static int DATECS_USB_VID = 65520;
        public static int FTDI_USB_VID = 1027;
        private PendingIntent mPermissionIntent;
        private static string ACTION_USB_PERMISSION = "com.android.example.USB_PERMISSION";
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            mPermissionIntent = PendingIntent.GetBroadcast(Context, 0, new Intent(ACTION_USB_PERMISSION), PendingIntentFlags.Immutable);
            // Create your fragment here
        }

        public static UsbConnectionFragment NewInstance()
        {
            var detailsFrag = new UsbConnectionFragment { Arguments = new Bundle() };
            return detailsFrag;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            if (container == null)
            {
                // Currently in a layout without a container, so no reason to create our view.
                return null;
            }

            // Use this to return your custom view for this Fragment
            fView = inflater.Inflate(Resource.Layout.UsbConnection, container, false);
            if (MainActivity.fiscal != null)
            {
                fView.FindViewById<Button>(Resource.Id.btnConnectUSB).Enabled = !MainActivity.fiscal.device_Connected;
                fView.FindViewById<Button>(Resource.Id.btnDisconnectUSB).Enabled = MainActivity.fiscal.device_Connected;
            }
            else
            {
                fView.FindViewById<Button>(Resource.Id.btnConnectUSB).Enabled = true;
                fView.FindViewById<Button>(Resource.Id.btnDisconnectUSB).Enabled = false;
            }
            fView.FindViewById<Button>(Resource.Id.btnConnectUSB).Click += btnConnectUSB_Click;
            fView.FindViewById<Button>(Resource.Id.btnDisconnectUSB).Click += btnDisconnectUSB_Click;
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);
            var lst = fView.FindViewById<Android.Widget.ListView>(Resource.Id.listUsb);
            lst.ChoiceMode = Android.Widget.ChoiceMode.Single;
            lst.ItemSelected += Lst_ItemSelected;
            lst.ItemClick += Lst_ItemClick;

            UsbManager manager = (UsbManager)Context.GetSystemService(Context.UsbService);
            if (manager != null)
            {
                Dictionary<string, UsbDevice> deviceList = new Dictionary<string, UsbDevice>(manager.DeviceList);
                var items = new List<IListItem>();
                foreach (var d in deviceList.Values)

                {
                    UsbDevice device = d;

                    if ((device.VendorId == DATECS_USB_VID) || (device.VendorId == FTDI_USB_VID))
                    {
                        manager.RequestPermission(device, mPermissionIntent);
                        items.Add(new DataItem(d.ProductName,d.ManufacturerName,null,d));

                    }
                }
                if (deviceList.Count > 0)
                {
                    lst.Adapter = new ListViewAdapter(Activity, items);
                    lst.SetItemChecked(0, true);
                    SelectedDevice = (items[0] as DataItem).UsbDev;
                }
                else
                {
                    fView.FindViewById<Button>(Resource.Id.btnConnectUSB).Enabled = false;
                    fView.FindViewById<Button>(Resource.Id.btnDisconnectUSB).Enabled = false;
                    MainActivity.ShowMessage("There is no devices plugged via USB!", "Warning");
                }

            }

                return fView;
        }

        private void Lst_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            if (Activity != null)
            {
                var lst = Activity.FindViewById<Android.Widget.ListView>(Resource.Id.listUsb);

                var adapter = lst.Adapter as ListViewAdapter;
                if (e.Position < 0) throw new Exception("USB device is not seleced.");
                if (adapter.Items[e.Position].GetListItemType() == ListItemType.DataItem)
                {
                    SelectedDevice = (adapter.Items[e.Position] as DataItem).UsbDev;

                }
            }
        }

        private void Lst_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            if (Activity != null)
            {
                var lst = Activity.FindViewById<Android.Widget.ListView>(Resource.Id.listUsb);
                var adapter = lst.Adapter as ListViewAdapter;
                if (e.Position < 0) throw new Exception("USB device is not seleced.");
                if (adapter.Items[e.Position].GetListItemType() == ListItemType.DataItem)
                    SelectedDevice = (adapter.Items[e.Position] as DataItem).UsbDev;
            }
        }

        private void btnConnectUSB_Click(object sender, EventArgs e)
        {
            fView.FindViewById<ProgressBar>(Resource.Id.progressBar1).Visibility = ViewStates.Visible;
            fView.FindViewById<Button>(Resource.Id.btnConnectUSB).Enabled = false;
            ThreadPool.QueueUserWorkItem(delegate
            {
               
                    FiscalCommAndroid_Usb fiscalComm = new FiscalCommAndroid_Usb(Context, SelectedDevice);
                MainActivity.fiscal = new FDGROUP_A_BGR(fiscalComm);
                MainActivity.fiscal.Connect();
                this.Activity.RunOnUiThread(() => // BeginInvoke 
                {
                    if(MainActivity.fiscal.device_Connected) fView.FindViewById<ProgressBar>(Resource.Id.progressBar1).Visibility = ViewStates.Invisible;
                     fView.FindViewById<Button>(Resource.Id.btnConnectUSB).Enabled =!MainActivity.fiscal.device_Connected;
                    fView.FindViewById<Button>(Resource.Id.btnDisconnectUSB).Enabled = MainActivity.fiscal.device_Connected;
                });
            });


        }
        private void btnDisconnectUSB_Click(object sender, EventArgs e)
        {
            ThreadPool.QueueUserWorkItem(delegate
            {
               
                MainActivity.fiscal.Disconnect();
                this.Activity.RunOnUiThread(() => // BeginInvoke 
                {
                    fView.FindViewById<Button>(Resource.Id.btnConnectUSB).Enabled = !MainActivity.fiscal.device_Connected;
                    fView.FindViewById<Button>(Resource.Id.btnDisconnectUSB).Enabled = MainActivity.fiscal.device_Connected;
                });
            });
        }
    }
}