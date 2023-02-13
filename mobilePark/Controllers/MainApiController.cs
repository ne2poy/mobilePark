using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using mobilePark.Models;
using mobilePark.Services;
using System.Globalization;
using System.Net;

namespace mobilePark.Controllers
{
	[ApiController]
	[Route("")]
	public class MainApiController : ControllerBase
	{
		private readonly ILogger<MainApiController> _logger;
		private readonly MeteoService meteoService;

		public MainApiController(ILogger<MainApiController> logger, MeteoService meteoService)
		{
			_logger = logger;
			this.meteoService = meteoService;
		}

		[HttpPut("data")]
		public async Task<IActionResult> PutData([FromBody] MeteoDataDbModel package)
		{
			package.DataType = DataTypeEnum.src;
			await meteoService.SaveData(package);
			return Ok();
		}

		[HttpPut("noiseData")]
		public async Task<IActionResult> PutNoiseData([FromBody] MeteoDataDbModel package)
		{
			await meteoService.SaveNoiseData(package);
			return Ok();
		}

		[HttpGet("statistics")]
		public async Task<IActionResult> GetStatistics()
		{
			return Ok(await meteoService.GetStatistics());
		}

		[HttpGet("report")]
		public async Task<IActionResult> GetAggregatedReportBySurvey([FromHeader] string emulatorId, string sensorName)
		{
			var records = await meteoService.GetCsvByMeteoEmulator(emulatorId, sensorName);
			using var memoryStream = new MemoryStream();
			using (var writer = new StreamWriter(memoryStream))
			using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
			{
				csv.WriteRecords(records);

				return File(memoryStream.ToArray(), "text/csv", "test.scv");
			}
		}
	}
}