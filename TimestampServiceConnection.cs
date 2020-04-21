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

namespace ServicesDemo3
{
	public class TimestampServiceConnection : Java.Lang.Object, IServiceConnection, IGetTimestamp
	{
		static readonly string TAG = typeof(TimestampServiceConnection).FullName;

		public TimestampServiceConnection()
		{
			IsConnected = false;
			Binder = null;
		}

		public bool IsConnected { get; private set; }
		public TimestampBinder Binder { get; private set; }

		public void OnServiceConnected(ComponentName name, IBinder service)
		{
			Binder = service as TimestampBinder;
			IsConnected = this.Binder != null;

			string message = "onServiceConnected - ";
			Log.Debug(TAG, $"OnServiceConnected {name.ClassName}");
		}

		public void OnServiceDisconnected(ComponentName name)
		{
			Log.Debug(TAG, $"OnServiceDisconnected {name.ClassName}");
			IsConnected = false;
			Binder = null;
		}

		public string GetFormattedTimestamp()
		{
			if (!IsConnected)
			{
				return null;
			}

			return Binder?.GetFormattedTimestamp();
		}
	}
}