using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoeStore.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Article { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }
        public decimal Price { get; set; }
        public string Supplier { get; set; }
        public string Manufacturer { get; set; }
        public string Category { get; set; }
        public int Discount { get; set; }
        public int Quantity { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; } 

        
        public string ImagePath
        {
            get
            {
                
                switch (Article)
                {
                    case "А112Т4": return "D:\\practika\\BD\\Practika\\ShoreStore\\ShoreStore\\Image\\10.jpg";  
                    case "F635R4": return "D:\\practika\\BD\\Practika\\ShoreStore\\ShoreStore\\Image\\5.jpg";
                    case "H782T5": return "D:\\practika\\BD\\Practika\\ShoreStore\\ShoreStore\\Image\\1.jpg";
                    case "G783F5": return "D:\\practika\\BD\\Practika\\ShoreStore\\ShoreStore\\Image\\3.jpg";
                    case "J384T6": return "D:\\practika\\BD\\Practika\\ShoreStore\\ShoreStore\\Image\\7.jpg";
                    case "D572U8": return "D:\\practika\\BD\\Practika\\ShoreStore\\ShoreStore\\Image\\9.jpg";
                    case "F572H7": return "D:\\practika\\BD\\Practika\\ShoreStore\\ShoreStore\\Image\\6.jpg";
                    case "D329H3": return "D:\\practika\\BD\\Practika\\ShoreStore\\ShoreStore\\Image\\2.jpg";
                    case "B320R5": return "D:\\practika\\BD\\Practika\\ShoreStore\\ShoreStore\\Image\\8.jpg";
                    case "G432E4": return "D:\\practika\\BD\\Practika\\ShoreStore\\ShoreStore\\Image\\4.jpg";
                    default: return "D:\\practika\\BD\\Practika\\ShoreStore\\ShoreStore\\Image\\picture.png";
                }
            }
        }

        
        public string DiscountBackground => Discount > 15 ? "#2E8B57" : "White";

        
        public decimal PriceWithDiscount => Price * (1 - Discount / 100m);
    }
}