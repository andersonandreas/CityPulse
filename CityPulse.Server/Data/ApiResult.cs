using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using System.Reflection;

namespace CityPulse.Server.Data
{
	public class ApiResult<T>
	{

		public List<T> Data { get; private set; }
		public int PageIndex { get; private set; }
		public int PageSize { get; private set; }
		public int TotalCount { get; private set; }
		public int TotalPages { get; private set; }
		public string? SortColumn { get; set; }
		public string? SortOrder { get; set; }
		public bool HasPreviousPage => PageIndex > 0;
		public bool HasNextPage => (PageIndex + 1) < TotalPages;




		private ApiResult(
			List<T> data,
			int count,
			int pageIndex,
			int pageSize,
			string? sortColumn = null,
			string? sortOrder = null)
		{
			Data = data;
			PageIndex = pageIndex;
			PageSize = pageSize;
			TotalCount = count;
			TotalPages = (int)Math.Ceiling(count / (double)pageSize);
			SortColumn = sortColumn;
			SortOrder = sortOrder;
		}


		public static async Task<ApiResult<T>> CreateAsync(
			IQueryable<T> source,
			int pageIndex,
			int pageSize,
			string? sortColumn,
			string? sortOrder)
		{

			var count = await source.CountAsync();

			if (!string.IsNullOrEmpty(sortColumn) && IsValidProperty(sortColumn))
			{
				sortOrder = !string.IsNullOrEmpty(sortOrder)
					&& sortOrder.ToUpper() == "ASC" ? "ASC" : "DESC";
				// from the dynamic linq core library not the original OrderBy
				source = source.OrderBy(string.Format("{0} {1}", sortColumn, sortOrder));
			}



			source = source
				.Skip(pageIndex * pageSize)
				.Take(pageSize);

			var data = await source.ToListAsync();

			return new ApiResult<T>(
				data,
				count,
				pageIndex,
				pageSize,
				sortColumn,
				sortOrder
				);

		}

		public static bool IsValidProperty(string propertyName, bool ThrowExeptionIfNotFound = true)
		{

			var prop = typeof(T).GetProperty(
				propertyName,
				BindingFlags.IgnoreCase |
				BindingFlags.Public |
				BindingFlags.Instance);

			if (prop == null)
			{
				throw new NotSupportedException(
					string.Format($"Error: Can't find the property '{propertyName}'."));
			}

			return prop != null;
		}



	}
}
