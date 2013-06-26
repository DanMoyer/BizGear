namespace BizGear.WPF.ViewModel
{
	using System;
	using System.Windows.Input;

	public class ClearCustomersCommand : ICommand
	{
		private readonly SalesViewModel _viewModel;

		public ClearCustomersCommand(SalesViewModel viewModel)
		{
			_viewModel = viewModel;
		}

		public bool CanExecute(object parameter)
		{
			return true;
		}

		public void Execute(object parameter)
		{
			_viewModel.Customers.Clear();
		}

		public event EventHandler CanExecuteChanged;
	}
}