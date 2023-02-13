using CsvHelper.Configuration.Attributes;

namespace mobilePark.Models
{
	public class CsvModel
	{
		[Name("DataPackageID")]
		public long DataPackageID { get; set; }

		[Name("Source")]
		public double? SrcSensorValue { get; set; }

		[Name("With noise")]
		public double? NoiseSensorValue { get; set; }

		[Name("Filtered noise")]
		public double? FilteredSensorValue { get; set; }
	}
}
