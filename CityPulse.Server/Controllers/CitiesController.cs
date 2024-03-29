﻿using CityPulse.Server.Data;
using CityPulse.Server.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CityPulse.Server.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CitiesController : ControllerBase
	{
		private readonly AppDbContext _context;


		public CitiesController(AppDbContext context)
		{
			_context = context;
		}


		[HttpGet]
		public async Task<ActionResult<ApiResult<City>>> GetCities(
		int pageIndex = 0,
		int pageSize = 10,
		string? sortColumn = null,
		string? sortOrder = null)
		{

			return await ApiResult<City>.CreateAsync(
				_context.Cities.AsNoTracking(),
				pageIndex,
				pageSize,
				sortColumn,
				sortOrder);
		}


		[HttpGet("{id}")]
		public async Task<ActionResult<City>> GetCity(int id)
		{
			var city = await _context.Cities.FindAsync(id);

			if (city == null)
			{
				return NotFound();
			}

			return city;
		}


		[HttpPut("{id}")]
		public async Task<IActionResult> PutCity(int id, City city)
		{
			if (id != city.Id)
			{
				return BadRequest();
			}

			_context.Entry(city).State = EntityState.Modified;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!CityExists(id))
				{
					return NotFound();
				}
				else
				{
					throw;
				}
			}

			return NoContent();
		}


		[HttpPost]
		public async Task<ActionResult<City>> PostCity(City city)
		{
			_context.Cities.Add(city);
			await _context.SaveChangesAsync();

			return CreatedAtAction("GetCity", new { id = city.Id }, city);
		}


		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteCity(int id)
		{
			var city = await _context.Cities.FindAsync(id);
			if (city == null)
			{
				return NotFound();
			}

			_context.Cities.Remove(city);
			await _context.SaveChangesAsync();

			return NoContent();
		}

		private bool CityExists(int id)
		{
			return _context.Cities.Any(e => e.Id == id);
		}
	}
}
