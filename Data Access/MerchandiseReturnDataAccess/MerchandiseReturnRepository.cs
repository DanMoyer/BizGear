

namespace MerchandiseReturnDataAccess
{
	#region using
	
		using System;
		using DomainEntities;

	#endregion

	public class MerchandiseReturnRepository : IMerchandiseReturnRepository
	{
		public Product GetProductById(int id)
		{
			return new Product{Id = 1, Description = "Computer"};
		}

		public SalesOrder GetSalesOrderById(int id)
		{
			return new SalesOrder {Id = 1, SalesDate = new DateTime(2013, 11, 15), ProductId = 1};
		}
	}

}