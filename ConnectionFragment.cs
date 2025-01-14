using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Support.Design.Widget;
using Android.Support.V4.Content;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using FiscalPrinterSDK;
using Java.Util;
using SalesDemo_DevGr_A.ListView;
using System.Threading;
using Android.Bluetooth;
using Android;
using Android.Content.PM;
using Android.Hardware.Usb;
using static Android.Bluetooth.BluetoothClass;

namespace SalesDemo_DevGr_A
{
    public class ConnectionFragment : Android.Support.V4.App.Fragment
    {
       
        Button button;
        private View fView;
        static BluetoothDevice SelectedDevice = null;
        private const int LocationPermissionsRequestCode = 1000;

        private static BluetoothAdapter bluetoothAdapter;
        private int REQUEST_ENABLE_BT = 10;

        private static readonly string[] LocationPermissions =
        {
            Manifest.Permission.AccessCoarseLocation,
            Manifest.Permission.AccessFineLocation
        };

        private static ConnectionFragment _instance;
        private bool _isReceiveredRegistered;
        private BluetoothDeviceReceiver _receiver;

       

        public static ConnectionFragment NewInstance()
        {
            var detailsFrag = new ConnectionFragment { Arguments = new Bundle() };
            return detailsFrag;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            bluetoothAdapter = BluetoothAdapter.DefaultAdapter;
            base.OnCreate(savedInstanceState);

            if (!bluetoothAdapter.IsEnabled)
            {
                Intent enableBtIntent = new Intent(BluetoothAdapter.ActionRequestEnable);
                StartActivityForResult(enableBtIntent, REQUEST_ENABLE_BT);
            }

            var coarseLocationPermissionGranted =
                ContextCompat.CheckSelfPermission(Activity, Manifest.Permission.AccessCoarseLocation);
            var fineLocationPermissionGranted =
                ContextCompat.CheckSelfPermission(Activity, Manifest.Permission.AccessFineLocation);

            if (coarseLocationPermissionGranted != Permission.Denied ||
                fineLocationPermissionGranted == Permission.Denied)
                Android.Support.V4.App.ActivityCompat.RequestPermissions(Activity, LocationPermissions, LocationPermissionsRequestCode);

            //Register for broadcasts when a device is discovered
           
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            if (container == null)
            {
                // Currently in a layout without a container, so no reason to create our view.
                return null;
            }

            // Use this to return your custom view for this Fragment
            fView = inflater.Inflate(Resource.Layout.Connection, container, false);
            

            button = fView.FindViewById<Button>(Resource.Id.button1);
            fView.FindViewById<Button>(Resource.Id.button1).Click += BtnConnect_Click;

            button = fView.FindViewById<Button>(Resource.Id.btnPrescan);
            fView.FindViewById<Button>(Resource.Id.btnPrescan).Click += BtnPrescan_Click;

            button = fView.FindViewById<Button>(Resource.Id.button2);
            fView.FindViewById<Button>(Resource.Id.button2).Click += BtnDisconnect_Click;
            fView.FindViewById<Button>(Resource.Id.button2).Enabled = false;

            // button = fView.FindViewById<Button>(Resource.Id.btnConnectUSB);
           

            var lst = fView.FindViewById<Android.Widget.ListView>(Resource.Id.list);
            lst.ChoiceMode = Android.Widget.ChoiceMode.Single;
            lst.ItemSelected += Lst_ItemSelected;
            lst.ItemClick += Lst_ItemClick;

            _receiver = new BluetoothDeviceReceiver(this);

            RegisterBluetoothReceiver();

            PopulateListView();
 
            if (MainActivity.fiscal != null)
            {
                fView.FindViewById<Button>(Resource.Id.button1).Enabled = !MainActivity.fiscal.device_Connected;
                fView.FindViewById<Button>(Resource.Id.button2).Enabled = MainActivity.fiscal.device_Connected;
                fView.FindViewById<Button>(Resource.Id.btnPrescan).Enabled = !MainActivity.fiscal.device_Connected;
            }
            return fView;
        }

        


        private void BtnDisconnect_Click(object sender, EventArgs e)
        {
            ManageControls_ConnectTab(false);
            
            ThreadPool.QueueUserWorkItem(delegate
            {
                try
                {
                    if (MainActivity.fiscal.device_Connected) MainActivity.fiscal.Disconnect();

                }
                catch (Exception ex)
                {
                    if (Activity != null)
                    {
                        this.Activity.RunOnUiThread(() =>
                        {
                            {
                                MainActivity.ShowMessage(ex.Message,"Error");
                                ManageControls_ConnectTab(true);
                                Activity.FindViewById<Button>(Resource.Id.button1).Enabled = false;
                            }
                        });
                    }
                }
                finally
                {
                    if (Activity != null)
                    {
                        Activity.RunOnUiThread(() =>
                    {
                        {
                            ManageControls_ConnectTab(true);
                            Calculate_Controls();
                        }
                    });
                    }
                }
            }, null);
        }

        private void Lst_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            if (Activity != null)
            {
                var lst = Activity.FindViewById<Android.Widget.ListView>(Resource.Id.list);

                var adapter = lst.Adapter as ListViewAdapter;
                if (e.Position < 0) throw new Exception("Bluetooth device is not seleced.");
                if (adapter.Items[e.Position].GetListItemType() == ListItemType.DataItem)
                {
                    SelectedDevice = (adapter.Items[e.Position] as DataItem).BluetoothDev;

                }
            }
        }

        private void Lst_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            if (Activity != null)
            {
                var lst = Activity.FindViewById<Android.Widget.ListView>(Resource.Id.list);
                var adapter = lst.Adapter as ListViewAdapter;
                if (e.Position < 0) throw new Exception("Bluetooth device is not seleced.");
                if (adapter.Items[e.Position].GetListItemType() == ListItemType.DataItem)
                    SelectedDevice = (adapter.Items[e.Position] as DataItem).BluetoothDev;
            }
        }

        private void BtnPrescan_Click(object sender, EventArgs e)
        {
            BluetoothAdapter bluetoothAdapter = BluetoothAdapter.DefaultAdapter;
            if (!bluetoothAdapter.IsEnabled)
            {
                Intent enableBtIntent = new Intent(BluetoothAdapter.ActionRequestEnable);
                StartActivityForResult(enableBtIntent, REQUEST_ENABLE_BT);
            }
            var coarseLocationPermissionGranted =
                ContextCompat.CheckSelfPermission(Activity, Manifest.Permission.AccessCoarseLocation);
            var fineLocationPermissionGranted =
                ContextCompat.CheckSelfPermission(Activity, Manifest.Permission.AccessFineLocation);

            if (coarseLocationPermissionGranted != Permission.Denied ||
                fineLocationPermissionGranted == Permission.Denied)
                Android.Support.V4.App.ActivityCompat.RequestPermissions(Activity, LocationPermissions, LocationPermissionsRequestCode);

            // Register for broadcasts when a device is discovered
            // _receiver = new BluetoothDeviceReceiver();

            RegisterBluetoothReceiver();

            PopulateListView();
        }

        private void RegisterBluetoothReceiver()
        {
            if (_isReceiveredRegistered) return;
            if (Activity != null)
            {
                Activity.RegisterReceiver(_receiver, new IntentFilter(BluetoothDevice.ActionFound));
                Activity.RegisterReceiver(_receiver, new IntentFilter(BluetoothAdapter.ActionDiscoveryStarted));
                Activity.RegisterReceiver(_receiver, new IntentFilter(BluetoothAdapter.ActionDiscoveryFinished));
                _isReceiveredRegistered = true;
            }
        }

        private void UnregisterBluetoothReceiver()
        {
            if (!_isReceiveredRegistered) return;
            if (Activity != null)
            {
                Activity.UnregisterReceiver(_receiver);
                _isReceiveredRegistered = false;
            }
        }

        private void PopulateListView()
        {
            var item = new List<IListItem>
            {
                new HeaderListItem("PREVIOUSLY PAIRED")
            };
            var paired = BluetoothDeviceReceiver.Adapter.BondedDevices.Select(
                    bluetoothDevice => new DataItem(
                        bluetoothDevice.Name,
                        bluetoothDevice.Address,
                        bluetoothDevice
                    )
                );
            item.AddRange(paired);
            var lst = fView.FindViewById<Android.Widget.ListView>(Resource.Id.list);
            item.Add(new StatusHeaderListItem("Scanning started..."));
            lst.Adapter = new ListViewAdapter(Activity, item);
            if (SelectedDevice!=null)
            {
                var arr = paired.ToArray();
                for(int i=0;i<item.Count();i++)
                {
                    if (item[i].GetType() != typeof(DataItem))
                        continue;

                    if(((DataItem)item[i]).BluetoothDev != null && ((DataItem)item[i]).BluetoothDev.Equals(SelectedDevice))
                    {
                        lst.SetItemChecked(i, true);
                        break;
                    }
                }
            }


            StartScanning();

            


        }

        private void ManageControls_ConnectTab(bool value)
        {if (fView != null)
            {
                fView.FindViewById<Button>(Resource.Id.button1).Enabled = value;
                fView.FindViewById<Button>(Resource.Id.button2).Enabled = value;
                fView.FindViewById<Button>(Resource.Id.btnPrescan).Enabled = value;
            }
        }

        private void BtnConnect_Click(object sender, System.EventArgs e)
        {
#if DEBUGX
            using (InvoiceRangeDialog fmInvoice = new InvoiceRangeDialog(MainActivity.GetInstance()))
            {
                fmInvoice.Show();
                fmInvoice.startDefaultVal = "10";
                return;
            }
#endif
                ManageControls_ConnectTab(false);
            ThreadPool.QueueUserWorkItem(delegate
            {
                try
                {
                    BluetoothAdapter adapter = BluetoothAdapter.DefaultAdapter;
                    if (adapter == null)
                        throw new Exception("No Bluetooth adapter found.");

                    if (!adapter.IsEnabled)
                        throw new Exception("Bluetooth adapter is not enabled.");

                    BluetoothDevice device = SelectedDevice;


                    if (device == null)
                        throw new Exception("Named device not found.");

                    BluetoothSocket _socket = device.CreateInsecureRfcommSocketToServiceRecord(UUID.FromString("00001101-0000-1000-8000-00805f9b34fb"));
                    FiscalCommAndroid fiscalComm = new FiscalCommAndroid(_socket);
                    MainActivity.fiscal = new FDGROUP_A_BGR(fiscalComm);
                    MainActivity.fiscal.Connect();

                    if (MainActivity.fiscal.device_Connected)
                    {
                        
                       // tcMain.SelectedTab = tpReceipts;
                       // cbLanguage.SelectedIndex = (int)MainActivity.fiscal.language;
                        
                       // get_StatusDescriptions();
                       // Get_StatusState();
                    }

                }
                catch (Exception ex)
                {
                    if (Activity != null)
                    {
                        this.Activity.RunOnUiThread(() =>
                    {
                        {
                            MainActivity.ShowMessage(ex.Message,"Error");
                            ManageControls_ConnectTab(true);
                            Activity.FindViewById<Button>(Resource.Id.button2).Enabled = false;
                        }
                    });
                    }
                }
                finally
                {
                    if (Activity != null)
                    {
                        this.Activity.RunOnUiThread(() =>
                    {
                        {
                            ManageControls_ConnectTab(true);
                            Calculate_Controls();
                        }
                    });
                    }
                }
            }, null);

        }

        private void Calculate_Controls()
        {
            if (Activity != null)
            {
                if (MainActivity.fiscal == null)
                {


                    this.Activity.FindViewById<Button>(Resource.Id.button1).Enabled = true;
                    this.Activity.FindViewById<Button>(Resource.Id.btnPrescan).Enabled = true;
                    this.Activity.FindViewById<Button>(Resource.Id.button2).Enabled = false;


                }
                else
                {
                    this.Activity.FindViewById<Button>(Resource.Id.button1).Enabled = !MainActivity.fiscal.device_Connected;
                    this.Activity.FindViewById<Button>(Resource.Id.button2).Enabled = MainActivity.fiscal.device_Connected;
                    this.Activity.FindViewById<Button>(Resource.Id.btnPrescan).Enabled = !MainActivity.fiscal.device_Connected;
                }
            }
        }

        private static void StartScanning()
        {
            if (!BluetoothDeviceReceiver.Adapter.IsDiscovering) BluetoothDeviceReceiver.Adapter.StartDiscovery();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            // Make sure we're not doing discovery anymore
            CancelScanning();

            // Unregister broadcast listeners
            UnregisterBluetoothReceiver();
        }

        public override void OnPause()
        {
            base.OnPause();

            // Make sure we're not doing discovery anymore
            CancelScanning();

            // Unregister broadcast listeners
            UnregisterBluetoothReceiver();
        }

        public override void OnResume()
        {
            base.OnResume();

            StartScanning();

            // Register broadcast listeners
            RegisterBluetoothReceiver();
        }

        public void UpdateAdapter(DataItem dataItem)
        {
            var lst = Activity.FindViewById<Android.Widget.ListView>(Resource.Id.list);
            var selected = lst.SelectedItemPosition;
            var adapter = lst.Adapter as ListViewAdapter;
            var items = adapter?.Items.Where(m => m.GetListItemType() == ListItemType.DataItem).ToList();

            if (items != null && !items.Any(x =>
                    ((DataItem)x).Text == dataItem.Text && ((DataItem)x).SubTitle == dataItem.SubTitle))
            {
                adapter.Items.Add(dataItem);
            }
            adapter.NotifyDataSetChanged();
            //lst.Adapter = new ListViewAdapter(Activity, adapter?.Items);
        }


        private static void CancelScanning()
        {
            if (BluetoothDeviceReceiver.Adapter.IsDiscovering) BluetoothDeviceReceiver.Adapter.CancelDiscovery();
        }

        public void UpdateAdapterStatus(string discoveryStatus)
        {
            var lst = fView.FindViewById<Android.Widget.ListView>(Resource.Id.list);
            var adapter = lst.Adapter as ListViewAdapter;

            var hasStatusItem = adapter?.Items?.Any(m => m.GetListItemType() == ListItemType.Status);

            if (hasStatusItem.HasValue && hasStatusItem.Value)
            {
                var statusItem = adapter.Items.Single(m => m.GetListItemType() == ListItemType.Status);
                statusItem.Text = discoveryStatus;
            }

            // lst.Adapter = new ListViewAdapter(this, adapter?.Items);
            //count = adapter.Items.
        }
    }
}