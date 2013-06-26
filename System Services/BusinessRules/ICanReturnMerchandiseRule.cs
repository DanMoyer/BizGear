namespace BusinessRules
{
	using DomainEntities;

	public interface ICanReturnMerchandiseRule
	{
		bool CanReturnMerchandise(Product product, SalesOrder salesOrder);
	}
}
