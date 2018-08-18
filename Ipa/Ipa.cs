using System;

using Xamarin.Forms;
using Acr.Settings;
using System.Collections.Generic;
using PushNotification.Plugin;
using System.Diagnostics;
using Plugin.Connectivity;

namespace Ipa
{
	public enum Role{ Participant, Manager, ManagerCumParticipant,Partner, Client, IpaTeam , None} ;
	public class App : Application
	{
		public static App CurrentApplication { get; set; }

		private static string _CurrentDate;
		public static string CurrentDate{
			get{ 
				return _CurrentDate ?? (_CurrentDate = Settings.Local.Get<string>("CurrentDate"));
			}
			set{ 
				_CurrentDate = value;
				Settings.Local.Set<string>("CurrentDate", value);
			}
		}
		private static string _Password;
		public static string Password{
			get{ 
				return _Password ?? (_Password = Settings.Local.Get<string>("Password"));
			}
			set{ 
				_Password = value;
				Settings.Local.Set<string>("Password", value);
			}
		}

		private static int[] _UserRole;
		public static int[] UserRole{
			set{
				_UserRole = value;
			}
			private get{ 
				return _UserRole;
			}
		}

		private static string _UserName;
		public static string UserName{
			get{ 
				return _UserName ?? (_UserName = Settings.Local.Get<string>("UserName"));
			}
			set{ 
				_UserName = value;
				Settings.Local.Set<string>("UserName", value);
			}
		}

		private static string _UserProfileImage;
		public static string UserProfileImage {
			set {
				_UserProfileImage = value;
			}
			get {
				return _UserProfileImage;
			}
		}

		private static string _UniqueAppId;
		public static string UniqueAppId{
			get{
				return _UniqueAppId ?? (_UniqueAppId = Settings.Local.Get<string> ("UniqueAppId"));
			}
			set{ 
				_UniqueAppId = value;
				Settings.Local.Set<string>("UniqueAppId", value);
			}
		}		

		private static string _IsFirstLaunch;
		public static string IsFirstLaunch{
			get{
				//return _IsFirstLaunch ?? (_IsFirstLaunch = Settings.Local.Get<string> ("IsFirstLaunch",null));
                return "No";
			}
			set{ 
				_IsFirstLaunch = "No";
				Settings.Local.Set<string>("IsFirstLaunch", value);
			}
		}

		public static bool IsRegistered
		{
			get
			{
				return  Settings.Local.Get<bool>("IsRegistered", false); 
			}
			set
			{
				Settings.Local.Get<bool>("IsRegistered", value);
			}

		}

		public MotherViewModel GlobalViewModel { get; set; }

		public async void StartActivity(string courseId, string activityId)
		{
			Device.BeginInvokeOnMainThread(async () =>
			{
				await NavigationHandler.GlobalNavigator.CurrentPage.DisplayAlert(Constants.APP_NAME, Constants.PUSH_ALERT_MESSAGE, Constants.OK_TEXT);
				GlobalViewModel.StartActivity(courseId, activityId);
			});
		}

		bool isAppInLive;
		public bool IsAppInLive
		{
			get
			{
				return isAppInLive;
			}
			set
			{
				isAppInLive = value;
			}
		}

		public App ( string courseIdToSelect = null, string activityIdToSelect = null )
		{
			
			if (null == MainPage)
			{
				NavigationPage mainPage = NavigationHandler.GlobalNavigator;
				mainPage.Navigation.PushAsync(new LoadingPage());

				if (courseIdToSelect != null)
					Debug.WriteLine(courseIdToSelect);

				if (activityIdToSelect != null)
					Debug.WriteLine(activityIdToSelect);

				if (!IsRegistered)
				{
					RegisterApp();
				}

				if (string.IsNullOrEmpty(IsFirstLaunch))
				{// if user opening for frist time
					mainPage.Navigation.PushModalAsync(new WalkThroughScreen(new WalkThroughViewModel()));
				}
				else if (string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(UniqueAppId))
				{
					NavigationHandler.LoginNavigator.Navigation.PushAsync(new LoginPage());
					mainPage.Navigation.PushModalAsync(NavigationHandler.LoginNavigator);
				}
				else
				{// if user is already logged in
                    if(string.IsNullOrEmpty(courseIdToSelect) || string.IsNullOrEmpty(activityIdToSelect)){
                        Debug.WriteLine("Course Id is missing, activityIdToSelect is missing");
                    }
                    GlobalViewModel = new MotherViewModel(courseIdToSelect, activityIdToSelect);
                     
				}

				MainPage = mainPage;
			}
			CurrentApplication = this;
		}

		public async void RegisterApp()
		{
			if (CrossConnectivity.Current.IsConnected){
				await DeviceInfoHandler.UpdateDeviceInfo(
					(response) =>
					{
						IsRegistered = true;
					},
					(errorResponse) =>
					{
						Debug.WriteLine("Device not registered.");
					}
				);
			}
		}

		protected override void OnStart ()
		{
			// Handle when your app starts
			var pushToken = Settings.Local.Get<string>(Constants.PUSH_TOKEN, null);
			if (null == pushToken)
			{
				CrossPushNotification.Current.Register();
			}
		}

		protected override void OnSleep ()
		{
//			if(!string.IsNullOrEmpty (App.UserName))
			Settings.Local.Set<string> ("UserName", App.UserName);
			Settings.Local.Set<string> ("UniqueAppId", UniqueAppId);
			Settings.Local.Set<string> ("IsFirstLaunch", IsFirstLaunch);
			Settings.Local.Set<string> ("CurrentDate", CurrentDate);
			Settings.Local.Set<string> ("Password", Password);
//			if (NavigationHandler.LoginNavigator.CurrentPage is AssessmentPage) {
//				(BindingContext as AssessmentAttemptViewModel).MoveToCache ();
//			}
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resume.
			if (TargetPlatform.Android == Device.OS)
			{
				var courseId = Settings.Local.Get<string>("CourseId", null);
				var activityId = Settings.Local.Get<string>("ActivityId", null);
				if(courseId != null){
					StartActivity(courseId, activityId);
					Settings.Local.Set<string>("CourseId", null);
					Settings.Local.Set<string>("ActivityId", null);
				}
			}
		}

		public static Role GetUserRole(){
			if (UserRole == null)
				return Role.None;

			if (UserRole.Length == 1) {
				if (UserRole [0] == 1) {
					return Role.Participant;
				} else if (UserRole [0] == 2) {
					return Role.Manager;
				} else if (UserRole [0] == 3) {
					return Role.Partner;
				} else if (UserRole [0] == 4) {
					return Role.Client;
				} else if (UserRole [0] == 5) {
					return Role.IpaTeam;
				}
			} else if(UserRole.Length == 2) {
				if (UserRole [0] == 1 && UserRole [1] == 2) {
					return Role.ManagerCumParticipant;
				} else if (UserRole [0] == 2 && UserRole [1] == 1) {
					return Role.ManagerCumParticipant;
				}
			}
			return Role.None;
		}
	}

}

