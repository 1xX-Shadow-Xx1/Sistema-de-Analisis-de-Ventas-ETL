using CsvHelper;
using Sistema_de_Análisis_de_Ventas_ETL.Models.Csv;
using System.Globalization;

namespace Sistema_de_Análisis_de_Ventas_ETL.Models.services
{
    public class LoadingDataServices
    {
        private readonly string pathFile;
        private readonly DB_Ventas_ETLContext context;

        public LoadingDataServices(string pathFile, DB_Ventas_ETLContext context)
        {
            this.pathFile = pathFile;
            this.context = context;
        }

        public void IniciarFlujoETL()
        {
            Console.WriteLine("Iniciando proceso ETL...");

            // ARCHIVO CUSTOMERS 
            string rutaCustomers = Path.Combine(pathFile, "customers.csv");

            // 1. ¿Existe el archivo CSV?
            if (File.Exists(rutaCustomers) == false)
            {
                // Mostrar error y terminar
                Console.WriteLine("Error crítico: El archivo de clientes no existe.");
            }
            else
            {
                // Leer archivo con CsvHelper
                using (var reader = new StreamReader(rutaCustomers))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    var customersData = csv.GetRecords<CustomerCsvMap>().ToList();

                    // ¿Hay Registros?
                    if (customersData.Count == 0)
                    {
                        // Fin - Archivo vacio
                        Console.WriteLine("Fin - Archivo de clientes vacio.");
                    }
                    else
                    {
                        // Extraer el CsvUnicos
                        var clientesUnicos = customersData.DistinctBy(c => c.Email).ToList();

                        // Insertar los Csv nuevos
                        foreach (var customer in clientesUnicos)
                        {
                            var existeCliente = context.Customers.FirstOrDefault(c => c.Email == customer.Email);

                            if (existeCliente == null)
                            {
                                var existeCountry = context.Countries.FirstOrDefault(c => c.CountryName == customer.Country);
                                if (existeCountry == null)
                                {
                                    existeCountry = new Country { CountryName = customer.Country };
                                    context.Countries.Add(existeCountry);
                                    context.SaveChanges();
                                }

                                var existeCity = context.Cities.FirstOrDefault(c => c.CityName == customer.City);
                                if (existeCity == null)
                                {
                                    existeCity = new City { CityName = customer.City, CountryId = existeCountry.CountryId };
                                    context.Cities.Add(existeCity);
                                    context.SaveChanges();
                                }

                                var nuevoCliente = new Customer
                                {
                                    CustomerId = customer.CustomerID,
                                    FirstName = customer.FirstName,
                                    LastName = customer.LastName,
                                    Email = customer.Email,
                                    Phone = customer.Phone,
                                    CityId = existeCity.CityId
                                };

                                context.Customers.Add(nuevoCliente);
                                context.SaveChanges();
                            }
                        }
                        Console.WriteLine("Carga de clientes terminada exitosamente.");
                    }
                }
            }

            // ARCHIVO PRODUCTS
            string rutaProducts = Path.Combine(pathFile, "products.csv");

            if (File.Exists(rutaProducts) == false)
            {
                Console.WriteLine("Error crítico: El archivo de productos no existe.");
            }
            else
            {
                using (var reader = new StreamReader(rutaProducts))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    var productsData = csv.GetRecords<ProductCsvMap>().ToList();

                    if (productsData.Count == 0)
                    {
                        Console.WriteLine("Fin - Archivo de productos vacio.");
                    }
                    else
                    {
                        var productosUnicos = productsData.DistinctBy(p => p.ProductName).ToList();

                        foreach (var prod in productosUnicos)
                        {
                            var existeProd = context.Products.FirstOrDefault(p => p.ProductName == prod.ProductName);
                            if (existeProd == null)
                            {
                                var existeCat = context.Categories.FirstOrDefault(c => c.CategoryName == prod.Category);
                                if (existeCat == null)
                                {
                                    existeCat = new Category { CategoryName = prod.Category };
                                    context.Categories.Add(existeCat);
                                    context.SaveChanges();
                                }

                                var nuevoProducto = new Product
                                {
                                    ProductId = prod.ProductID,
                                    ProductName = prod.ProductName,
                                    CategoryId = existeCat.CategoryId
                                };

                                context.Products.Add(nuevoProducto);
                                context.SaveChanges();
                            }
                        }
                        Console.WriteLine("Carga de productos terminada exitosamente.");
                    }
                }
            }


            // FLUJO 3: ARCHIVO ORDERS 
            string rutaOrders = Path.Combine(pathFile, "orders.csv");

            if (File.Exists(rutaOrders) == false)
            {
                Console.WriteLine("Error crítico: El archivo de órdenes no existe.");
            }
            else
            {
                using (var reader = new StreamReader(rutaOrders))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    var ordersData = csv.GetRecords<OrderCsvMap>().ToList();

                    if (ordersData.Count == 0)
                    {
                        Console.WriteLine("Fin - Archivo de órdenes vacio.");
                    }
                    else
                    {
                        var ordenesUnicas = ordersData.DistinctBy(o => o.OrderID).ToList();

                        foreach (var order in ordenesUnicas)
                        {
                            var existeOrden = context.Orders.FirstOrDefault(o => o.OrderId == order.OrderID);

                            if (existeOrden == null)
                            {
                                // Estado del pedido
                                var existeStatus = context.Statuses.FirstOrDefault(s => s.StatusName == order.Status);
                                if (existeStatus == null)
                                {
                                    existeStatus = new Status { StatusName = order.Status };
                                    context.Statuses.Add(existeStatus);
                                    context.SaveChanges(); 
                                }

                                DateTime.TryParse(order.OrderDate, out DateTime dateConvert);

                                var nuevaOrden = new Order
                                {
                                    OrderId = order.OrderID,
                                    CustomerId = order.CustomerID, 
                                    StatusId = existeStatus.StatusId,
                                    OrderDate = dateConvert
                                };

                                context.Orders.Add(nuevaOrden);
                                context.SaveChanges(); 
                            }
                        }
                        Console.WriteLine("Carga de órdenes terminada exitosamente.");
                    }
                }
            }

            // ARCHIVO ORDER DETAILS
            string rutaOrderDetails = Path.Combine(pathFile, "orderdetails.csv");

            if (File.Exists(rutaOrderDetails) == false)
            {
                Console.WriteLine("Error crítico: El archivo de detalles de órdenes no existe.");
            }
            else
            {
                using (var reader = new StreamReader(rutaOrderDetails))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    var detailsData = csv.GetRecords<OrderDetailCsvMap>().ToList();

                    if (detailsData.Count == 0)
                    {
                        Console.WriteLine("Fin - Archivo de detalles de órdenes vacio.");
                    }
                    else
                    {

                        foreach (var detail in detailsData)
                        {
                            var existeDetalle = context.OrderDetails.FirstOrDefault(od => od.OrderId == detail.OrderID && od.ProductId == detail.ProductID);

                            if (existeDetalle == null)
                            {
                                var nuevoDetalle = new OrderDetail
                                {
                                    OrderId = detail.OrderID,
                                    ProductId = detail.ProductID,
                                    Quantity = detail.Quantity,
                                    UnitPrice = detail.TotalPrice
                                };

                                context.OrderDetails.Add(nuevoDetalle);
                                context.SaveChanges(); 
                            }
                        }
                        Console.WriteLine("Carga de detalles de órdenes terminada exitosamente.");
                    }
                }
            }

            Console.WriteLine("Proceso de carga de datos finalizado por completo.");
        }

    }
}
