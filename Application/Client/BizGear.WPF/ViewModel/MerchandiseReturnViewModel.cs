namespace BizGear.WPF.ViewModel
{
	using Proxy;
	using EventSources;
	using GalaSoft.MvvmLight;
	using GalaSoft.MvvmLight.Command;
	using Microsoft.Practices.Unity;
	using MerchandiseReturnDTO;

	public class MerchandiseReturnViewModel : ViewModelBase
	{
		private bool _canMerchandiseReturnExecute;
		private bool _canMerchandiseClearExecute;
		private string _returnResult;
		private readonly IEtwLogger _etwLogger;

		public RelayCommand MerchandiseReturnCommand { get; private set; }
		public RelayCommand MerchandiseClearCommand { get; private set; }


		public MerchandiseReturnViewModel([Dependency("MerchandiseReturn")] IEtwLogger etwLogger)
		{
			_etwLogger                   = etwLogger;
			_canMerchandiseReturnExecute = true;
			_canMerchandiseClearExecute  = true;
			_returnResult                = string.Empty;

			MerchandiseReturnCommand = new RelayCommand(MerchandiseReturnExecute, () => CanMerchandiseReturnExecute);
			MerchandiseClearCommand  = new RelayCommand(MerchandiseClearExecute, () => CanMerchandiseClearExecute);
		}

		private void MerchandiseReturnExecute()
		{
			using (var proxy = new MerchandiseReturnClient())
			{
				var dto = new MerchandiseDto {ProductId = 1, SalesOrderId = 1};
				var canReturn = proxy.CanReturnMerchandise(dto);

				ReturnResult = "Complete";
				_etwLogger.Presentation("MerchandiseReturnViewModel", "MerchandiseReturnExecute", canReturn.ToString());
			}
		}

		public string ReturnResult
		{
			get
			{
				return _returnResult;
			}
			set
			{
				if (_returnResult == value)
				{
					return;
				}

				_returnResult = value;

				RaisePropertyChanged("ReturnResult");
			}
		}

		private bool CanMerchandiseReturnExecute
		{
			get
			{
				return _canMerchandiseReturnExecute;
			}
			set
			{
				if (_canMerchandiseReturnExecute == value)
				{
					return;
				}
				_canMerchandiseReturnExecute = value;

				RaisePropertyChanged("CanMerchandiseReturnExecute");
			}
		}

		private void MerchandiseClearExecute()
		{
			ReturnResult = string.Empty;
		}

		private bool CanMerchandiseClearExecute
		{
			get
			{
				return _canMerchandiseClearExecute;
			}

			set
			{
				if (_canMerchandiseClearExecute == value)
				{
					return;
				}

				_canMerchandiseClearExecute = value;

				RaisePropertyChanged("CanMerchandiseClearExecute");
			}

		}
	}
}
