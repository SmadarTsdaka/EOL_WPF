
using Controls.ViewModels;
using EOL.Views;
using Syncfusion.Windows.Tools.Controls;
using System.Windows.Controls;

namespace EOL.ViewModels
{
	public class EOLDockingViewModel : DocingBaseViewModel
	{
		private ContentControl _userView;

		public EOLDockingViewModel(
			UserViewModel userVM) :
			base("EOL")
		{
			DockFill = true;

			CreateWindows(userVM);
		}

		private void CreateWindows(
			UserViewModel userVM)
		{
			_userView = new ContentControl();
			UserView userView = new UserView() { DataContext = userVM };
			_userView.Content = userView;
			SetHeader(_userView, "User");
			ShowUser();
			Children.Add(_userView);
		}

		public void ShowUser()
		{
			SetState(_userView, DockState.Dock);
		}

		public void HideUser()
		{
			SetState(_userView, DockState.Hidden);
		}
	}
}
