using System.Diagnostics;
using System.Runtime.Serialization;

namespace SalesDTO
{
	[DebuggerDisplay("{DebuggerDisplay, nq}")]
	[DataContract]
	public class CustomerDto
	{
		[DataMember]
		public string FirstName { get; set; }

		[DataMember]
		public string LastName { get; set; }


		public string DebuggerDisplay
		{
			get
			{
				return string.Format("FirstName {0} LastName {1} ",
									 FirstName, LastName);
			}
		}
	}
}
