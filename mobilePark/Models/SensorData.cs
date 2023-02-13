using System.ComponentModel.DataAnnotations;

namespace mobilePark.Models
{
	public class SensorData
	{
		[Key]
		public Guid Id { get; set; } = Guid.NewGuid();
		public string SensorName { get; set; }
		public double SensorValue { get; set; }
	}
}
