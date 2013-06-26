namespace EventSources.ApplicationEventSource
{
	using EventSources;

	public class MerchandiseReturnLogStrategy : LogStrategyBase
	{
		public override ILogStrategy Clone(string className, string methodName, Layer layer)
		{
			return new MerchandiseReturnLogStrategy { ClassName = ClassName, MethodName = MethodName, Layer = Layer };
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
					MerchandiseReturnContextEventSource.Log.Presentation(ClassName, MethodName, data);
					break;

				case Layer.Business:
					MerchandiseReturnContextEventSource.Log.Business(ClassName, MethodName, data);
					break;

				case Layer.Data:
					MerchandiseReturnContextEventSource.Log.Data(ClassName, MethodName, data);
					break;
			}
		}

		private static bool IsEnabled()
		{
			return MerchandiseReturnContextEventSource.Log.IsEnabled();
		}
	}
}
