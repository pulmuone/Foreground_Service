﻿using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Android.Util;
using System;

namespace ServicesDemo3
{
	[Activity(Label = "@string/app_name", MainLauncher = true, Icon = "@mipmap/icon")]
	public class MainActivity : Activity
	{
		static readonly string TAG = typeof(MainActivity).FullName;

		TimeReceiver receiver;

		Button stopServiceButton;
		Button startServiceButton;
		Button timestampButton;
		Intent startServiceIntent;
		Intent stopServiceIntent;
		bool isStarted = false;
		TimestampServiceConnection serviceConnection;
		internal TextView timestampMessageTextView;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			receiver = new TimeReceiver();

			SetContentView(Resource.Layout.Main);
			OnNewIntent(this.Intent);

			if (savedInstanceState != null)
			{
				isStarted = savedInstanceState.GetBoolean(Constants.SERVICE_STARTED_KEY, false);
			}

			startServiceIntent = new Intent(this, typeof(TimestampService)); //kotlin -> ServiceClass1::class.java
			startServiceIntent.SetAction(Constants.ACTION_START_SERVICE);

			stopServiceIntent = new Intent(this, typeof(TimestampService));
			stopServiceIntent.SetAction(Constants.ACTION_STOP_SERVICE);

			stopServiceButton = FindViewById<Button>(Resource.Id.stop_timestamp_service_button);
			startServiceButton = FindViewById<Button>(Resource.Id.start_timestamp_service_button);
			if (isStarted)
			{
				stopServiceButton.Click += StopServiceButton_Click;
				stopServiceButton.Enabled = true;
				startServiceButton.Enabled = false;
			}
			else
			{
				startServiceButton.Click += StartServiceButton_Click;
				startServiceButton.Enabled = true;
				stopServiceButton.Enabled = false;
			}

			timestampButton = FindViewById<Button>(Resource.Id.get_timestamp_button);
			timestampButton.Click += GetTimestampButton_Click;

			timestampMessageTextView = FindViewById<TextView>(Resource.Id.message_textview);
		}

		protected override void OnResume()
		{
			base.OnResume();
			RegisterReceiver(receiver, new IntentFilter(Constants.NOTIFICATION_BROADCAST_ACTION));
		}

		protected override void OnPause()
		{
			UnregisterReceiver(receiver);
			base.OnPause();
		}

		protected override void OnStart()
		{
			base.OnStart();
			if (serviceConnection == null)
			{
				serviceConnection = new TimestampServiceConnection();
			}
		}

		protected override void OnStop()
		{
			//DoUnBindService();
			base.OnStop();
		}

		protected override void OnNewIntent(Intent intent)
		{
			if (intent == null)
			{
				return;
			}

			var bundle = intent.Extras;
			if (bundle != null)
			{
				if (bundle.ContainsKey(Constants.SERVICE_STARTED_KEY) )
				{
					isStarted = true;
				}
			}
		}

		protected override void OnSaveInstanceState(Bundle outState)
		{
			outState.PutBoolean(Constants.SERVICE_STARTED_KEY, isStarted);
			base.OnSaveInstanceState(outState);
		}

		protected override void OnDestroy()
		{
			//Log.Info(TAG, "Activity is being destroyed; stop the service.");

			//StopService(startServiceIntent);
			base.OnDestroy();
		}

		void DoBindService()
		{
			Intent serviceToStart = new Intent(this, typeof(TimestampService));
			BindService(serviceToStart, serviceConnection, Bind.AutoCreate);
			timestampMessageTextView.Text = "";
		}

		void DoUnBindService()
		{
			UnbindService(serviceConnection);
			timestampMessageTextView.Text = "";
		}

		void GetTimestampButton_Click(object sender, System.EventArgs e)
		{
			if (serviceConnection.IsConnected)
			{
				timestampMessageTextView.Text = serviceConnection.Binder.Service.GetFormattedTimestamp();
			}
			else
			{
				timestampMessageTextView.SetText(Resource.String.service_not_connected);
			}
		}

		void StopServiceButton_Click(object sender, System.EventArgs e)
		{
			timestampButton.Enabled = false;
			stopServiceButton.Click -= StopServiceButton_Click;
			stopServiceButton.Enabled = false;

			Log.Info(TAG, "User requested that the service be stopped.");
			StopService(stopServiceIntent);
			isStarted = false;

			startServiceButton.Click += StartServiceButton_Click;
			startServiceButton.Enabled = true;

			DoUnBindService();
		}

		void StartServiceButton_Click(object sender, System.EventArgs e)
		{
			timestampButton.Enabled = true;
			startServiceButton.Enabled = false;
			startServiceButton.Click -= StartServiceButton_Click;

			if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
			{
				StartForegroundService(startServiceIntent);
			}
			else
			{
				StartService(startServiceIntent);
			}

				
			Log.Info(TAG, "User requested that the service be started.");

			isStarted = true;
			stopServiceButton.Click += StopServiceButton_Click;

			stopServiceButton.Enabled = true;

			DoBindService();
		}
	}

	
}

