using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Entities
{
    public class Settings
    {
        [Key]
        public int Id { get; set; }
        public int ENFORCED_NUMBER_OF_ATTEMPTS { get; set; }
        public TimeSpan ENFORCED_DELAY_BETWEEN_ATTEMPTS { get; set; }
    }
}
