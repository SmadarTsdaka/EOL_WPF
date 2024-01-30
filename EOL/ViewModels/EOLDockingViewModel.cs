
using Controls.ViewModels;
using DeviceHandler.ViewModels;
using DeviceHandler.Views;
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
		private ContentControl _userView;
		private ContentControl _designView;
		private ContentControl _runView;
		private ContentControl _mainScriptLogger;
		private ContentControl _communicationSettings;

		public EOLDockingViewModel(
			UserViewModel userVM,
			DesignViewModel designVM,
			RunViewModel runVM,
			CommunicationViewModel communicationSettings) :
			base("EOL")
		{
			DockFill = true;

			CreateWindows(
				userVM,
				designVM,
				runVM,
				communicationSettings);
		}

		private void CreateWindows(
			UserViewModel userVM,
			DesignViewModel designVM,
			RunViewModel runVM,
			CommunicationViewModel communicationSettings)
		{
			_userView = new ContentControl();
			UserView userView = new UserView() { DataContext = userVM };
			_userView.Content = userView;
			SetHeader(_userView, "User");
			//SetCanClose(_userView, false);
			SetCanAutoHide(_userView, false);
			SetCanFloat(_userView, false);
			SetCanDocument(_userView, false);
			Children.Add(_userView);


			_communicationSettings = new ContentControl();
			CommunicationView communication = new CommunicationView() { DataContext = communicationSettings };
			_communicationSettings.Content = communication;
			SetHeader(_communicationSettings, "Communication Settings");
			SetFloatParams(_communicationSettings);
			Children.Add(_communicationSettings);


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





		public void ShowUser()
		{			
			SetState(_userView, DockState.Dock);
			SetCanClose(_userView, false);
		}

		public void HideUser()
		{
			SetCanClose(_userView, true);
			SetState(_userView, DockState.Hidden);
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
