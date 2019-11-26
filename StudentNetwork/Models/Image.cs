using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StudentNetwork.Models
{
    public class Image
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public uint Width { get; set; }
        public uint Height { get; set; }
        public byte[] Bytes { get; set; } 
    }
}
