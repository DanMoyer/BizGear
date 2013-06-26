using System;

namespace BizGear.Model
{
	public interface IDataService
	{
		void GetData(Action<DataItem, Exception> callback);
	}
}
