namespace LoggingBehaviors
{
	using EventSources;
	using EventSources.Adapters;
	using EventSources.ApplicationEventSource;


	public class EtwLoggingFactory
	{
		public static IEtwLogger CreateLogger(Context context)
		{
			IEtwLogger etwLogger = null;
			switch (context)
			{
				case Context.Sales:
					etwLogger = new SalesContextEventSourceAdapter();
					break;
				case Context.MerchandiseReturns:
					etwLogger = new MerchandiseReturnContextEventSourceAdapter();
					break;
				case Context.Business:
					etwLogger = new BusinessContextEventSourceAdapter();
					break;
			}

			return etwLogger;
		}
	}
}
