
using Controls.ViewModels;
using DeviceHandler.ViewModels;
using DeviceHandler.Views;
using DeviceSimulators.ViewModels;
using DeviceSimulators.Views;
using EOL.Views;
using ScriptHandler.ViewModels;
using ScriptHandler.Views;
using ScriptRunner.ViewModels;
using ScriptRunner.Views;
using Syncfusion.Windows.Tools.Controls;
using System.Windows.Controls;
using System.Windows.Documents;

namespace EOL.ViewModels
{
	public class EOLDockingViewModel : DocingBaseViewModel
	{
		private ContentControl _technicianView;
		private ContentControl _designView;
		private ContentControl _runView;
		private ContentControl _mainScriptLogger;
		private ContentControl _communicationSettings;
		private ContentControl _deviceSimulatorsViewModel;

		public EOLDockingViewModel(
			OperatorViewModel technicianVM,
			DesignViewModel designVM,
			RunViewModel runVM,
			CommunicationViewModel communicationSettings,
			DeviceSimulatorsViewModel deviceSimulatorsViewModel) :
			base("EOL")
		{
			DockFill = true;

			CreateWindows(
				technicianVM,
				designVM,
				runVM,
				communicationSettings,
				deviceSimulatorsViewModel);
		}

		private void CreateWindows(
			OperatorViewModel technicianVM,
			DesignViewModel designVM,
			RunViewModel runVM,
			CommunicationViewModel communicationSettings,
			DeviceSimulatorsViewModel deviceSimulatorsViewModel)
		{
			_technicianView = new ContentControl();
			TechnicianView technicianView = new TechnicianView() { DataContext = technicianVM };
			_technicianView.Content = technicianView;
			SetHeader(_technicianView, "Technician");
			//SetCanClose(_userView, false);
			SetCanAutoHide(_technicianView, false);
			SetCanFloat(_technicianView, false);
			SetCanDocument(_technicianView, false);
			Children.Add(_technicianView);


			_communicationSettings = new ContentControl();
			CommunicationView communication = new CommunicationView() { DataContext = communicationSettings };
			_communicationSettings.Content = communication;
			SetHeader(_communicationSettings, "Communication Settings");
			SetFloatParams(_communicationSettings);
			Children.Add(_communicationSettings);

			_deviceSimulatorsViewModel = new ContentControl();
			DeviceSimulatorsView deviceSimulators = new DeviceSimulatorsView() { DataContext = deviceSimulatorsViewModel };
			_deviceSimulatorsViewModel.Content = deviceSimulators;
			SetHeader(_deviceSimulatorsViewModel, "Device Simulators");
			SetFloatParams(_deviceSimulatorsViewModel);
			Children.Add(_deviceSimulatorsViewModel);


			DesignView designView = new DesignView() { DataContext = designVM };
			CreateTabbedWindow(designView, "Design", string.Empty, out _designView);
			SetDesiredWidthInDockedMode(_designView, 1200);

			RunView runView = new RunView() { DataContext = runVM };
			CreateTabbedWindow(runView, "Run", "Design", out _runView);
			runVM.CreateScriptLogDiagramViewEvent += Run_CreateScriptLogDiagramViewEvent;
			runVM.ShowScriptLogDiagramViewEvent += Run_ShowScriptLogDiagramViewEvent;
		}

		private void Run_CreateScriptLogDiagramViewEvent(ScriptLogDiagramViewModel mainScriptLogger)
		{
			_mainScriptLogger = new ContentControl();
			ScriptLogDiagramView scriptLog = new ScriptLogDiagramView() { DataContext = mainScriptLogger };
			_mainScriptLogger.Content = scriptLog;
			SetHeader(_mainScriptLogger, "Script Run Diagram");
			SetState(_mainScriptLogger, DockState.Hidden);
			SetSideInDockedMode(_mainScriptLogger, DockSide.Right);
			Children.Add(_mainScriptLogger);
		}

		private void Run_ShowScriptLogDiagramViewEvent()
		{
			OpenLogScript();
		}

		public void OpenLogScript()
		{
			SetState(_mainScriptLogger, DockState.Dock);
		}

		public void OpenCommSettings()
		{
			SetState(_communicationSettings, DockState.Float);
		}

		public void OpenDeviceSimulators()
		{
			SetState(_deviceSimulatorsViewModel, DockState.Float);
		}



		public void ShowTechnician()
		{			
			SetState(_technicianView, DockState.Dock);
			SetCanClose(_technicianView, false);
		}

		public void HideTechnician()
		{
			SetCanClose(_technicianView, true);
			SetState(_technicianView, DockState.Hidden);
		}

		public void ShowAdmin()
		{
			SetState(_designView, DockState.Dock);
			SetState(_runView, DockState.Dock);
			SetState(_mainScriptLogger, DockState.Dock);
		}

		public void HideAdmin()
		{
			SetState(_designView, DockState.Hidden);
			SetState(_runView, DockState.Hidden);
			SetState(_mainScriptLogger, DockState.Hidden);
		}
	}
}
