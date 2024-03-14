namespace CityPulse.Server.Data.Models
{
	public class Country
	{
		public int Id { get; set; }
		public required string Name { get; set; }
		public required string ISO2 { get; set; }
		public required string ISO3 { get; set; }
		public ICollection<City>? Cities { get; set; }

	}

}
