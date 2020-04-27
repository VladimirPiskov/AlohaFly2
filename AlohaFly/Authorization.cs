using AlohaService.ServiceDataContracts;
using System.Collections.Generic;

namespace AlohaFly
{
    public static class Authorization
    {
        internal static bool DoAuthorization(string login, string password)
        {
            User user;
            string errorMsg = "";
            bool res = DBProvider.LoginUser(login, password, out user, errorMsg);
            if (res)
            {
                CurentUser = user;
                UserFuncs = DBProvider.GetUserFuncs(user.Id);
            }
            return res;
        }

        public static User CurentUser { set; get; }

        internal static UserRole GetCurentUserRole()
        {
            return CurentUser.UserRole;
        }
        internal static Dictionary<long, FuncAccessType> UserFuncs { set; get; }
        public static Utils.FuncAccessTypeEnum GetAccessType(long funcId)
        {
            if (CurentUser == null) return Utils.FuncAccessTypeEnum.Disable;
            if (CurentUser.UserRole == UserRole.Admin)
            {
                if (Utils.AccessTypeConst.NonAdminFuncs.Contains(funcId)) { return Utils.FuncAccessTypeEnum.Disable; }
                else { return Utils.FuncAccessTypeEnum.Edit; }
            }
            else if (CurentUser.UserRole == UserRole.Director)
            {
                if (Utils.AccessTypeConst.NonDirectorFuncs.Contains(funcId)) { return Utils.FuncAccessTypeEnum.Disable; }
                else { return Utils.FuncAccessTypeEnum.Edit; }
            }
            else
            {
                if (UserFuncs == null) { return Utils.FuncAccessTypeEnum.Disable; }
                FuncAccessType at;
                if (UserFuncs.TryGetValue(funcId, out at))
                {
                    switch (at)
                    {
                        case FuncAccessType.View:
                            return Utils.FuncAccessTypeEnum.View;
                        case FuncAccessType.Edit:
                            return Utils.FuncAccessTypeEnum.Edit;
                        default:
                            return Utils.FuncAccessTypeEnum.Disable;

                    }
                }
                else
                {
                    return Utils.FuncAccessTypeEnum.Disable;
                }
            }
        }

        internal static bool ChangePassCurentUsr(string login, string password)
        {
            return DBProvider.ChangePass(CurentUser, password);
        }


        public static bool IsDirector
        {
            get
            {
                if (CurentUser == null) return false;
                return (CurentUser.UserRole == UserRole.Director);
            }
        }


        public static bool IsAdmin
        {
            get
            {
                if (CurentUser == null) return false;
                return (CurentUser.UserRole == UserRole.Admin);
            }
        }

    }
    internal class AuthorizationContext
    {

    }
}
