using System.ComponentModel.DataAnnotations;

namespace mobilePark.Models
{
	public class MeteoDataPackage
	{
		[Key]
		public Guid Id { get; set; } = Guid.NewGuid();
		public long DataPackageID { get; set; }
		public string EmulatorID { get; set; }
		public List<SensorData> SensorData { get; set; }

	}
}
