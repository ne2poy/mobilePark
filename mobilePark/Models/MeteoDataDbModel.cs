namespace mobilePark.Models
{
	public class MeteoDataDbModel : MeteoDataPackage 
	{
		public DateTime Date { get; set; } = DateTime.UtcNow;
		public DataTypeEnum DataType { get; set; }

	}
}
