
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EOL.Models;
using Services.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.InteropServices;
using static System.Net.Mime.MediaTypeNames;
using System.Windows.Documents;
using System.Windows;

namespace EOL.ViewModels
{
	public class EOLMainViewModel: ObservableObject
	{
		#region Properties

		public string Version { get; set; }
		public EOLDockingViewModel Docking { get; set; }

		#endregion Properties

		#region Fields

		private EOLSettings _eolSettings;

		#endregion Fields

		#region Constructor

		public EOLMainViewModel()
		{		

			Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
			Docking = new EOLDockingViewModel("DockingMain");

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

		private void Loaded()
		{
			try
			{ 
			
				LoggerService.Init("EOL.log", Serilog.Events.LogEventLevel.Information);
				LoggerService.Inforamtion(this, "-------------------------------------- EOL ---------------------");


				LoggerService.Inforamtion(this, "Starting Loaded of EOLMainViewModel");

				_eolSettings = EOLSettings.LoadEvvaUserData("EOL");
				ChangeDarkLight();


			}
			catch (Exception ex)
			{
				LoggerService.Error(this, "Failed to init the main window", "Startup Error", ex);
			}
		}

		private void SetAdmin()
		{

		}

		private void SetUser()
		{

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
