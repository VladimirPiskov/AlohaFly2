using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlohaService.ServiceDataContracts;

namespace AlohaFlyAdmin
{
    public static class CTmp
    {
        public static void CreatePUser()
        {
            var U = new UserInfo()
            {
                FullName = "Владимир Писков",
                Password = "123",
                UserName = "v.p",
                UserRole = UserRole.Admin
            };
            AlohaFly.DBProvider.CreateUser(U);
        }

        public static void CreateP2User()
        {
            var U = new UserInfo()
            {
                FullName = "Владимир Писков Дир",
                Email = "v.p@m.ru",
                Password = "123",
                UserName = "v.pp",
                UserRole = UserRole.Director
            };
            AlohaFly.DBProvider.CreateUser(U);
        }

        public static void CreateAlohaSharUser()
        {
            var U = new UserInfo()
            {
                FullName = "Алоха Шереметьево",
                Email = "al.sh@m.ru",
                Password = "123",
                UserName = "a.sh",
                UserRole = UserRole.Other
            };
            AlohaFly.DBProvider.CreateUser(U);

            long userid = AlohaFly.DBProvider.GetClient().CreateUser(U).CreatedObjectId;
            AlohaFly.DBProvider.GetClient().AddUserToGroup(userid, OperGrId);
        }

        public static void CreateTUser()
        {
            var U = new UserInfo()
            {
                FullName = "Татьяна Пролыгина",
                Email = "tanjaxxx77@mail.ru",
                Password = "123",
                UserName = "t.p",
                UserRole = UserRole.Admin
            };
            AlohaFly.DBProvider.CreateUser(U);
        }

        public static void CreateAlohaSharUser2()
        {
            var U = new UserInfo()
            {
                FullName = "Шереметьево пользователь",
                Email = "al.sh2@m.ru",
                Password = "123",
                UserName = "sh.user",
                UserRole = UserRole.Admin
            };
            AlohaFly.DBProvider.CreateUser(U);

            long userid = AlohaFly.DBProvider.GetClient().CreateUser(U).CreatedObjectId;
            AlohaFly.DBProvider.GetClient().AddUserToGroup(userid, OperGrId);
        }

        public static void CreateDDUser()
        {
            var U = new UserInfo()
            {
                FullName = "Денис Денисенко",
                Email = "e.m@bk.ru",
                Password = "123",
                UserName = "d.d",
                UserRole = UserRole.Other
            };
            AlohaFly.DBProvider.CreateUser(U);
        }

        public static void CreateEMUser()
        {
            var U = new UserInfo()
            {
                FullName = "Эвелина",
                Email = "e.m@bk.ru",
                Password = "123",
                UserName = "e.m",
                UserRole = UserRole.Admin
            };
            AlohaFly.DBProvider.CreateUser(U);
        }

        public static void CreateNPUser()
        {
            var U = new UserInfo()
            {
                FullName = "Надежда Парасинина",
                Email = "parasinina@bk.ru",
                Password = "123",
                UserName = "n.p",
                UserRole = UserRole.Admin
            };
            AlohaFly.DBProvider.CreateUser(U);
        }


        public static void CreateDirectorUser()
        {
            var U = new UserInfo()
            {
                FullName = "Андрей Нестеров",
                Email = "nesterov.a@list.ru",
                Password = "Z nen ukfdysq",
                UserName = "a.n",
                UserRole = UserRole.Director
            };
            AlohaFly.DBProvider.CreateUser(U);
        }

        public static void CreateOperatorGroup()
        {
            /*
            UserGroup Ug = new UserGroup()
            {
                Name = "Оператор"
            };
            long Id = AlohaFly.DBProvider.GetClient().CreateUserGroup(Ug).CreatedObjectId;
            */

            var r = AlohaFly.DBProvider.GetClient().GetAllGroupFuncs(OperGrId);
            for (int f = 1; f < 12; f++)
            {
                if (!r.Result.Select(a => a.UserFuncId).Contains(f))
                {
                    AlohaFly.DBProvider.GetClient().AddFuncToGroup(f, OperGrId, FuncAccessType.Edit);
                }
            }
        }

        public static void CreateOperatorUser()
        {
            var U = new UserInfo()
            {
                FullName = "Оператор",
                Email = "operator@bk.ru",
                Password = "123",
                UserName = "oper",
                UserRole = UserRole.Other
            };
            long userid =  AlohaFly.DBProvider.GetClient().CreateUser(U).CreatedObjectId;
            AlohaFly.DBProvider.GetClient().AddUserToGroup(userid, OperGrId);   
        }

        public static void CreateOperatorUser1()
        {
            var U = new UserInfo()
            {
                FullName = "Денис Денисенко",
                Email = "den.denisenko.93@mail.ru",
                Password = "123",
                UserName = "d.d",
                Phone = "89017859615",
                UserRole = UserRole.Other
            };
            long userid = AlohaFly.DBProvider.GetClient().CreateUser(U).CreatedObjectId;
            AlohaFly.DBProvider.GetClient().AddUserToGroup(userid, OperGrId);
        }

        public static void CreateOperatorUser11()
        {
            var U = new UserInfo()
            {
                FullName = "Людмила Тувыкина",
                Email = "l.t@mail.ru",
                Password = "123",
                UserName = "l.t",
                Phone = "89017777777",
                UserRole = UserRole.Other
            };
            long userid = AlohaFly.DBProvider.GetClient().CreateUser(U).CreatedObjectId;
            AlohaFly.DBProvider.GetClient().AddUserToGroup(userid, OperGrId);
        }

        public static void CreateOperatorUser12()
        {
            var U = new UserInfo()
            {
                FullName = "Алиса Крутова",
                Email = "a.k@mail.ru",
                Password = "123",
                UserName = "a.k",
                Phone = "89017777777",
                UserRole = UserRole.Other
            };
            long userid = AlohaFly.DBProvider.GetClient().CreateUser(U).CreatedObjectId;
            AlohaFly.DBProvider.GetClient().AddUserToGroup(userid, OperGrId);
        }


        public static void CreateOperatorUser10()
        {
            var U = new UserInfo()
            {
                FullName = "Дмитрий Раковский",
                Email = "den2.denisenko.93@mail.ru",
                Password = "123",
                UserName = "d.r",
                Phone = "+7 985 209-48-25",
                UserRole = UserRole.Other
            };
            long userid = AlohaFly.DBProvider.GetClient().CreateUser(U).CreatedObjectId;
            AlohaFly.DBProvider.GetClient().AddUserToGroup(userid, OperGrId);
        }
        public static void CreateOperatorUser2()
        {
            var U = new UserInfo()
            {
                FullName = "Александр Авдонин",
                Email = "alexanderavdonin719@gmail.com",
                Password = "123",
                UserName = "a.a",
                Phone = "89169717252",
                UserRole = UserRole.Other
            };
            long userid = AlohaFly.DBProvider.GetClient().CreateUser(U).CreatedObjectId;
            AlohaFly.DBProvider.GetClient().AddUserToGroup(userid, OperGrId);
        }
        public static void CreateOperatorUser3()
        {
            var U = new UserInfo()
            {
                FullName = "Александр Процюк",
                Email = "modern_dancer86@mail.ru",
                Password = "123",
                UserName = "a.p",
                Phone = "89296262162",
                UserRole = UserRole.Other
            };
            long userid = AlohaFly.DBProvider.GetClient().CreateUser(U).CreatedObjectId;
            AlohaFly.DBProvider.GetClient().AddUserToGroup(userid, OperGrId);
        }
        public static void CreateOperatorUser4()
        {
            var U = new UserInfo()
            {
                FullName = "Анна Жгун ",
                Email = "nyrka_666@mail.ru",
                Password = "123",
                UserName = "a.j",
                Phone = "89104948422",
                UserRole = UserRole.Other
            };
            long userid = AlohaFly.DBProvider.GetClient().CreateUser(U).CreatedObjectId;
            AlohaFly.DBProvider.GetClient().AddUserToGroup(userid, OperGrId);
        }

        public static long OperGrId = 1;
        public static void CreateUser2()
        {
            var U = new UserInfo()
            {
                FullName = "Test user",
                Password = "1234",
                UserName = "v.pp",
                UserRole = UserRole.Other
            };
}
    }
}
