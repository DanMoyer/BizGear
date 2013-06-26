namespace EventSources.ApplicationEventSource
{
	public interface IApplicationLogFactory
	{
		ApplicationLog Create(Context context, Layer layer, string className, string methodName);
	}
}