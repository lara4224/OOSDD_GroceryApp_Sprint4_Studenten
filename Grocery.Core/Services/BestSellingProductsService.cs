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

        public List<BestSellingProducts> GetBestSellingProducts(int topX = 5)
        {
            Dictionary<int, int> productSales = [];
            List<BestSellingProducts> bestSellingProducts = [];

            foreach (GroceryListItem item in _groceryListItemsRepository.GetAll())
            {
                if (item.Amount > 0)
                    productSales[item.ProductId] = productSales.GetValueOrDefault(item.ProductId) + item.Amount;
            }

            var topSoldProducts = productSales
                .OrderByDescending(p => p.Value)
                .Take(topX)
                .ToList();

            int rank = 1;
            foreach (var productSalesKVP in topSoldProducts)
            {
                Product? p = _productRepository.Get(productSalesKVP.Key);
                if (p == null) continue;
                int numberOfSales = productSalesKVP.Value;
                bestSellingProducts.Add(new BestSellingProducts(p.Id, p.Name, p.Stock, numberOfSales, rank++));
            }

            return bestSellingProducts;
        }
    }
}
