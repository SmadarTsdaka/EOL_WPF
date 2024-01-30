
using Controls.ViewModels;
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

		public EOLDockingViewModel(
			UserViewModel userVM,
			DesignViewModel designVM,
			RunViewModel runVM) :
			base("EOL")
		{
			DockFill = true;

			CreateWindows(
				userVM,
				designVM,
				runVM);
		}

		private void CreateWindows(
			UserViewModel userVM,
			DesignViewModel designVM,
			RunViewModel runVM)
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


			DesignView designView = new DesignView() { DataContext = designVM };
			CreateTabbedWindow(designView, "Design", string.Empty, out _designView);
			SetDesiredWidthInDockedMode(_designView, 1200);

			RunView runView = new RunView() { DataContext = runVM };
			CreateTabbedWindow(runView, "Run", "Design", out _runView);
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
		}

		public void HideAdmin()
		{
			SetState(_designView, DockState.Hidden);
			SetState(_runView, DockState.Hidden);
		}
	}
}
