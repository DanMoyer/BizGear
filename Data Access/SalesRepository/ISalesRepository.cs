using System.Collections.Generic;
using SalesDTO;

namespace SalesDataAccess
{
	#region usings

	

	#endregion

	public interface ISalesRepository
	{
		IList<CustomerDto> GetAllCustomers();
	}
}
