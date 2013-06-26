namespace EventSources.ApplicationEventSource
{
	using System;

	public interface ILogStrategy : IDisposable
	{
		ILogStrategy Clone(string className, string methodName, Layer layer);

		void Enter();
		void Log(string data);
	}
}
