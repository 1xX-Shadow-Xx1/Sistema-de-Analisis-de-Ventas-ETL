
namespace Sistema_de_Análisis_de_Ventas_ETL.Models.Csv
{
    public class OrderDetailCsvMap
    {
        public int OrderID { get; set; }
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
