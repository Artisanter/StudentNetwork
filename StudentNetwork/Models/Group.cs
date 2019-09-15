using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StudentNetwork.Models
{
    public class Group
    {
        [Key]
        public int Id { get; set; }
        public uint Number { get; set; }
    }
}
