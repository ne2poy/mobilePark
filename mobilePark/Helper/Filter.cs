using Microsoft.CodeAnalysis;
using mobilePark.Models;

namespace mobilePark.Helper
{
	public static class Filter
	{

		private static double k = 0.1;  // коэффициент фильтрации, 0.0-1.0
		private static Dictionary<string, MeteoDataDbModel> lastMeteoDataDictionary = new();

		public static double GetLastSensorValue(string emulatorId, string sensorName)
		{
			if (lastMeteoDataDictionary.TryGetValue(emulatorId, out var lastMeteoData))
			{
				var lastSensorData = lastMeteoData.SensorData.Find(x => x.SensorName == sensorName);
				if (lastSensorData != null) return lastSensorData.SensorValue;
			}
			return 0;
		}

		public static List<SensorData> FilteringDataNoise(MeteoDataDbModel meteoData)
		{
			return meteoData.SensorData.Select(x => new SensorData()
			{
				Id = Guid.NewGuid(),
				SensorName = x.SensorName,
				SensorValue = RunningAverage(x.SensorValue, GetLastSensorValue(meteoData.EmulatorID, x.SensorName)),
			}).ToList();
		}

		public static double RunningAverage(double newVal, double filVal)
		{
			var filter = (newVal - filVal) * k;
			return newVal + filter;
		}
	}
}
