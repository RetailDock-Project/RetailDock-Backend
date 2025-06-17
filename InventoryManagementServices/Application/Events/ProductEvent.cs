using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Events
{
    public abstract class ProductEvent
    {
        public Guid ProductId { get; set; }
        //public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    public class ProductCreatedEvent : ProductEvent
    {
        public string ProductName { get; set; }
        public Guid OrgnaisationId { get; set; }

        public decimal Gst { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal MRP { get; set; }
        public string UnitOfMeasure { get; set; }
        public string Category { get; set; }
        public string ProductCode { get; set; }

        public decimal CostPrice { get; set; }

    }


    //public class ProductUpdateEvent : ProductEvent
    //{
    //    public string ProductName { get; set; }
    //    public Guid OrgnaisationId { get; set; }

    //    public decimal Gst { get; set; }
    //    public decimal SellingPrice { get; set; }
    //    public decimal MRP { get; set; }
    //    public string UnitOfMeasure { get; set; }
    //    public string Category { get; set; }
    //    public string ProductCode { get; set; }

    //    public decimal CostPrice { get; set; }

    //}

    //public class ProductUpdatedEvent : ProductEvent
    //{
    //    public string Name { get; set; }
    //    public string Description { get; set; }
    //    public decimal Price { get; set; }
    //    public int StockQuantity { get; set; }
    //}

    //public class ProductDeletedEvent : ProductEvent
    //{
    //}
}
