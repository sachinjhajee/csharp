using Android.Bluetooth;
using Android.Content;
using System.Collections.Generic;
using SalesDemo_DevGr_A.ListView;

namespace SalesDemo_DevGr_A
{
    public class BluetoothDeviceReceiver : BroadcastReceiver
    {
        private ConnectionFragment conF;
        public static BluetoothAdapter Adapter => BluetoothAdapter.DefaultAdapter;
        
        public BluetoothDeviceReceiver(ConnectionFragment cf)
        {
            conF = cf;
        }

        public override void OnReceive(Context context, Intent intent)
        {
            var action = intent.Action;
            
            // Found a device
            switch (action)
            {
                case BluetoothDevice.ActionFound:
                    // Get the device
                    var device = (BluetoothDevice)intent.GetParcelableExtra(BluetoothDevice.ExtraDevice);
                    //MainActivity.GetInstance().DeviceDiscovered(device);

                    // Only update the adapter with items which are not bonded
                   if (device.BondState != Bond.Bonded)
                    {
                        conF.UpdateAdapter(new DataItem(device.Name, device.Address,device));
                    }

                    break;
                case BluetoothAdapter.ActionDiscoveryStarted:
                    {
                        conF.UpdateAdapterStatus("Discovery Started...");
                        break;
                    }
                case BluetoothAdapter.ActionDiscoveryFinished:
                    conF.UpdateAdapterStatus("Discovery Finished.");
                    break;
                default:
                    break;
            }
        }
    }
}