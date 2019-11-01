using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StudentNetwork.ViewModels
{
    public class UserEditModel
    {
        [Required(ErrorMessage = "Не указан логин")]
        [Remote("IsAvaible", "Account", ErrorMessage = "Логин занят")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Не указано имя")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Не указана фамилия")]
        public string LastName { get; set; }
    }
}
