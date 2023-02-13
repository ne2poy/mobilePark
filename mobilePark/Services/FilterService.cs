using mobilePark.Controllers;
using mobilePark.Helper;
using mobilePark.Models;

namespace mobilePark.Services
{
	public class FilterService
	{
		private List<SensorData> datas = new List<SensorData>();

		public FilterService()
		{
		}

		public void Add(SensorData sensorData)
		{
			datas.Add(sensorData);
		}

		public void GenerateFilteredValue()
		{

		}


		public List<SensorData> GetAll()
		{
			return datas;
		}
	}
}
