
namespace MerchandiseReturnDTO
{
	using System.Runtime.Serialization;

	[DataContract]
	public class MerchandiseDto
	{
		[DataMember]
		public int ProductId { get; set; }

		[DataMember]
		public int SalesOrderId { get; set; }
	}
}
