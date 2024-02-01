
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
		private ContentControl _operatorView;
		private ContentControl _communicationSettings;
		private ContentControl _deviceSimulatorsViewModel;

		public EOLDockingViewModel(
			OperatorViewModel operatorVM,
			CommunicationViewModel communicationSettings,
			DeviceSimulatorsViewModel deviceSimulatorsViewModel) :
			base("EOL")
		{
			DockFill = true;

			CreateWindows(
				operatorVM,
				communicationSettings,
				deviceSimulatorsViewModel);
		}

		private void CreateWindows(
			OperatorViewModel operatorVM,
			CommunicationViewModel communicationSettings,
			DeviceSimulatorsViewModel deviceSimulatorsViewModel)
		{
			_operatorView = new ContentControl();
			OperatorView operatorView = new OperatorView() { DataContext = operatorVM };
			_operatorView.Content = operatorView;
			SetHeader(_operatorView, "Operator");
			//SetCanClose(_userView, false);
			SetCanAutoHide(_operatorView, false);
			SetCanFloat(_operatorView, false);
			SetCanDocument(_operatorView, false);
			Children.Add(_operatorView);


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

		}

		public void OpenCommSettings()
		{
			SetState(_communicationSettings, DockState.Float);
		}

		public void OpenDeviceSimulators()
		{
			SetState(_deviceSimulatorsViewModel, DockState.Float);
		}


	}
}
