using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StudentNetwork.ViewModels
{
    public class GroupModel
    {
        [Required(ErrorMessage = "Не указан номер группы")]
        public uint Number { get; set; }
        public string Name { get; set; }
        [Key]
        public int Id { get; set; }
    }
}
