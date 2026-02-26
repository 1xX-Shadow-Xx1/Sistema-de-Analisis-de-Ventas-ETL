using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Sistema_de_Análisis_de_Ventas_ETL.Models;
using Sistema_de_Análisis_de_Ventas_ETL.Models.services;

Console.WriteLine("=== Iniciando Sistema de Análisis de Ventas ETL ===");

string carpetaCsv = @"C:\Users\kevin\Downloads\Archivo CSV Análisis de Ventas-20260204";

var optionsBuilder = new DbContextOptionsBuilder<DB_Ventas_ETLContext>();
optionsBuilder.UseSqlServer("Server=(localdb)\\ProjectModels;Database=DB_Ventas_ETL;Trusted_Connection=True;TrustServerCertificate=True;");

try
{
    using (var dbContext = new DB_Ventas_ETLContext(optionsBuilder.Options))
    {
        LoadingDataServices etlService = new LoadingDataServices(carpetaCsv, dbContext);

        etlService.IniciarFlujoETL();
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Ocurrió un error crítico en la ejecución: {ex.Message}");
}

Console.WriteLine("=== Proceso finalizado. Presiona ENTER para salir ===");
Console.ReadLine(); 