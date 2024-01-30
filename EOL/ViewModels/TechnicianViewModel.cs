
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DeviceCommunicators.Models;
using DeviceHandler.Models;
using Newtonsoft.Json;
using ScriptHandler.Models;
using ScriptHandler.Services;
using ScriptRunner.Enums;
using ScriptRunner.Models;
using ScriptRunner.Services;
using Services.Services;
using System;
using System.IO;

namespace EOL.ViewModels
{
	public class TechnicianViewModel:ObservableObject
	{
		public enum ScriptStateEnum { Running, Pass, Fail, None, }

		#region Properties

		public ScriptStateEnum ScriptState { get; set; }

		public RunScriptService RunScript { get; set; }
		public string ErrorDescription { get; set; }

		#endregion Properties

		#region Fields

		private DevicesContainer _devicesContainer;
		private OpenProjectForRunService _openProjectForRun;
		private ScriptUserData _scriptUserData;

		#endregion Fields

		#region Constructor

		public TechnicianViewModel(
			DevicesContainer devicesContainer,
			ScriptUserData scriptUserData)
		{
			_devicesContainer = devicesContainer;
			_scriptUserData = scriptUserData;

			RunCommand = new RelayCommand(Run);

			ScriptState = ScriptStateEnum.None;

			StopScriptStepService stopScriptStep = new StopScriptStepService();
			RunScript = new RunScriptService(
					null,
					devicesContainer,
					null,
					null);

			RunScript.ScriptEndedEvent += RunScript_ScriptEndedEvent;

			_openProjectForRun = new OpenProjectForRunService();
		}

		

		#endregion Constructor

		#region Methods

		private GeneratedScriptData GetScript(string path)
		{
			GeneratedProjectData project = _openProjectForRun.Open(path, _devicesContainer, RunScript);

			return project.TestsList[0];
		}

		private void Run()
		{
			try
			{
				ErrorDescription = string.Empty;
				ScriptState = ScriptStateEnum.None;

				GeneratedScriptData currentScript = GetScript(
					@"C:\Users\smadar\Documents\Scripts\Tests\Run Repeat Set\Run Repeat Set.gprj");
				if (currentScript == null)
					return;

				RunScript.AbortScriptPath = @"C:\Users\smadar\Documents\Scripts\Tests\Empty Script.scr";

				if (RunScript.AbortScriptStep == null)
				{
					if (string.IsNullOrEmpty(RunScript.AbortScriptPath))
					{
						LoggerService.Error(this, "No abort script is defined", "Run Script");
						return;
					}

					RunScript.AbortScriptStep = new ScriptStepAbort(RunScript.AbortScriptPath, _devicesContainer);
					if (RunScript.AbortScriptStep == null)
					{
						LoggerService.Error(this, "The abort script is invalid", "Run Script");
						return;
					}


				}

				RunScript.Run(
					null,
					currentScript,
					null,
					false);
			}
			catch(Exception ex) 
			{
				LoggerService.Error(this, "Fialed to run", "Error", ex);
			}
		}

		private void RunScript_ScriptEndedEvent(ScriptStopModeEnum scriptStopMode)
		{
			if (scriptStopMode == ScriptStopModeEnum.Ended)
				ScriptState = ScriptStateEnum.Pass;
			else
				ScriptState = ScriptStateEnum.Fail;

			if (RunScript.CurrentScript != null &&
				RunScript.CurrentScript.CurrentScript.Name == "Failed Step Notification")
			{
				ScriptStepNotification notification =
					RunScript.CurrentScript.CurrentScript.ScriptItemsList[0] as ScriptStepNotification;
				ErrorDescription = notification.Notification;
			}

			RunScript.ExecutedStepsPercentage = 100;
		}

		#endregion Methods

		#region Commands

		public RelayCommand RunCommand { get; private set; }

		#endregion Commands
	}
}
