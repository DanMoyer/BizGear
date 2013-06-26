namespace EventSources.ApplicationEventSource
{
	using EventSources;

	public class SalesLogStrategy : LogStrategyBase
	{
		public override ILogStrategy Clone(string className, string methodName, Layer layer)
		{
			return new SalesLogStrategy {ClassName = className, MethodName = methodName, Layer = layer};
		}

		public override void Dispose()
		{
			Log("exit");
		}

		public override void Enter()
		{
			Log("enter");
		}
		
		public override void Log(string data)
		{
			if (!IsEnabled()) return;

			switch (Layer)
			{
				case Layer.Presentation:
					SalesContextEventSource.Log.Presentation(ClassName, MethodName, data);
					break;

				case Layer.Business:
					SalesContextEventSource.Log.Business(ClassName, MethodName, data);
					break;

				case Layer.Data:
					SalesContextEventSource.Log.Data(ClassName, MethodName, data);
					break;
			}
		}

		private static bool IsEnabled()
		{
			return SalesContextEventSource.Log.IsEnabled();
		}
	}
}
