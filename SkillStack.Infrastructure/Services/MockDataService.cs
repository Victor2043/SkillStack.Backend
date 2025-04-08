// MockDataService.cs
using SkillStack.Domain.Entities;

public class MockDataService
{
    public List<Product> GetProducts()
    {
        return new List<Product>
        {
            new Product {
                Id = 1,
                Name = "iPhone 15 Pro",
                Price = 8999.00m,
                ProductTypeId = 1,
                Type = new ProductType { Id = 1, Name = "Smartphone", Description = "Celulares premium" }
            },
            new Product {
                Id = 2,
                Name = "MacBook Pro M2",
                Price = 14999.00m,
                ProductTypeId = 2,
                Type = new ProductType { Id = 2, Name = "Notebook", Description = "Laptops de alta performance" }
            },
            new Product {
                Id = 3,
                Name = "Fone Sony WH-1000XM5",
                Price = 2299.00m,
                ProductTypeId = 3,
                Type = new ProductType { Id = 3, Name = "Acessório", Description = "Acessórios eletrônicos" }
            },
            new Product {
                Id = 4,
                Name = "Samsung Galaxy S23",
                Price = 4999.00m,
                ProductTypeId = 1,
                Type = new ProductType { Id = 1, Name = "Smartphone", Description = "Celulares premium" }
            },
            new Product {
                Id = 5,
                Name = "Dell XPS 15",
                Price = 9999.00m,
                ProductTypeId = 2,
                Type = new ProductType { Id = 2, Name = "Notebook", Description = "Laptops de alta performance" }
            },
            new Product {
                Id = 6,
                Name = "Xiaomi Redmi Note 12",
                Price = 1499.00m,
                ProductTypeId = 1,
                Type = new ProductType { Id = 1, Name = "Smartphone", Description = "Celulares premium" }
            },
            new Product {
                Id = 7,
                Name = "Cabo USB-C 2m",
                Price = 59.90m,
                ProductTypeId = 3,
                Type = new ProductType { Id = 3, Name = "Acessório", Description = "Acessórios eletrônicos" }
            },           
            new Product {
                Id = 8,
                Name = "Lenovo ThinkPad X1",
                Price = 7999.00m,
                ProductTypeId = 2,
                Type = new ProductType { Id = 2, Name = "Notebook", Description = "Laptops de alta performance" }
            },
            new Product {
                Id = 9,
                Name = "Carregador Wireless",
                Price = 199.00m,
                ProductTypeId = 3,
                Type = new ProductType { Id = 3, Name = "Acessório", Description = "Acessórios eletrônicos" }
            }
        };
    }

    public List<ProductType> GetProductTypes()
    {
        return new List<ProductType>
        {
            new ProductType { Id = 1, Name = "Smartphone", Description = "Celulares premium" },
            new ProductType { Id = 2, Name = "Notebook", Description = "Laptops de alta performance" },
            new ProductType { Id = 3, Name = "Acessório", Description = "Acessórios eletrônicos" }
        };
    }
}