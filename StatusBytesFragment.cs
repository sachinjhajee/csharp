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
    public class StatusBytesFragment : Android.Support.V4.App.Fragment
    {
        Button button;
        private View fView;
        private StatusBytesCustomAdapter mAdapter;
        private CheckBox[,] checkboxes = new CheckBox[6, 8];
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }
    

        public static StatusBytesFragment NewInstance()
        {
            var detailsFrag = new StatusBytesFragment { Arguments = new Bundle() };
            return detailsFrag;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            Android.Widget.ListView listView;
            if (container == null)
            {
                // Currently in a layout without a container, so no reason to create our view.
                return null;
            }
            
            // Use this to return your custom view for this Fragment
            fView = inflater.Inflate(Resource.Layout.StatusBytesBasicList, container, false);
            listView = fView.FindViewById<Android.Widget.ListView>(Resource.Id.list);
            mAdapter = new StatusBytesCustomAdapter(MainActivity.GetInstance());
            for (int i = 0; i <= 5; i++)
            {
                mAdapter.addSectionHeaderItem("Status byte " + i);
                for (int j = 7; j >= 0; j--)
                {
                    
                    var tmpDescription = MainActivity.fiscal.Get_SBit_Description(i, j);
                    var tmpCheckForErr = MainActivity.fiscal.Get_SBit_ErrorChecking(i, j);
                    var tmpState = MainActivity.fiscal.Get_SBit_State(i, j);
                    mAdapter.addItem(tmpDescription + "|"+ (tmpCheckForErr? "1":"0") +"|" + (tmpState? "1": "0"));

                }
            }
            listView.SetAdapter(mAdapter);
            return fView;
        }
    }
}