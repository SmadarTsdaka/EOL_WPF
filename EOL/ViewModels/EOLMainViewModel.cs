﻿
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
using DeviceHandler.ViewModels;
using System.Windows;
using DeviceSimulators.ViewModels;

namespace EOL.ViewModels
{
	public class EOLMainViewModel: ObservableObject
	{
		#region Properties

		public string Version { get; set; }
		public EOLDockingViewModel Docking { get; set; }

		public DevicesContainer DevicesContainter { get; set; }

		public Visibility SimulatorsButtonVisibility { get; set; }

		#endregion Properties

		#region Fields

		private EOLSettings _eolSettings;

		private OperatorViewModel _operatorVM;

		#endregion Fields

		#region Constructor

		public EOLMainViewModel()
		{		

			Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();

			ChangeDarkLightCommand = new RelayCommand(ChangeDarkLight);

			ClosingCommand = new RelayCommand<CancelEventArgs>(Closing);
			LoadedCommand = new RelayCommand(Loaded);


			CommunicationSettingsCommand = new RelayCommand(InitCommunicationSettings); 
			DeviceSimulatorCommand = new RelayCommand(DeviceSimulator);
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


				_operatorVM = new OperatorViewModel(DevicesContainter,_eolSettings.ScriptUserData);

				CommunicationViewModel communicationSettings = new CommunicationViewModel(DevicesContainter);
				DeviceSimulatorsViewModel deviceSimulatorsViewModel =
					new DeviceSimulatorsViewModel(DevicesContainter);

				Docking = new EOLDockingViewModel(
					_operatorVM,
					communicationSettings,
					deviceSimulatorsViewModel);

				SimulatorsButtonVisibility = Visibility.Collapsed;
#if DEBUG
				SimulatorsButtonVisibility = Visibility.Visible;
#endif

				try
				{
					foreach (DeviceFullData deviceFullData in DevicesContainter.DevicesFullDataList)
					{
						deviceFullData.InitCheckConnection();
					}
				}
				catch (Exception ex)
				{
					LoggerService.Error(this, "Failed to init the communication check", ex);

				}
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

				deviceFullData.Init("EOL");

				DevicesContainter.DevicesFullDataList.Add(deviceFullData);
				DevicesContainter.DevicesList.Add(device as DeviceData);
				if (DevicesContainter.TypeToDevicesFullData.ContainsKey(device.DeviceType) == false)
					DevicesContainter.TypeToDevicesFullData.Add(device.DeviceType, deviceFullData);

				deviceFullData.Connect();
			}
		}

#endregion Load

		private void DeviceSimulator()
		{
			Docking.OpenDeviceSimulators();
		}


		private void ChangeDarkLight()
		{
			_eolSettings.IsLightTheme = !_eolSettings.IsLightTheme;
			App.ChangeDarkLight(_eolSettings.IsLightTheme);			
		}

		private void InitCommunicationSettings()
		{
			Docking.OpenCommSettings();
		}

#endregion Methods

#region Commands

		public RelayCommand ChangeDarkLightCommand { get; private set; }

		public RelayCommand LoadedCommand { get; private set; }
		public RelayCommand<CancelEventArgs> ClosingCommand { get; private set; }

		public RelayCommand CommunicationSettingsCommand { get; private set; }
		public RelayCommand DeviceSimulatorCommand { get; private set; }

#endregion Commands
	}
}
