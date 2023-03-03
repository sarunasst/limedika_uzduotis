using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

namespace ClientImport.Models
{
    [Index("Name", Name = "IX_ClientName",  IsUnique = true)]
    public class Client
    {
        [Key]
        public virtual int Id { get; set; }

        [Required]
        [StringLength(255)]
        public virtual string Name { get; set; }

        [Required]
        [StringLength(255)]
        public virtual string Address { get; set; }

        [StringLength(31)]
        public virtual string PostCode { get; set; }
    }
}
