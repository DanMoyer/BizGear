using EventSources;
using EventSources.EventSources;

namespace MerchandiseReturnService
{
	#region usings

	using BusinessRules;
	using Interfaces;
	using MerchandiseReturnDataAccess;
	using MerchandiseReturnDTO;

	#endregion

	//TODO: add wcf unity behavior to remove default constructor
	//TODO: Add "AOP" behaviors to instrument the RPC call CanREeturnMerchandise
	//TODO: Add Unity AOP on IMerchandiseRepository and ICanREturnMerchandiseRule methods

	public class MerchandiseReturnService : IMerchandiseReturnService
	{
		#region members

		private readonly ICanReturnMerchandiseRule _canReturnMerchandiseRule;
		private readonly IMerchandiseReturnRepository _merchandiseRepository;

		#endregion

		public MerchandiseReturnService()
		{
			_canReturnMerchandiseRule = new CanReturnMerchandiseRule();
			_merchandiseRepository = new MerchandiseReturnRepository();
		}

		public MerchandiseReturnService(
			ICanReturnMerchandiseRule canReturnMerchandiseRule,
			IMerchandiseReturnRepository merchandiseReturnRepository
			)
		{
			var etwTrace = new MerchandiseReturnContextEventSource();
			var enabled = etwTrace.IsEnabled();

			_canReturnMerchandiseRule = canReturnMerchandiseRule;
			_merchandiseRepository    = merchandiseReturnRepository;
		}

		public bool CanReturnMerchandise(MerchandiseDto dto)
		{
			var product = _merchandiseRepository.GetProductById(dto.ProductId);
			var salesOrder = _merchandiseRepository.GetSalesOrderById(dto.SalesOrderId);

			return _canReturnMerchandiseRule.CanReturnMerchandise(product, salesOrder);
		}
	}
}
