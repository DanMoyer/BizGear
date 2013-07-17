namespace BizGear.WPF.ViewModel
{
	using System;
	using System.Windows.Input;
	using EventSources.ApplicationEventSource;
	using Proxy;

	public class GetCustomersCommand : ICommand
	{
		private readonly SalesViewModel _viewModel;

		public GetCustomersCommand(SalesViewModel viewModel) 
		{
			_viewModel = viewModel;
		}


		public void Execute(object parameter)
		{
			using (var appLog = ApplicationLogFactory.Create(Context.Sales,
															Layer.Presentation,
															"SalesViewModel",
															"GetCustomersCommand"))
			{
				_viewModel.Customers.Clear();

				using (var proxy = new SalesClient())
				{
					var customers = proxy.GetAllCustomers();

					foreach (var customer in customers)
					{
						_viewModel.Customers.Add(customer);
					}
				}
			}
		}

		public bool CanExecute(object parameter)
		{
			return true;
		}

		public event EventHandler CanExecuteChanged;
	}
}


/*
			using (var etwEnvelope = SalesViewModel.GetSalesApplicationLog())
			{
				_viewModel.Customers.Clear();

				using (var proxy = new SalesClient())
				{
					//var clientCookie = "USER_IDENTITY";
					//etwEnvelope.Log(clientCookie);
					//var etwLoggBehavior = new EtwLoggingEnpointBehavior(new SalesContextEventSourceAdapter(), clientCookie);
					//proxy.ChannelFactory.Endpoint.Behaviors.Add(etwLoggBehavior);

					//IList<CustomerDto> customers = new List<CustomerDto>();
					//for (var ctr = 0; ctr < 1000; ctr++)
					//{
					//	customers = proxy.GetAllCustomers();
					//}

					var customers = proxy.GetAllCustomers();

					foreach (var customer in customers)
					{
						_viewModel.Customers.Add(customer);

						//etwEnvelope.Log(customer.FirstName + " " + customer.LastName);
					}
				}
			}	

*/