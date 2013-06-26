namespace EventSources.ApplicationEventSource
{
	public class LogStrategyBase : ILogStrategy
	{
		public string ClassName { get; set; }
		public string MethodName { get; set; }
		public Layer Layer { get; set; }

		public virtual ILogStrategy Clone(string className, string methodName, Layer layer)
		{
			return null;
		}

		public virtual void Dispose()
		{
		}

		public virtual void Enter()
		{
		}

		public virtual void Log(string data)
		{
		}
		}
}
