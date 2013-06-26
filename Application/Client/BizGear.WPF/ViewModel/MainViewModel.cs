namespace BizGear.ViewModel
{
	using Model;
	using GalaSoft.MvvmLight;
	using GalaSoft.MvvmLight.Command;


	/// <summary>
	/// This class contains properties that the main View can data bind to.
	/// <para>
	/// See http://www.galasoft.ch/mvvm
	/// </para>
	/// </summary>
	public class MainViewModel : ViewModelBase
	{
		private readonly IDataService _dataService;
		private bool _canIncrement = true;
		private int _counter;

		public int Counter
		{
			get { return _counter; }
			private set 
			{
				if (_counter == value) return;
				
				_counter = value;
				RaisePropertyChanged("Counter");
			}
		}


		private string _welcomeTitle = string.Empty;

		/// <summary>
		/// Gets the WelcomeTitle property.
		/// Changes to that property's value raise the PropertyChanged event. 
		/// </summary>
		public string WelcomeTitle
		{
			get
			{
				return _welcomeTitle;
			}

			set
			{
				if (_welcomeTitle == value)
				{
					return;
				}

				_welcomeTitle = value;
				RaisePropertyChanged("WelcomeTitle");
			}
		}

		public RelayCommand IncreaseCounterCommand { get; private set; }

		/// <summary>
		/// Initializes a new instance of the MainViewModel class.
		/// </summary>
		public MainViewModel(IDataService dataService)
		{
			_dataService = dataService;
			_dataService.GetData(
				(item, error) =>
				{
					if (error != null)
					{
						// Report error here
						return;
					}

					WelcomeTitle = item.Title;
				});

			IncreaseCounterCommand = new RelayCommand(() => Counter++, () => CanIncrement);
		}

		public bool CanIncrement
		{
			get
			{
				return _canIncrement;
			}

			set
			{
				if (_canIncrement == value)
				{
					return;
				}

				_canIncrement = value;

				// Update bindings, no broadcast
				RaisePropertyChanged("CanIncrement");

				IncreaseCounterCommand.RaiseCanExecuteChanged();
			}
		}

		////public override void Cleanup()
		////{
		////    // Clean up if needed

		////    base.Cleanup();
		////}
	}
}