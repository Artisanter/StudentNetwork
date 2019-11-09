using System;
using System.Linq;

namespace StudentNetwork.Models
{
	public static class SampleData
    {
        public static void Initialize(StudentContext context)
        {
            if (context is null || context.Groups.Any())
                return;

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
                    FirstName = "Евгений",
                    LastName = "Чижик",
                    Login = "eugene",
                    Password = "eugene",
                    Group = g1
                },
                new Student()
                {
                    FirstName = "Валентин",
                    LastName = "Дорофеев",
                    Login = "valya",
                    Password = "valya",
                    Group = g1
                },
                new Student()
                {
                    FirstName = "Иван",
                    LastName = "Чибисов",
                    Login = "vanya",
                    Password = "vanya",
                    Group = g2
                },
                new Student()
                {
                    FirstName = "Вадим",
                    LastName = "Олисейчик",
                    Login = "vadim",
                    Password = "vadim",
                    Group = g2
                }
            );
            foreach (var student in context.Students.ToList())
            {
                student.Group.Students.Add(student);
                var message = new Message()
                {
                    Sender = student,
                    DateTime = DateTime.Now,
                    Text = $"Привет, меня зовут {student.Name}"
                };
                context.Messages.Add(message);
                student.Group.Chat.Send(message);
            }

            context.SaveChanges();
        }
    }
}
