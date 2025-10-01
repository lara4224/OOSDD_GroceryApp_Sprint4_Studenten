using Grocery.Core.Interfaces.Repositories;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grocery.Core.Services
{
    public class BestSellingProductsService : IBestSellingProductsService
    {
        private readonly IGroceryListItemsRepository _groceryListItemsRepository;
        private readonly IProductRepository _productRepository;

        public BestSellingProductsService(IGroceryListItemsRepository groceryListItemsRepository, IProductRepository productRepository)
        {
            _groceryListItemsRepository = groceryListItemsRepository;
            _productRepository = productRepository;
        }

        public List<BestSellingProducts> GetBestSellingProducts(uint topX = 5)
        {
            Dictionary<int, int> productSales = [];
            List<BestSellingProducts> bestSellingProducts = [];

            // Get amount of sales for each product and add them to productSales dictionary
            foreach (GroceryListItem item in _groceryListItemsRepository.GetAll())
            {
                // Only add items with Amount greater than 0 to avoid getting products without sales on returned list 
                if (item.Amount > 0)
                    productSales[item.ProductId] = productSales.GetValueOrDefault(item.ProductId) + item.Amount;
            }

            // Create list of products ordered by sales, and limit the list size to topX
            var topSoldProducts = productSales
                .OrderByDescending(p => p.Value)
                .Take((int)topX)
                .ToList();

            // Fill list with best selling products, using productSales key value pairs
            int rank = 1;
            foreach (var productSalesKVP in topSoldProducts)
            {
                Product? p = _productRepository.Get(productSalesKVP.Key);
                if (p == null) continue; // Avoid edge case where product doesn't exist in repository but still exists in groceryListItems
                int numberOfSales = productSalesKVP.Value;
                bestSellingProducts.Add(new BestSellingProducts(p.Id, p.Name, p.Stock, numberOfSales, rank++));
            }

            return bestSellingProducts;
        }
    }
}
