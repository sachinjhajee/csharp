using Android.App;
using Android.Content;
using Android.Hardware.Usb;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Animation;
using Android.Support.Design.Chip;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Support.V4.App;

namespace SalesDemo_DevGr_A
{
    public class UsbReceiver : BroadcastReceiver
    {
        public static int DATECS_USB_VID = 65520;
        public static int FTDI_USB_VID = 1027;
        private static string ACTION_USB_DETACHED = "android.hardware.usb.action.USB_DEVICE_DETACHED";
        private static UsbConnectionFragment child;

        public override void OnReceive(Context context, Intent intent)
        {
            String action = intent.Action;
            if (UsbManager.ActionUsbDeviceDetached.Equals(action))
            {
                lock(this) {
                    UsbDevice device = (UsbDevice)intent.GetParcelableExtra(UsbManager.ExtraDevice);
                    if ((device.VendorId == DATECS_USB_VID) ||
                            (device.VendorId == FTDI_USB_VID))
                    {
                        //child = new UsbConnectionFragment();
                        //var fmtrans = SupportFragmentManager.BeginTransaction();
                        //fmtrans.Add(Resource.Id.fragment_container, child, "child");
                        //fmtrans.Commit();
                    }
                }
            }
        }
    }
}