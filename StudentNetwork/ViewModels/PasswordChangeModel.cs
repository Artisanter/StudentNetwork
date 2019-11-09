using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StudentNetwork.ViewModels
{
    public class PasswordChangeModel
    {
        [Required(ErrorMessage = "Не указан пароль")]
        [Remote("ConfirmPassword", "Account", ErrorMessage = "Пароль введен неверно")]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Не указан пароль")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Пароль введен неверно")]
        public string ConfirmNewPassword { get; set; }
    }
}
