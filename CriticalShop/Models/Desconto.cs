using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CriticalShop.Models
{
    public class Desconto
    {
        public int Id { get; set; }
        public int Valor { get; set; } // percentual de desconto
    }
}
