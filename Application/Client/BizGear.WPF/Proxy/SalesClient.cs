using System.Collections.Generic;
using System.ServiceModel;
using Interfaces;
using SalesDTO;

namespace BizGear.WPF.Proxy
{
	#region usings

	

	#endregion

	public class SalesClient : ClientBase<ISalesService>, ISalesService
	{
		public IList<CustomerDto> GetAllCustomers()
		{
			return Channel.GetAllCustomers();
		}
	}
}
