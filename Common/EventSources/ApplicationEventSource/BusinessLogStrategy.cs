namespace EventSources.ApplicationEventSource
{
	using EventSources;

	public class BusinessLogStrategy : LogStrategyBase
	{
		public override ILogStrategy Clone(string className, string methodName, Layer layer)
		{
			return new BusinessLogStrategy { ClassName = className, MethodName = methodName, Layer = layer };
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
					BusinessContextEventSource.Log.Presentation(ClassName, MethodName, data);
					break;

				case Layer.Business:
					BusinessContextEventSource.Log.Business(ClassName, MethodName, data);
					break;

				case Layer.Data:
					BusinessContextEventSource.Log.Data(ClassName, MethodName, data);
					break;
			}
		}

		private static bool IsEnabled()
		{
			return BusinessContextEventSource.Log.IsEnabled();
		}
	}
}
