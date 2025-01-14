using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SalesDemo_DevGr_A
{
    class StatusBytesCustomAdapter : BaseAdapter
    {
        private const int TYPE_ITEM = 0;
        private const int TYPE_SEPARATOR = 1;

        private Java.Util.ArrayList mData = new Java.Util.ArrayList();
        private TreeSet sectionHeader = new TreeSet();

        private LayoutInflater mInflater;

        public StatusBytesCustomAdapter(Context context)
        {
            mInflater = (LayoutInflater)context
                    .GetSystemService(Context.LayoutInflaterService);
        }

        public void addItem(string item)
        {
            mData.Add(item);
            NotifyDataSetChanged();
        }

        public void addSectionHeaderItem(string item)
        {
            mData.Add(item);
            sectionHeader.Add(mData.Size() - 1);
            NotifyDataSetChanged();
        }


        public override int GetItemViewType(int position)
        {
            return sectionHeader.Contains(position) ? TYPE_SEPARATOR : TYPE_ITEM;
        }


        public override int ViewTypeCount
        {
            get
            { return 2; }
        }

        private int tag = new System.Random().Next();

        public override int Count
        {
            get
            { return mData.Size(); }
        }



        public override Java.Lang.Object GetItem(int position)
        {
            return mData.Get(position);
        }


        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            ViewHolder holder = null;
            int rowType = GetItemViewType(position);

            if (convertView == null)
            {
                holder = new ViewHolder();
                switch (rowType)
                {
                    case TYPE_ITEM:
                        convertView = mInflater.Inflate(Resource.Layout.StatusContent, null);
                        break;
                    case TYPE_SEPARATOR:
                        convertView = mInflater.Inflate(Resource.Layout.StatusBytesHeaders, null);
                         break;
                }
                convertView.SetTag(tag,holder);
            }
            else
            {
                holder = (ViewHolder)convertView.GetTag(tag);
            }
            switch (rowType)
            {
                case TYPE_ITEM:
                    holder.textView = (CheckBox)convertView.FindViewById(Resource.Id.chckStatus);
                    var data = ((String)mData.Get(position)).ToString().Split('|');
                    bool bold = data[1] == "1";
                    holder.textView.SetTypeface(null, bold? Android.Graphics.TypefaceStyle.Bold : Android.Graphics.TypefaceStyle.Normal);
                    bool check = data[2] == "1";
                    ((CheckBox)holder.textView).Checked = check;
                    holder.textView.SetText(data[0], TextView.BufferType.Normal);
                    break;
                case TYPE_SEPARATOR:
                    holder.textView = (TextView)convertView.FindViewById(Resource.Id.textSeparator);
                    holder.textView.SetText((String)mData.Get(position), TextView.BufferType.Normal);
                    break;
            }
            // holder.textView.SetTypeface(null, Typeface.BOLD);
            //holder.textView.SetText(mData.Get(position), 0, ((Java.Lang.String)mData.Get(position)).Length());
            return convertView;
        }

        public class ViewHolder: Java.Lang.Object
        {
            public TextView textView;
        }

    }
}