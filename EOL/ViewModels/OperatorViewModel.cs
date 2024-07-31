
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DeviceHandler.Models;
using EOL.Models;
using ScriptHandler.Models;

namespace EOL.ViewModels
{
	public class OperatorViewModel:ObservableObject
	{
		public enum ScriptStateEnum { Running, Pass, Fail, None, }

		#region Properties

		public RunData RunData { get; set; }

		#endregion Properties

		#region Fields



		#endregion Fields

		#region Constructor

		public OperatorViewModel(
			DevicesContainer devicesContainer,
			ScriptUserData scriptUserData)
		{
			RunData = new RunData();
		}

		

		#endregion Constructor

		#region Methods

		

		#endregion Methods

		#region Commands

		public RelayCommand RunCommand { get; private set; }

		#endregion Commands
	}
}
