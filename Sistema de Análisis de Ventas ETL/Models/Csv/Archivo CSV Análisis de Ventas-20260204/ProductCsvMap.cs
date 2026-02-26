namespace Sistema_de_Análisis_de_Ventas_ETL.Models.Csv
{
    public class ProductCsvMap
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
    }
}
