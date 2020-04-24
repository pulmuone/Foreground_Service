using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace ServicesDemo3
{
    [BroadcastReceiver(Enabled = true, Exported = false)]
    [IntentFilter(new[] { Constants.NOTIFICATION_BROADCAST_ACTION })]
    public class TimeReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            Toast.MakeText(context, "Received intent!", ToastLength.Short).Show();

            string value = intent.GetStringExtra(Constants.BROADCAST_MESSAGE_KEY);
            Console.WriteLine(value);
        }
    }
}