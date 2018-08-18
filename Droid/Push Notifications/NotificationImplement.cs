using System;
using Acr.Settings;
using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Preferences;
using Android.Support.V7.App;
using Android.Widget;
using Ipa.Notification;
using Newtonsoft.Json.Linq;

namespace Ipa.Droid
{
	public class NotificationImplement : INotification
	{

		public Context context { 
			get 
			{
				return MainApplication.AppContext;
			} 
		}

		public void Show(int notifyId,  NotificationPayload payload)
		{
			try
			{
				PendingIntent resultPendingIntent = null;
				if ((null != payload.payload) && ( (null != payload.payload.CourseId) || (null != payload.payload.ActivityId)))
				{
					Intent resultIntent = context.PackageManager.GetLaunchIntentForPackage(context.PackageName);


					if (null != payload.payload.CourseId && MainActivity.IsAppRunning)
					{
						App.CurrentApplication.StartActivity(payload.payload.CourseId, payload.payload.ActivityId);
						return;
					}

					//Store the data locally 
					Settings.Local.Set<string>("CourseId",payload.payload.CourseId);
					Settings.Local.Set<string>("ActivityId", payload.payload.ActivityId);

					// Create a PendingIntent; we're only using one PendingIntent (ID = 0):
					const int pendingIntentId = 0;
					resultPendingIntent = PendingIntent.GetActivity(context, pendingIntentId, resultIntent, PendingIntentFlags.UpdateCurrent);
				}
				// Build the notification
				var builder = new NotificationCompat.Builder(context)
						  .SetAutoCancel(true) // dismiss the notification from the notification area when the user clicks on it
						  .SetContentIntent(resultPendingIntent) // start up this activity when the user clicks the intent.
						  .SetContentTitle(Constants.APP_NAME)  //(payload.Info.Title) // Set the title
						  .SetSound(RingtoneManager.GetDefaultUri(RingtoneType.Notification))
						  .SetSmallIcon(Resource.Drawable.ic_launcher) // This is the icon to display
				                                    .SetContentText(payload.Info.Body)
				                                    .SetColor(Resource.Color.notificationBgColor); // the message to display.

				Console.WriteLine("package name:" + context.PackageName);
				var remoteView = new RemoteViews(context.PackageName, Resource.Layout.customNotification);
				remoteView.SetTextViewText(Resource.Id.message, payload.Info.Body);
				builder.SetContent(remoteView);
						  

				if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Lollipop)
				{
					builder.SetSmallIcon(Resource.Drawable.notification_icon);
					//builder.SetLargeIcon();
				}
				/*

				var style = new NotificationCompat.BigTextStyle(builder);
				style.BigText(payload.Info.Body);
				*/

				//style.Build();
				//builder.SetStyle(style);
				//if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.JellyBean)
				//{
				//	// Using BigText notification style to support long message
				//}

				NotificationManager notificationManager = (NotificationManager)context.GetSystemService(Context.NotificationService);
				notificationManager.Notify(notifyId, builder.Build());//style.Build());
			}
			catch(Exception e)
			{
				Console.WriteLine(" Exception on Show " +e.Message+ "\n Source: " + e.Source + "\n Stacktrace: "+ e.StackTrace);
			}
		}
	}
}

