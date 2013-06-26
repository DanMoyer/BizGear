using DomainEntities;

namespace MerchandiseReturnDataAccess
{
	public interface IMerchandiseReturnRepository
	{
		Product GetProductById(int id);

		SalesOrder GetSalesOrderById(int id);
	}
}
