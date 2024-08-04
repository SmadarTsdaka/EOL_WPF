
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DeviceHandler.Models;
using ScriptHandler.Models;
using ScriptHandler.Services;
using ScriptHandler.ViewModels;
using ScriptRunner.Enums;
using ScriptRunner.Services;
using ScriptRunner.ViewModels;
using Services.Services;
using System;
using System.Collections.ObjectModel;

namespace EOL.ViewModels
{
	public class RunViewModel:ObservableObject
	{
		#region Properties

		public enum RunStateEnum { None, Running, Ended, Aborted }

		public int RunPercentage { get; set; }

		public RunStateEnum RunState { get; set; }

		public ObservableCollection<string> TerminalTextsList { get; set; }

		public bool IsRunButtonEnabled { get; set; }

		public TimeSpan RunTime { get; set; }

		public RunScriptService RunScript { get; set; }

		public ScriptDiagramViewModel ScriptDiagram { get; set; }

		public bool IsAdminMode { get; set; }

		#endregion Properties

		#region Fields

		private GeneratedScriptData _currentScript;

		private DevicesContainer _devicesContainer;

		#endregion Fields

		#region Constructor

		public RunViewModel(
			GeneratedScriptData currentScript,
			DevicesContainer devicesContainer)
		{
			_currentScript = currentScript;
			_devicesContainer = devicesContainer;


			IsRunButtonEnabled = true;

			RunCommand = new RelayCommand(Run);
			AbortCommand = new RelayCommand(Abort);

			RunPercentage = 0;
			TerminalTextsList = new ObservableCollection<string>();
			RunState = RunStateEnum.None;

			StopScriptStepService stopScriptStep = new StopScriptStepService();
			RunScript = new RunScriptService(null, _devicesContainer, stopScriptStep, null);
			RunScript.ScriptEndedEvent += _runScriptService_ScriptEndedEvent;
			RunScript.ScriptStartedEvent += _runScriptService_ScriptStartedEvent;

			ScriptDiagram = new ScriptDiagramViewModel();

			OpenProjectForRunService openProject = new OpenProjectForRunService();
			GeneratedProjectData project = openProject.Open(
				@"C:\Users\smadar\Documents\Scripts\Tests\Short Delay.scr",
				devicesContainer,
				null,
				stopScriptStep);
			if (project == null ||
				project.TestsList == null || project.TestsList.Count == 0)
			{
				LoggerService.Error(this, "Failed to open the script", "Error");
				return;
			}

			_currentScript = project.TestsList[0];
		}

		#endregion Constructor

		#region Methods

		private void _runScriptService_ScriptStartedEvent()
		{
			if(_currentScript == null)
				return;

			ScriptDiagram.DrawScript(_currentScript);
		}

		private void _runScriptService_ScriptEndedEvent(ScriptStopModeEnum stopeMode)
		{
			Stop(stopeMode);
		}

		private void Run()
		{
			IsRunButtonEnabled = false;
			RunState = RunStateEnum.Running;

			RunScript.Run(null, _currentScript, null, false);
		}		

		private void Abort()
		{
			RunScript.AbortScript("User Abort");
		}

		private void Stop(ScriptStopModeEnum stopeMode)
		{			
			IsRunButtonEnabled = true;

			if (stopeMode == ScriptStopModeEnum.Aborted)
				RunState = RunStateEnum.Aborted;
			else
				RunState = RunStateEnum.Ended;
		}

		#endregion Methods

		#region Commands

		public RelayCommand RunCommand { get; private set; }
		public RelayCommand AbortCommand { get; private set; }

		#endregion Commands
	}
}
