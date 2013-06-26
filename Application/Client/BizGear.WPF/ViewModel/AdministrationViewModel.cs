namespace BizGear.WPF.ViewModel
{
	using System.Collections.ObjectModel;
	using System.Diagnostics;
	using BizGear2;
	using GalaSoft.MvvmLight;
	using System.Diagnostics.Tracing;
	using GalaSoft.MvvmLight.Command;
	using System;
	using EventSources.EventSources;
	using ServiceBus.Contracts;

	public class SalesContextListener : EventListener
	{
		private AdministrationViewModel _viewModel;

		public SalesContextListener(AdministrationViewModel viewModel)
		{
			_viewModel = viewModel;
		}

		protected override void OnEventWritten(EventWrittenEventArgs eventData)
		{
			var eventMessage = new EventMessage
				                   {
									   Source     = eventData.EventSource.Name,
					                   ClassName  = eventData.Payload[0].ToString(),
					                   MethodName = eventData.Payload[1].ToString(),
					                   Data       = eventData.Payload[2].ToString()
				                   };

			_viewModel.EventMessages.Add(eventMessage);

			var etwMessage = new EtwMessage
				                 {
					                 Source     = eventData.EventSource.Name,
					                 ClassName  = eventData.Payload[0].ToString(),
					                 MethodName = eventData.Payload[1].ToString(),
					                 Data       = eventData.Payload[2].ToString()
				                 };

			App.Bus.Send("backend", etwMessage);
		}

		protected override void OnEventSourceCreated(EventSource eventSource)
		{
			base.OnEventSourceCreated(eventSource);
			//EnableEvents(eventSource, EventLevel.Verbose);
		}
	}

	[DebuggerDisplay("{DebuggerDisplay, nq}")]
	public class EventMessage
	{
		public string Source { get; set; }
		public string ClassName { get; set; }
		public string MethodName  {get; set;}
		public string Data { get; set; }

		public string DebuggerDisplay
		{
			get
			{
				return string.Format("ClassName {0} MethodName {1}, Data {2}",
									 ClassName, MethodName, Data);
			}
		}
	}

	public class AdministrationViewModel : ViewModelBase, IDisposable
	{
		#region declarations

		private SalesContextListener _salesListener;
		private readonly EventKeywords _salesKeywords;
		private bool _canStartExecute;
		private bool _canStopExecute;
		private bool _canClearExecute;

		public RelayCommand StartCommand { get; private set; }
		public RelayCommand StopCommand { get; private set; }
		public RelayCommand ClearCommand { get; private set; }

		#endregion
		
		public AdministrationViewModel()
		{
			_salesListener = new SalesContextListener(this);

			EventMessages = new ObservableCollection<EventMessage>();

			_salesKeywords = SalesContextEventSource.Keywords.Business |
							 SalesContextEventSource.Keywords.DataAccess |
							 SalesContextEventSource.Keywords.Presentation |
							 SalesContextEventSource.Keywords.Service;

			StartCommand = new RelayCommand(StartExecute, () => CanStartExecute);
			StopCommand = new RelayCommand(StopExecute, () => CanStopExecute);
			ClearCommand = new RelayCommand(ClearExecute, () => CanClearExecute);

			_canStartExecute = true;
			_canClearExecute = true;
		}

		public ObservableCollection<EventMessage> EventMessages { get; set; }

		private void StartExecute()
		{
			_salesListener.EnableEvents(SalesContextEventSource.Log, EventLevel.Verbose, _salesKeywords);
			_canStartExecute = false;
			_canClearExecute = true;
			_canStopExecute = true;
		}

		private bool CanStartExecute
		{
			get { return _canStartExecute; }

			set
			{
				if (_canStartExecute == value)
				{
					return;
				}

				_canStartExecute = value;

				RaisePropertyChanged("CanStartExecute");
			}
		}

		private void StopExecute()
		{
			_salesListener.DisableEvents(SalesContextEventSource.Log);
			_canStartExecute = true;
			_canStopExecute = false;

		}

		private bool CanStopExecute
		{
			get { return _canStopExecute; }

			set
			{
				if (_canStopExecute == value)
				{
					return;
				}

				_canStopExecute = value;

				RaisePropertyChanged("CanStopExecute");
			}
		}
		
		private void ClearExecute()
		{
			EventMessages.Clear();
		}

		private bool CanClearExecute
		{
			get { return _canClearExecute; }

			set
			{
				if (_canStopExecute == value)
				{
					return;
				}

				_canClearExecute = value;

				RaisePropertyChanged("CanClearExecute");
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_salesListener != null)
				{
					_salesListener.Dispose();
					_salesListener = null;
				}
			}
		}

	}
}
