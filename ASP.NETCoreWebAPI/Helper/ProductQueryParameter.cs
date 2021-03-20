using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASPNETCoreWebAPI.Helper
{
    /// <summary>
    /// Takes a SKU(int), MinPrice(Decimal), MaxPrice(Decimal) from Query String and Filters the result
    /// based on given Values.
    /// </summary>
    public class ProductQueryParameter : QueryParameter
    {
        public string Sku { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string Name { get; set; }
    }
}
