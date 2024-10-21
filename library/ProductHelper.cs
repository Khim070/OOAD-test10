using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MenuLib;

namespace productlib
{
    public static class ProductHelper
    {
        private static ProductService service = new ProductService(
                                                    new ProductRepo(
                                                        new FileContext("products.json")));
        public static MenuBank MenuBank { get; set; } = new MenuBank()
        {
            Title = "Products",
            Menus = new List<Menu>()
            {
                new Menu(){ Text= "Viewing", Action=Viewing},
                new Menu(){ Text= "Creating", Action=Creating},
                new Menu(){ Text= "Updating", Action=Updating},
                new Menu(){ Text= "Deleting", Action=Deleting},
                new Menu(){ Text= "Exiting", Action = Exiting}
            }
        };

        static ProductHelper()
        {
            service.Initialize();
        }

        public static void Exiting()
        {
            Console.WriteLine("\n[Exiting Program]");
            Environment.Exit(0);
        }

        private static void Deleting()
        {
            Console.WriteLine("\n[Deleting]");
            while (true)
            {
                Console.Write("Product Id/Code: ");
                var key = Console.ReadLine() ?? "";
                var result = service.Delete(key);
                if(result == true)
                {
                    Console.WriteLine($"Successfully delete the product with id/code {key}");
                }
                else
                {
                    Console.WriteLine($"Failed to delete the product with id/code {key}");
                }
                Console.WriteLine();
                if (WaitForEscPressed("ESC to stop or any key for more deleting...")) break;
            }

        }

        private static void Updating()
        {
            Console.WriteLine("\n[Updating]");
            while (true)
            {
                Console.WriteLine("Product Id/Code(required): ");
                var key = Console.ReadLine() ?? "";
                if (!service.Exist(key))
                {
                    Console.WriteLine($"no product with id/code, {key}");
                }else
                {
                    Console.Write("New Name (optional): ");
                    var name = Console.ReadLine();
                    Console.WriteLine($"Category Available: {Enum.GetNames<Category>().Aggregate((a, b) => a + "," + b)}");
                    Console.Write("New Category: ");
                    var category = Console.ReadLine();
                    var result = service.Update(new ProductUpdateReq()
                    {
                        Key = key,
                        Name = name,
                        Category = category
                    });
                    if(result == true)
                    {
                        Console.WriteLine($"Successfully update the product with id/code, {key}");
                    }else
                    {
                        Console.WriteLine($"Failed to update the product with id/code, {key}");
                    }
                }
                Console.WriteLine();
                if (WaitForEscPressed("ESC to stop or any key for more updating...")) break;
            }
        }

        private static bool WaitForEscPressed(string text)
        {
            Console.Write(text); ;
            ConsoleKeyInfo keyInfo = Console.ReadKey(true);
            Console.WriteLine();
            return keyInfo.Key == ConsoleKey.Escape;
        }

        private static void Creating()
        {
            Console.WriteLine("\n[Creating]");
            while (true)
            {
                var req = GetCreateProduct();
                if( req != null )
                {
                    var id = service.Create(req);
                    if (!string.IsNullOrEmpty(id)) Console.WriteLine($"Sucessfully created a new product with id, {id}");
                    else Console.WriteLine($"Failed to created a new product code, {req.Code}");
                }
                Console.WriteLine();
                if (WaitForEscPressed("ESC to stop or any key for more creating...")) break;
            }
        }

        private static void Viewing()
        {
            Console.WriteLine("\n[Viewing]");
            var all = service.ReadAll();
            var count = all.Count();
            Console.WriteLine($"Products: {count}");
            if (count == 0) return;
            Console.WriteLine($"{"Id",-36} {"Code",-10} {"Name",-10} {"Category",-20}");
            foreach(var prd in all)
            {
                Console.WriteLine($"{prd.Id,-36} {prd.Code,-10} {prd.Name,-10} {prd.Category,-20}");
            }
            Console.Write("Press any key to continue...");
            Console.ReadKey();
            Console.WriteLine();
        }

        private static ProductCreateReq? GetCreateProduct()
        {
            Console.WriteLine($"Category available: {Enum.GetNames<Category>().Aggregate((a, b) => a + "," + b)}");
            Console.Write("Product(code/name/category): ");
            string data = Console.ReadLine() ?? "";
            var dataParts = data.Split('/');
            if (dataParts.Length < 3) 
            {
                Console.WriteLine("Invalid create product's data");
            }
            var code = dataParts[0].Trim();
            var name = dataParts[1].Trim();
            var categroy = dataParts[2].Trim();

            return new ProductCreateReq()
            {
                Code = code,
                Name = name,
                Category = categroy,
            };
        }
    }
}
