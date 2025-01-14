using Android.Bluetooth;
using Android.Hardware.Usb;

namespace SalesDemo_DevGr_A.ListView
{
    public class DataItem : IListItem
    {
        public DataItem(string title, string subtitle, BluetoothDevice device=null, UsbDevice usbDev=null)
        {
            Text = title;
            SubTitle = subtitle;
            BluetoothDev = device;
            UsbDev = usbDev;
        }

        public string SubTitle { get; }
        public BluetoothDevice BluetoothDev { get; }
        public UsbDevice UsbDev { get; }
        public string Text { get; set; }

        public ListItemType GetListItemType()
        {
            return ListItemType.DataItem;
        }
    }
}