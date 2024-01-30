
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EOL.Models;
using Services.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;
using System.ComponentModel;
using System.Reflection;
using DeviceHandler.Models;
using DeviceCommunicators.Models;
using DeviceHandler.Models.DeviceFullDataModels;
using Entities.Enums;
using DeviceCommunicators.Services;
using System.Linq;
using ScriptHandler.ViewModels;
using ScriptRunner.ViewModels;

namespace EOL.ViewModels
{
	public class EOLMainViewModel: ObservableObject
	{
		#region Properties

		public string Version { get; set; }
		public EOLDockingViewModel Docking { get; set; }

		public DevicesContainer DevicesContainter { get; set; }

		#endregion Properties

		#region Fields

		private EOLSettings _eolSettings;

		private UserViewModel _userVM;
		private DesignViewModel _designVM;
		private RunViewModel _runVM;

		#endregion Fields

		#region Constructor

		public EOLMainViewModel()
		{		

			Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();

			SetAdminCommand = new RelayCommand(SetAdmin);
			SetUserCommand = new RelayCommand(SetUser);
			ChangeDarkLightCommand = new RelayCommand(ChangeDarkLight);

			ClosingCommand = new RelayCommand<CancelEventArgs>(Closing);
			LoadedCommand = new RelayCommand(Loaded);
		}

		#endregion Constructor

		#region Methods

		private void Closing(CancelEventArgs e)
		{
			EOLSettings.SaveEvvaUserData("EOL",_eolSettings);
		}

		#region Load

		private void Loaded()
		{
			try
			{ 
			
				LoggerService.Init("EOL.log", Serilog.Events.LogEventLevel.Information);
				LoggerService.Inforamtion(this, "-------------------------------------- EOL ---------------------");


				LoggerService.Inforamtion(this, "Starting Loaded of EOLMainViewModel");

				_eolSettings = EOLSettings.LoadEvvaUserData("EOL");
				ChangeDarkLight();

				DevicesContainter = new DevicesContainer();
				DevicesContainter.DevicesFullDataList = new ObservableCollection<DeviceFullData>();
				DevicesContainter.DevicesList = new ObservableCollection<DeviceData>();
				DevicesContainter.TypeToDevicesFullData = new Dictionary<DeviceTypesEnum, DeviceFullData>();
				UpdateSetup();


				_userVM = new UserViewModel();
				_designVM = new DesignViewModel(DevicesContainter, _eolSettings.ScriptUserData);

				ObservableCollection<DeviceParameterData> logParametersList = 
					new ObservableCollection<DeviceParameterData>();
				_runVM = new RunViewModel(logParametersList, DevicesContainter, _eolSettings.ScriptUserData, null);

				Docking = new EOLDockingViewModel(
					_userVM,
					_designVM,
					_runVM);

				Docking.ShowUser();
				Docking.HideAdmin();

				_runVM.CreateScriptLoggerWindow();
			}
			catch (Exception ex)
			{
				LoggerService.Error(this, "Failed to init the main window", "Startup Error", ex);
			}
		}

		private void UpdateSetup()
		{

			ReadDevicesFileService readDevicesFile = new ReadDevicesFileService();
			ObservableCollection<DeviceData> deviceList = readDevicesFile.ReadAllFiles(
				@"Data\Device Communications\",
				@"Data\Device Communications\param_defaults.json",
				null,
				null,
				null,
				false);


			List<DeviceData> newDevices = new List<DeviceData>();
			foreach (DeviceData deviceData in deviceList)
			{
				DeviceData existingDevice =
					DevicesContainter.DevicesList.ToList().Find((d) => d.DeviceType == deviceData.DeviceType);
				if (existingDevice == null)
					newDevices.Add(deviceData);
			}

			List<DeviceData> removedDevices = new List<DeviceData>();
			foreach (DeviceData deviceData in DevicesContainter.DevicesList)
			{
				DeviceData existingDevice =
					deviceList.ToList().Find((d) => d.DeviceType == deviceData.DeviceType);
				if (existingDevice == null)
					removedDevices.Add(deviceData);
			}




			foreach (DeviceData device in removedDevices)
			{
				DeviceFullData deviceFullData =
					DevicesContainter.DevicesFullDataList.ToList().Find((d) => d.Device.DeviceType == device.DeviceType);
				deviceFullData.Disconnect();

				DevicesContainter.DevicesFullDataList.Remove(deviceFullData);
				DevicesContainter.DevicesList.Remove(deviceFullData.Device);
				DevicesContainter.TypeToDevicesFullData.Remove(deviceFullData.Device.DeviceType);
			}



			foreach (DeviceData device in newDevices)
			{
				DeviceFullData deviceFullData = DeviceFullData.Factory(device);

				deviceFullData.Init();

				DevicesContainter.DevicesFullDataList.Add(deviceFullData);
				DevicesContainter.DevicesList.Add(device as DeviceData);
				if (DevicesContainter.TypeToDevicesFullData.ContainsKey(device.DeviceType) == false)
					DevicesContainter.TypeToDevicesFullData.Add(device.DeviceType, deviceFullData);

				deviceFullData.Connect();
			}
		}

		#endregion Load

		private void SetAdmin()
		{
			Docking.HideUser();
			Docking.ShowAdmin();
		}

		private void SetUser()
		{
			Docking.ShowUser();
			Docking.HideAdmin();
		}

		private void ChangeDarkLight()
		{
			_eolSettings.IsLightTheme = !_eolSettings.IsLightTheme;
			App.ChangeDarkLight(_eolSettings.IsLightTheme);

			
		}

		#endregion Methods

		#region Commands

		public RelayCommand SetAdminCommand { get; private set; }
		public RelayCommand SetUserCommand { get; private set; }
		public RelayCommand ChangeDarkLightCommand { get; private set; }

		public RelayCommand LoadedCommand { get; private set; }
		public RelayCommand<CancelEventArgs> ClosingCommand { get; private set; }

		#endregion Commands
	}
}
