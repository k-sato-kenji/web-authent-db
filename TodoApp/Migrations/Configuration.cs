namespace TodoApp.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using TodoApp.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<TodoApp.Models.TodoesContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;�@// �񂪍폜����Ă��}�C�O���[�V�����ɔ��f����Ă��ǂ����̃I�v�V����

            ContextKey = "TodoApp.Models.TodoesContext";
        }

        protected override void Seed(TodoApp.Models.TodoesContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.

            User admin = new User()
            {
                Id = 1,
                UserName = "admin",
                Password = "password",
                // ��̃��[����o�^
                Roles = new List<Role>()
            };

            // ��ʃ��[�U�[							
            User sato = new User()
            {
                Id = 2,
                UserName = "sato",
                Password = "password",
                Roles = new List<Role>()
            };

            Role administrators = new Role()
            {
                Id = 1,
                RoleName = "Administrators",
                Users = new List<User>()
            };

            Role users = new Role()
            {
                Id = 2,
                RoleName = "Users",
                Users = new List<User>()
            };

            admin.Roles.Add(administrators);
            administrators.Users.Add(admin);
            sato.Roles.Add(users);
            users.Users.Add(sato);

            // �c�a�֓o�^�i�L��΍X�V�A������Βǉ��j							
            context.Users.AddOrUpdate(user => user.Id, new User[] { admin, sato });
            context.Roles.AddOrUpdate(role => role.Id, new Role[] { administrators, users });

            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
        }
    }
}
