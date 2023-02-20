using Microsoft.EntityFrameworkCore;
using mobilePark.Controllers;
using mobilePark.Helper;
using mobilePark.Models;

namespace mobilePark.Services
{
	public class MeteoService
	{
		private readonly ILogger<MainApiController> logger;
		private readonly MeteoDbContext dbContext;

		public MeteoService(ILogger<MainApiController> logger, MeteoDbContext dbContext)
		{
			this.logger = logger;
			this.dbContext = dbContext;
		}

		public async Task SaveNoiseData(MeteoDataDbModel meteoData)
		{
			meteoData.DataType = DataTypeEnum.noise;
			await SaveData(meteoData);

			var filteredSensorData = Filter.FilteringDataNoise(meteoData);
			Filter.AddMeteoData(meteoData);

			var filteredMeteoData = new MeteoDataDbModel()
			{
				Id = Guid.NewGuid(),
				EmulatorID = meteoData.EmulatorID,
				DataPackageID = meteoData.DataPackageID,
				DataType = DataTypeEnum.filtered,
				Date = meteoData.Date,
				SensorData = filteredSensorData,
			};
			await SaveData(filteredMeteoData);
		}

		public async Task SaveData(MeteoDataDbModel meteoData)
		{
			logger.LogDebug("Saving data...");
			await dbContext.MeteoDataPackages.AddAsync(meteoData);
			await dbContext.SaveChangesAsync();
			logger.LogDebug("Saving success!");
		}

		public async Task<IEnumerable<StatisticsModel>> GetStatistics()
		{
			return await dbContext.MeteoDataPackages.GroupBy(x => x.EmulatorID).Select(m => new StatisticsModel
			{
				MeteoStation = m.Key,
				PackageCount = m.Select(p => p.DataPackageID).Distinct().LongCount()
			})
			.ToListAsync();
		}

		public async Task<IEnumerable<CsvModel>> GetCsvByMeteoEmulator(string emulatorId, string sensorName)
		{
            var res = await dbContext.MeteoDataPackages
				.Include(x => x.SensorData)
				.Where(x => x.EmulatorID == emulatorId)
				.GroupBy(x => x.DataPackageID)
				.Select(m => new CsvModel()
				{
					DataPackageID = m.Key,
					SrcSensorValue = m.First(s => s.DataType == DataTypeEnum.src).SensorData.First(x => x.SensorName == sensorName).SensorValue,
					NoiseSensorValue = m.First(s => s.DataType == DataTypeEnum.noise).SensorData.First(x => x.SensorName == sensorName).SensorValue,
					FilteredSensorValue = m.First(s => s.DataType == DataTypeEnum.filtered).SensorData.First(x => x.SensorName == sensorName).SensorValue,
				})
				.ToListAsync();

			return res;
		}
	}
}
