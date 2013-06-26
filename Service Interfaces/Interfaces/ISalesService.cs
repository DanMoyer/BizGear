namespace Interfaces
{
	#region usings

	using System.Runtime.Serialization;
	using SalesDTO;
	using System.Collections.Generic;
	using System.ServiceModel;
	using LoggingBehaviors;
	using Context = EventSources.ApplicationEventSource.Context;

	#endregion

	[ServiceContract]
	public interface ISalesService
	{
		[OperationContract]
		//[EtwLoggingOperationBehavior(Context.Sales)]
		IList<CustomerDto> GetAllCustomers();
	}
}
