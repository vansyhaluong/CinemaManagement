using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema
{
	public class InventoryItem
	{
		public int Id { get; set; }
		public string Name { get; set; } = "";
		public string Category { get; set; } = "";
		public int Qty { get; set; }
		public decimal Price { get; set; }
		public string Supplier { get; set; } = "";
		public DateTime LastUpdated { get; set; }
		public string Status { get; set; } = "";

		public InventoryItem(int id, string name, string category, int qty,
			decimal price, string supplier, DateTime lastUpdated, string status)
		{
			Id = id; Name = name; Category = category; Qty = qty;
			Price = price; Supplier = supplier; LastUpdated = lastUpdated;
			Status = status;
		}

		// Formatted display properties for WPF binding
		public string PriceDisplay => Price.ToString("C");
		public string LastUpdatedDisplay => LastUpdated.ToString("d MMM yyyy");
	}
}
