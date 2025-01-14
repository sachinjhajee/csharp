using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SalesDemo_DevGr_A
{
    public class DeviceNotConnectedFragment : Android.Support.V4.App.Fragment
    {
        private View fView;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public static DeviceNotConnectedFragment NewInstance()
        {
            var detailsFrag = new DeviceNotConnectedFragment { Arguments = new Bundle() };
            return detailsFrag;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);
            if (container == null)
            {
                // Currently in a layout without a container, so no reason to create our view.
                return null;
            }
            fView = inflater.Inflate(Resource.Layout.DeviceNotConnected, container, false);
            return fView;
        }
    }
}