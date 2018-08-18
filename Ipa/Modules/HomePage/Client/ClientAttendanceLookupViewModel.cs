﻿using System;

using Xamarin.Forms;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace Ipa
{
	public class ClientAttendanceLookupViewModel :  ViewModelBase
	{
		private AttendanceListItemViewModel _SelectedItem;

		private ObservableCollection<ReporteeViewModel> _ReporteeAbsent;
		public ObservableCollection<ReporteeViewModel> ReporteeAbsent{
			set{ 
				if(Set(()=> ReporteeAbsent, ref _ReporteeAbsent, value)){
					RaisePropertyChanged(()=> ReporteeAbsent);
				}
			}
			get{ 
				return _ReporteeAbsent ?? (_ReporteeAbsent = new ObservableCollection<ReporteeViewModel> ());
			}
		}
		private bool _IsBusy;

		public bool IsBusy {
			set { 
				if (Set (() => IsBusy, ref _IsBusy, value)) {
					_IsBusy = value;
				}
			}
			get { 
				return _IsBusy;
			}
		}

		private ObservableCollection<ReporteeViewModel> _ReporteePresent;
		public ObservableCollection<ReporteeViewModel> ReporteePresent{
			set{ 
				if(Set(()=> ReporteePresent, ref _ReporteePresent, value)){
					RaisePropertyChanged(()=> ReporteePresent);
				}
			}
			get{ 
				return _ReporteePresent ?? (_ReporteePresent = new ObservableCollection<ReporteeViewModel> ());
			}
		}
		public string ClassDate{
			get{ 
				return _SelectedItem.ClassDate;
			}
		}

		public string ClassDay{
			get{ 
				return _SelectedItem.ClassDay;
			}
		}
			
		public string TotalPresent{
			get{
				return string.Format ("PRESENT ({0})", _SelectedItem.PresentList.Count);
			}
		}

		public string TotalAbsent {
			get {
				return string.Format ("ABSENT ({0})", _SelectedItem.AbsentList.Count);
			}
		}
		public bool CanShowPresent {
			get {
				return _SelectedItem.CanShowPresent;
			}
		}

		public bool CanShowAbsent {
			get {
				return _SelectedItem.CanShowAbsent;
			}
		}
		private ReporteeViewModel _SelectedReportee;

		public ReporteeViewModel SelectedReportee {

			set { 
				_SelectedReportee = value;
				RaisePropertyChanged (() => SelectedReportee);
				if (value != null)
					SelectedReportee = null;
			}
			get { 
				return _SelectedReportee;
			}
		}

		public ClientAttendanceLookupViewModel (AttendanceListItemViewModel selectedAttendance)
		{
			this._SelectedItem = selectedAttendance;
			GetReporteeAttendanceItemList (selectedAttendance.ParticipantList);
		}

		private RelayCommand _BackBtnCmd;

		public RelayCommand BackBtnCmd {
			get { 
				return _BackBtnCmd ?? (_BackBtnCmd = new RelayCommand (() => {
					NavigationHandler.GlobalNavigator.Navigation.PopAsync();
				}));
			}
		}

		public void GetReporteeAttendanceItemList(List<Reportee> reporteeList)
		{
			foreach (var reportee in reporteeList) {
				ReporteeViewModel reporteeViewModel = new ReporteeViewModel (reportee);
				if ((_SelectedItem.AbsentList != null) && _SelectedItem.AbsentList.Exists (s => s.Equals (reportee.UserName))) {
					ReporteeAbsent.Add (reporteeViewModel);
				} else if ((_SelectedItem.PresentList != null) && _SelectedItem.PresentList.Exists (s => s.Equals (reportee.UserName))) {
					ReporteePresent.Add (reporteeViewModel);
				}
			}
		}
	
	}
}


