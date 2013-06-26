using System.ServiceModel;
using Interfaces;
using MerchandiseReturnDTO;

namespace BizGear.WPF.Proxy
{
	public class MerchandiseReturnClient : ClientBase<IMerchandiseReturnService>, IMerchandiseReturnService
	{
		public bool CanReturnMerchandise(MerchandiseDto dto)
		{
			return Channel.CanReturnMerchandise(dto);
		}
	}
}
