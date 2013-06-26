namespace BusinessRules
{
	using DomainEntities;

	public class CanReturnMerchandiseRule : ICanReturnMerchandiseRule
	{
		[Performance]
		public bool CanReturnMerchandise(Product product, SalesOrder salesOrder)
		{
			return ValidateProductIsReturnable(product) && ValidateReturnTimePeriod(product, salesOrder);
		}

		//Validate product is not discontinued or purchased "as is"
		private bool ValidateProductIsReturnable(Product product)
		{
			return true;
		}

		private bool ValidateReturnTimePeriod(Product product, SalesOrder salesOrder)
		{
			return true;
		}
	}
}
