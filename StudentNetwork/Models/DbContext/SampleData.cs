using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace StudentNetwork.Models
{
	public static class SampleData
    {
        public static void Initialize(StudentContext context)
        {
            if (context is null || context.Groups.Any())
                return;
            byte[] data = null;
            FileStream stream = new FileStream(Directory.GetCurrentDirectory() + 
                "\\wwwroot\\images\\default_pic.jpg", FileMode.Open);
            using (var binaryReader = new BinaryReader(stream))
            {
                data = binaryReader.ReadBytes((int)stream.Length);
            }
            Image img = new Image()
            {
                Bytes = data,
                Name = "Default"
            };
            context.Images.Add(img);


            var adminRole = new Role() { Name = "Admin" };
            var userRole = new Role() { Name = "User" };

            context.Roles.AddRange(adminRole, userRole);
            var g1 = new Group()
            {
                Number = 753504,
                Name = "Четвертая группа"
            };
            var g2 = new Group()
            {
                Number = 753505,
                Name = "Пятая группа"
            };
            context.Groups.AddRange(g1, g2);

            context.Students.AddRange(
                new Student()
                {
                    FirstName = "Admin",
                    LastName = "Adminov",
                    Login = "admin",
                    Password = "admin",
                    Role = adminRole
                },
                new Student()
                {
                    FirstName = "Евгений",
                    LastName = "Чижик",
                    Login = "eugene",
                    Password = "eugene",                    
                    Memberships = new HashSet<Membership>()
                    { 
                        new Membership()
                        {
                            Group = g1,
                            Role = adminRole
                        }
                    },
                    Role = userRole
                },
                new Student()
                {
                    FirstName = "Валентин",
                    LastName = "Дорофеев",
                    Login = "valya",
                    Password = "valya",
                    Memberships = new HashSet<Membership>()
                    {
                        new Membership()
                        {
                            Group = g1,
                            Role = userRole
                        }
                    },
                    Role = userRole
                },
                new Student()
                {
                    FirstName = "Иван",
                    LastName = "Чибисов",
                    Login = "vanya",
                    Password = "vanya",
                    Memberships = new HashSet<Membership>()
                    {
                        new Membership()
                        {
                            Group = g2,
                            Role = adminRole
                        }
                    },
                    Role = userRole
                },
                new Student()
                {
                    FirstName = "Вадим",
                    LastName = "Олисейчик",
                    Login = "vadim",
                    Password = "vadim",
                    Memberships = new HashSet<Membership>()
                    {
                        new Membership()
                        {
                            Group = g2,
                            Role = userRole
                        }
                    },
                    Role = userRole
                },
                new Student()
                {
                    FirstName = "Дима",
                    LastName = "Димов",
                    Login = "dima",
                    Password = "dima",
                    Role = userRole
                }
            );
            foreach (var student in context.Students.ToList())
            {
                if (student.Memberships.Count > 0)
                    student.Memberships.First().Student = student;
                student.Image = img;
            }

            context.SaveChanges();
        }
    }
}
