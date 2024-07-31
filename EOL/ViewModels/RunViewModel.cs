
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Syncfusion.DocIO.ReaderWriter;
using System.Collections.ObjectModel;
using System.Windows;

namespace EOL.ViewModels
{
	public class RunViewModel:ObservableObject
	{
		#region Properties

		public enum RunStateEnum { None, Running, Ended }

		public int RunPercentage { get; set; }

		public RunStateEnum RunState { get; set; }

		public ObservableCollection<string> TerminalTextsList { get; set; }

		#endregion Properties

		#region Constructor

		public RunViewModel()
		{
			RunCommand = new RelayCommand(Run);

			RunPercentage = 0;
			TerminalTextsList = new ObservableCollection<string>();
			RunState = RunStateEnum.None;
		}

		#endregion Constructor

		#region Methods

		private void Run()
		{
			MessageBox.Show("Run");
		}

		#endregion Methods

		#region Commands

		public RelayCommand RunCommand { get; private set; }

		#endregion Commands
	}
}
