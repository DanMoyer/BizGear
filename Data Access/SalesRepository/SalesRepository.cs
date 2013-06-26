using System.Collections.Generic;
using SalesDTO;

namespace SalesDataAccess
{
	#region usings

	

	#endregion

	public class SalesRepository : ISalesRepository
	{
		public IList<CustomerDto> GetAllCustomers()
		{
			var customers = new List<CustomerDto>
				                {
					                new CustomerDto {FirstName = "john", LastName = "smith"},
					                new CustomerDto {FirstName = "jane", LastName = "doe"}
				                };

			return customers;
		}
	}
}
