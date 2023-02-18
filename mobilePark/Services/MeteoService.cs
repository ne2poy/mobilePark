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
		private readonly FilterService filterService;

		public MeteoService(ILogger<MainApiController> logger, MeteoDbContext dbContext, FilterService filterService)
		{
			this.logger = logger;
			this.dbContext = dbContext;
			this.filterService = filterService;
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
			var packages = await dbContext.MeteoDataPackages
				.ToArrayAsync();

			return packages.GroupBy(x => x.EmulatorID).Select(m => new StatisticsModel 
			{
				MeteoStation = m.Key,
				PackageCount = m.Select(p => p.DataPackageID).Distinct().LongCount()
			});
		}

		public async Task<IEnumerable<CsvModel>> GetCsvByMeteoEmulator(string emulatorId, string sensorName)
		{
			var packages = await dbContext.MeteoDataPackages
				.Include(x => x.SensorData)
				.Where(x => x.EmulatorID == emulatorId)
				.ToArrayAsync();

			var res = packages.GroupBy(x => x.DataPackageID).Select(m => 
			{
				return new CsvModel()
				{
					DataPackageID = m.Key,
					SrcSensorValue = m.FirstOrDefault(s => s.DataType == DataTypeEnum.src)?.SensorData.FirstOrDefault(x => x.SensorName == sensorName)?.SensorValue,
					NoiseSensorValue = m.FirstOrDefault(s => s.DataType == DataTypeEnum.noise)?.SensorData.FirstOrDefault(x => x.SensorName == sensorName)?.SensorValue,
					FilteredSensorValue = m.FirstOrDefault(s => s.DataType == DataTypeEnum.filtered)?.SensorData.FirstOrDefault(x => x.SensorName == sensorName)?.SensorValue,
				};
			}).ToList();

			return res;
		}
	}
}
