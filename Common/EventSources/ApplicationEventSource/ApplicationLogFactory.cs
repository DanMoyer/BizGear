namespace EventSources.ApplicationEventSource
{
	using System.Collections.Generic;

	public class ApplicationLogFactory : IApplicationLogFactory
	{
		private static readonly Dictionary<Context, ILogStrategy> Strategies = new Dictionary<Context, ILogStrategy>();

		static ApplicationLogFactory()
		{
			Strategies.Add(Context.Sales, new SalesLogStrategy());
			Strategies.Add(Context.Business, new BusinessLogStrategy());
			Strategies.Add(Context.MerchandiseReturns, new MerchandiseReturnLogStrategy());
		}

		public static ApplicationLog Create(Context context, Layer layer, string className, string methodName)
		{
			return new ApplicationLog(Strategies[context], className, methodName, layer);
		}

		ApplicationLog IApplicationLogFactory.Create(Context context, Layer layer, string className, string methodName)
		{
			return Create(context, layer, className, methodName);
		}
	}
}