namespace BusinessRules
{
	#region usings

	using DomainEntities;
	using EventSources.EventSources;
	using Microsoft.Practices.Unity.InterceptionExtension;

	#endregion
	class CanReturnMerchandiseRuleHandler : ICallHandler
	{
		public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
		{
			var productData = string.Empty;
			var salesData = string.Empty;

			var etwTracer =  MerchandiseReturnContextEventSource.Log;

			if (etwTracer.IsEnabled())
			{
				var methodName = input.MethodBase.Name;
				var className = input.Target.ToString();

				var product = input.Inputs[0] as Product;
				if (product != null)
				{
					productData = string.Format("Product: Id {0} Description {1}  ", product.Id, product.Description);
				}

				var salesOrder = input.Inputs[1] as SalesOrder;
				if (salesOrder != null)
				{
					salesData = string.Format("SalesOrder Id {0} SalesDate {1} ", salesOrder.Id, salesOrder.SalesDate);
				}

				etwTracer.Business(className, methodName, productData + salesData);
			}

			// Perform the operation
			var methodReturn = getNext().Invoke(input, getNext);

			return methodReturn;
		}

		public int Order { get; set; }
	}
}
