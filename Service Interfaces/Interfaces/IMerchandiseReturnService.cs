namespace Interfaces
{
	#region usings

	using MerchandiseReturnDTO;
	using System.ServiceModel;

	#endregion

	[ServiceContract]
	public interface IMerchandiseReturnService
	{
		[OperationContract]
		bool CanReturnMerchandise(MerchandiseDto dto);
	}
}
