using CityPulse.Server.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.Security;

namespace CityPulse.Server.Controllers
{

	[Route("api/[controller]/[action]")]
	[ApiController]
	public class SeedController : ControllerBase
	{

		private readonly AppDbContext _appDbContext;
		private readonly IWebHostEnvironment _env;

		public SeedController(AppDbContext appDbContext, IWebHostEnvironment Env)
		{
			_appDbContext = appDbContext;
			_env = Env;
		}




		[HttpGet]
		public async Task<ActionResult> Import()
		{
			if (!_env.IsDevelopment())
			{
				throw new SecurityException("I'm sorry, I can't let you do that.");
			}

			var path = Path.Combine(_env.ContentRootPath, @"Data\Source\worldcities.xlsx");

			using var stream = System.IO.File.OpenRead(path);
			using var excelPackage = new ExcelPackage(stream);

			var worksheet = excelPackage.Workbook.Worksheets[0];
			var nEndRow = worksheet.Dimension.End.Row;

			var numberOfCountriesAdded = 0;
			var numberOfCitiesAdded = 0;

			var countryByName = _appDbContext.Countries
				.AsNoTracking()
				.AsNoTracking()
				.ToDictionary(c => c.Name, StringComparer.OrdinalIgnoreCase);


			// Countries
			numberOfCountriesAdded = await SeedCountries(
				worksheet, nEndRow, numberOfCountriesAdded, countryByName);


			// Cities 
			numberOfCitiesAdded = await SeedCities(
				worksheet, nEndRow, numberOfCitiesAdded, countryByName);

			return new JsonResult(new
			{
				cities = numberOfCitiesAdded,
				country = numberOfCountriesAdded
			});
		}



		private async Task<int> SeedCountries(ExcelWorksheet worksheet, int nEndRow, int numberOfCountriesAdded, Dictionary<string, Country> countryByName)
		{
			for (int nRow = 2; nRow <= nEndRow; nRow++)
			{
				var row = worksheet.Cells[nRow, 1, nRow, worksheet.Dimension.End.Column];

				var countryName = row[nRow, 5].GetValue<string>();
				var iso2 = row[nRow, 6].GetValue<string>();
				var iso3 = row[nRow, 7].GetValue<string>();

				if (countryByName.ContainsKey(countryName))
				{
					continue;
				}

				var country = new Country
				{
					Name = countryName,
					ISO2 = iso2,
					ISO3 = iso3,
				};


				await _appDbContext.Countries.AddAsync(country);
				countryByName.Add(countryName, country);

				numberOfCountriesAdded++;
			}


			if (numberOfCountriesAdded > 0)
			{
				await _appDbContext.SaveChangesAsync();
			}

			return numberOfCountriesAdded;
		}


		private async Task<int> SeedCities(
			ExcelWorksheet worksheet,
			int nEndRow, int numberOfCitiesAdded,
			Dictionary<string,
			Country> countryByName)
		{

			var cities = _appDbContext.Cities
				.AsNoTracking()
				.ToDictionary(c => (
				Name: c.Name,
				Lat: c.Lat,
				Lon: c.Lon,
				CountryId: c.CountryId
				));


			for (int nRow = 2; nRow <= nEndRow; nRow++)
			{
				var row = worksheet.Cells[nRow, 1, nRow, worksheet.Dimension.End.Column];

				var name = row[nRow, 1].GetValue<string>();
				var lat = row[nRow, 3].GetValue<decimal>();
				var lon = row[nRow, 4].GetValue<decimal>();
				var countryName = row[nRow, 5].GetValue<string>();

				var countryId = countryByName[countryName].Id;

				if (cities.ContainsKey((
					Name: name,
					Lat: lat,
					Lon: lon,
					CountryId: countryId)))
				{
					continue;
				}


				var city = new City
				{
					Name = name,
					Lat = lat,
					Lon = lon,
					CountryId = countryId
				};

				await _appDbContext.Cities.AddAsync(city);

				numberOfCitiesAdded++;
			}

			if (numberOfCitiesAdded > 0)
			{
				await _appDbContext.SaveChangesAsync();
			}

			return numberOfCitiesAdded;
		}
	}
}
