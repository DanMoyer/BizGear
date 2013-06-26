

namespace SalesService
{
	#region usings
	
	using EventSources.ApplicationEventSource;
	using Interfaces;
	using LoggingBehaviors;
	using SalesDTO;
	using SalesDataAccess;
	using System.Collections.Generic;
	
	#endregion

	[EtwLoggingServiceBehavior(Context.Sales)]
	public class SalesService : ISalesService
	{
		//private readonly IEtwLogger _etwLogger;
		private readonly ILogStrategy _logStrategy;
		private readonly ISalesRepository _salesRespository;


		/* Default constructor needed when not using the Unit.Wcf library to inject
		 * dependent objects via IInstanceProvider
		 */

		public SalesService()
		{
			_logStrategy         = new SalesLogStrategy();
			_salesRespository    = new SalesRepository();
		}

		public IList<CustomerDto> GetAllCustomers()
		{
			return _salesRespository.GetAllCustomers();
		}
	}
}
