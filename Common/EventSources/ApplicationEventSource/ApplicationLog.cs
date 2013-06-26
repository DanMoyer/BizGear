namespace EventSources.ApplicationEventSource
{
	using System;

	public class ApplicationLog : ILogStrategy
	{
		private ILogStrategy _logStrategy;

		public ApplicationLog(ILogStrategy logStrategy, string className, string methodName, Layer layer)
		{
			_logStrategy = logStrategy.Clone(className, methodName, layer);

			Enter();
		}

		public ILogStrategy Clone(string className, string methodName, Layer layer)
		{
			throw new NotImplementedException();
		}

		public void Dispose()
		{
			_logStrategy.Dispose();
			_logStrategy = null;
		}

		public void Enter()
		{
			_logStrategy.Enter();
		}

		public void Log(string data)
		{
			_logStrategy.Log(data);
		}
	}
}
