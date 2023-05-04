using System;
using System.Collections.Generic;

namespace WorkerService1.Models
{
    public partial class Skupackage
    {
        public int Uniqueid { get; set; }
        public string Packagetype { get; set; } = null!;
        public int Quantity { get; set; }
        public int Weight { get; set; }
        public int Length { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public bool Isprimaryuom { get; set; }
        public bool Isstatus { get; set; }
        public bool Isitemwithbox { get; set; }
        public int Skuid { get; set; }
        public DateTime Entrydate { get; set; }
        public int Entryuserid { get; set; }
        public DateTime Modifieddate { get; set; }
        public int Modifieduserid { get; set; }
        public int Shippingboxid { get; set; }

        public virtual Whuser Entryuser { get; set; } = null!;
        public virtual Whuser Modifieduser { get; set; } = null!;
        public virtual Skushippingbox Shippingbox { get; set; } = null!;
        public virtual Sku Sku { get; set; } = null!;
    }
}
