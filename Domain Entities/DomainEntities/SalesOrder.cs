using System;

namespace DomainEntities
{
	public class SalesOrder
	{
		public int Id { get; set; }
		public DateTime SalesDate { get; set; }
		public int ProductId { get; set; }
	}
}