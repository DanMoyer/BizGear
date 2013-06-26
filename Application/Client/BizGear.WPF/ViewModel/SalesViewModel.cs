namespace BizGear.WPF.ViewModel
{
	using EventSources;
	using EventSources.ApplicationEventSource;
	using GalaSoft.MvvmLight;
	using Microsoft.Practices.Unity;
	using SalesDTO;
	using System.Collections.ObjectModel;

	public class SalesViewModel : ViewModelBase
	{
		private readonly IEtwLogger _etwLogger;

		public GetCustomersCommand GetCustomersCommand { get; private set; }
		public ClearCustomersCommand ClearCustomersCommand { get; private set; }

		public ObservableCollection<CustomerDto> Customers { get; set; }

		public SalesViewModel([Dependency("Sales")] IEtwLogger etwLogger)
		{
			_etwLogger = etwLogger;

			GetCustomersCommand = new GetCustomersCommand(this);
			ClearCustomersCommand = new ClearCustomersCommand(this);

			Customers = new ObservableCollection<CustomerDto>();
		}

		public static ILogStrategy GetSalesApplicationLog()
		{
			var salesAppLog = ApplicationLogFactory.Create(Context.Sales, 
															Layer.Presentation, 
															"SalesViewModel", 
															"GetAllCustomersCommand");
			return salesAppLog;
		}
	}
}
