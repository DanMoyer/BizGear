namespace LoggingBehaviors
{
	using System.Runtime.Serialization;

	[DataContract]
	public class CustomEtwLoggingHeader 
	{
		public const string Name = "EtwHeader";
		public const string NameSpace = @"http://www.w3.org/2005/08/addressing";

		[DataMember]
		public string EtwCookie;

	}
}