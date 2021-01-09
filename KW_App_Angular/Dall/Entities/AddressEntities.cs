using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KW_App_Angular.Dall.Entities
{
    public class AddressEntities
    {
        [Key]
        public int AddressId { get; set; }

        [Required]
        public string City { get; set; }
        public string NPCode { get; set; }

        [ForeignKey("UserId")]
        public string UserId { get; set; }
        public virtual ApplicationUserEntities User { get; set; }
    }
}
