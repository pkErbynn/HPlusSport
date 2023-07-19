using System;
namespace HPlusSport.API.Models
{
	public class ProductFilterQueryParams : QueryParams
	{
		// for filter
		public decimal? MinPrice { get; set; }	// nullable
		public decimal? MaxPrice { get; set; }

		// for search
		public string Name { get; set; } = String.Empty;
		public string Sku { get; set; } = String.Empty;
        public string SearchTerm { get; set; } = String.Empty;	// generic search term

    }
}

