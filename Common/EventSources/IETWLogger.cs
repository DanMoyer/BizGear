namespace EventSources
{
	public interface IEtwLogger
	{
		bool IsEnabled();

		void Presentation(string className, string methodName, string data);

		void Business(string className, string methodName, string data);

		void Service(string className, string methodName, string data);

		void Data(string className, string methodName, string data);
	}
}
