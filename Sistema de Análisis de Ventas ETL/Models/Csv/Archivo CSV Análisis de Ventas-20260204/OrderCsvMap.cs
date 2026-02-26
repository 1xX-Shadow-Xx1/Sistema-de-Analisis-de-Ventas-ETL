namespace Sistema_de_Análisis_de_Ventas_ETL.Models.Csv
{
    public class OrderCsvMap
    {
        public int OrderID { get; set; }
        public int CustomerID { get; set; }
        public string OrderDate { get; set; }
        public string Status { get; set; }
    }
}
