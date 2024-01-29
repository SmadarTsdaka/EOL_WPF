
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Windows;

namespace EOL.ViewModels
{
	public class UserViewModel:ObservableObject
	{
		public enum ScriptStateEnum { Running, Pass, Fail, None, }

		#region Properties

		public ScriptStateEnum ScriptState { get; set; }

		public int ProgressPercentage { get; set; }

		public string CurrentStepName { get; set; }

		#endregion Properties

		#region Constructor

		public UserViewModel()
		{
			RunCommand = new RelayCommand(Run);

			ScriptState = ScriptStateEnum.None;
			ProgressPercentage = 0;
		}

		#endregion Constructor

		#region Methods

		private void Run()
		{
			ScriptState = ScriptStateEnum.Running;

			Task.Run(() =>
			{
				for (int i = 0; i < 100; i += 10)
				{
					Application.Current.Dispatcher.Invoke(() =>
					{
						ProgressPercentage = i;
						switch (ProgressPercentage)
						{
							case 0: CurrentStepName = "Set  \"Voltage\" = 48234 - ID:1"; break;
							case 10: CurrentStepName = "Delay 3 sec - ID:2"; break;
							case 20: CurrentStepName = "Set  \"Turn ON channel\" = 0 - ID:3"; break;
							case 30: CurrentStepName = "Increment  \"Manual Throttle (MCU)\" By 5 - ID:4"; break;
							case 40: CurrentStepName = "Compare Range: \"Manual Throttle (MCU)\" = \"Motor Kv (MCU)\" = \"Manual FOC Control (MCU)\" - ID:5"; break;
							case 50: CurrentStepName = "Converge  \"Manual Throttle (MCU)\" = 50 - ID:6"; break;
							case 60: CurrentStepName = "Dynamic Control - ID:1"; break;
							case 70: CurrentStepName = "Set  \"Turn ON channel\" = 0 - ID:7"; break;
							case 80: CurrentStepName = "Set \"Manual Threottle\" = " + i + "-ID:8"; break;
							case 90: CurrentStepName = "Set \"Manual Threottle\" = " + i + "-ID:9"; break;
						}
					});

					System.Threading.Thread.Sleep(1000);
				}

				CurrentStepName = null;
				ScriptState = ScriptStateEnum.Pass;
				ProgressPercentage = 100;
			});
		}

		#endregion Methods

		#region Commands

		public RelayCommand RunCommand { get; private set; }

		#endregion Commands
	}
}
