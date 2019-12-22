using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AlohaService.ServiceDataContracts;
using AlohaService.Entities;
using log4net;
using AlohaService.Helpers;

namespace AlohaService.BusinessServices
{
    public class UserGroupService
    {
        private AlohaDb db;
        protected ILog log;

        public UserGroupService(AlohaDb databaseContext)
        {
            db = databaseContext;
            LogHelper.Configure();
            log = LogHelper.GetLogger();
        }

        public OperationResult CreateUserGroup(ServiceDataContracts.UserGroup userGroup)
        {
            try
            {
                var dbContext = new AlohaDb();

                var group = new Entities.UserGroup();
                group.Name = userGroup.Name;

                dbContext.UserGroups.Add(group);
                dbContext.SaveChanges();

                return new OperationResult
                {
                    Success = true,
                    CreatedObjectId = group.Id
                };
            }
            catch (Exception e)
            {
                log.Error("Error", e);
                return new OperationResult
                {
                    Success = false,
                    ErrorMessage = e.Message
                };
            }
        }

        public OperationResult DeleteUserGroup(long userGroupId)
        {
            try
            {
                var dbContext = new AlohaDb();

                var group = dbContext.UserGroups.First(g => g.Id == userGroupId);

                dbContext.UserGroups.Remove(group);
                dbContext.SaveChanges();

                return new OperationResult
                {
                    Success = true,
                    CreatedObjectId = group.Id
                };
            }
            catch (Exception e)
            {
                log.Error("Error", e);
                return new OperationResult
                {
                    Success = false,
                    ErrorMessage = e.Message
                };
            }
        }

        public OperationResultValue<List<ServiceDataContracts.UserGroup>> GetUserGroupList()
        {
            try
            {
                var dbContext = new AlohaDb();
                var result = dbContext.UserGroups.ToList().Select(ug => new ServiceDataContracts.UserGroup { Id = ug.Id, Name = ug.Name }).ToList();

                return new OperationResultValue<List<ServiceDataContracts.UserGroup>>
                {
                    Success = true,
                    Result = result
                };
            }
            catch (Exception e)
            {
                log.Error("Error", e);
                return new OperationResultValue<List<ServiceDataContracts.UserGroup>>
                {
                    Success = false,
                    ErrorMessage = e.Message
                };
            }
        }

        public OperationResult CreateUserFunc(ServiceDataContracts.UserFunc userFunc)
        {
            try
            {
                var dbContext = new AlohaDb();

                var func = new Entities.UserFunc();
                func.Name = userFunc.Name;
                dbContext.UserFuncs.Add(func);
                dbContext.SaveChanges();

                return new OperationResult
                {
                    Success = true,
                    CreatedObjectId = func.Id
                };
            }
            catch (Exception e)
            {
                log.Error("Error", e);
                return new OperationResult
                {
                    Success = false,
                    ErrorMessage = e.Message
                };
            }
        }

        public OperationResultValue<ServiceDataContracts.UserFunc> GetUserFunc(long userFuncId)
        {
            try
            {
                var dbContext = new AlohaDb();

                var result = dbContext.UserFuncs.First(uf => uf.Id == userFuncId);

                var userFunc = new ServiceDataContracts.UserFunc()
                {
                    Id = result.Id,
                    Name = result.Name
                };

                return new OperationResultValue<ServiceDataContracts.UserFunc>
                {
                    Success = true,
                    Result = userFunc
                };
            }
            catch (Exception e)
            {
                log.Error("Error", e);
                return new OperationResultValue<ServiceDataContracts.UserFunc>
                {
                    Success = false,
                    ErrorMessage = e.Message
                };
            }
        }

        public OperationResult UpdateUserFunc(ServiceDataContracts.UserFunc userFunc)
        {
            try
            {
                var dbContext = new AlohaDb();

                var func = dbContext.UserFuncs.First(uf => uf.Id == userFunc.Id);

                func.Name = userFunc.Name;
                dbContext.SaveChanges();

                return new OperationResult
                {
                    Success = true,
                    CreatedObjectId = func.Id
                };
            }
            catch (Exception e)
            {
                log.Error("Error", e);
                return new OperationResult
                {
                    Success = false,
                    ErrorMessage = e.Message
                };
            }
        }

        public OperationResult DeleteUserFunc(long userFuncId)
        {
            try
            {
                var dbContext = new AlohaDb();

                var func = dbContext.UserFuncs.First(f => f.Id == userFuncId);

                dbContext.UserFuncs.Remove(func);
                dbContext.SaveChanges();

                return new OperationResult
                {
                    Success = true,
                    CreatedObjectId = func.Id
                };
            }
            catch (Exception e)
            {
                log.Error("Error", e);
                return new OperationResult
                {
                    Success = false,
                    ErrorMessage = e.Message
                };
            }
        }

        public OperationResultValue<List<ServiceDataContracts.UserFunc>> GetUserFuncList()
        {
            try
            {
                var dbContext = new AlohaDb();

                var result = dbContext.UserFuncs.Select(f => new ServiceDataContracts.UserFunc
                {
                    Id = f.Id,
                    Name = f.Name
                }).ToList();

                return new OperationResultValue<List<ServiceDataContracts.UserFunc>>
                {
                    Success = true,
                    Result = result
                };
            }
            catch (Exception e)
            {
                log.Error("Error", e);
                return new OperationResultValue<List<ServiceDataContracts.UserFunc>>
                {
                    Success = false,
                    ErrorMessage = e.Message
                };
            }
        }

        public OperationResult AddUserToGroup(long userId, long usrGroupId)
        {
            try
            {
                var dbContext = new AlohaDb();

                var existedLink = dbContext.UserUserGroups.FirstOrDefault(uug => uug.GroupId == usrGroupId && uug.UserId == userId);

                if (existedLink != null)
                {
                    return new OperationResult
                    {
                        Success = false,
                        ErrorMessage = "User already included in this group."
                    };
                }

                var newLink = new Entities.UserUserGroup();
                newLink.GroupId = usrGroupId;
                newLink.UserId = userId;

                dbContext.UserUserGroups.Add(newLink);
                dbContext.SaveChanges();

                return new OperationResult
                {
                    Success = true,
                    CreatedObjectId = newLink.Id
                };
            }
            catch (Exception e)
            {
                log.Error("Error", e);
                return new OperationResult
                {
                    Success = false,
                    ErrorMessage = e.Message
                };
            }
        }

        public OperationResult RemoveUserFromGroup(long userId, long usrGroupId)
        {
            try
            {
                var dbContext = new AlohaDb();

                var existedLink = dbContext.UserUserGroups.FirstOrDefault(uug => uug.GroupId == usrGroupId && uug.UserId == userId);

                if (existedLink == null)
                {
                    return new OperationResult
                    {
                        Success = false,
                        ErrorMessage = "User is not presented in this group"
                    };
                }

                dbContext.UserUserGroups.Remove(existedLink);
                dbContext.SaveChanges();

                return new OperationResult
                {
                    Success = true,
                };
            }
            catch (Exception e)
            {
                log.Error("Error", e);
                return new OperationResult
                {
                    Success = false,
                    ErrorMessage = e.Message
                };
            }
        }

        public OperationResultValue<List<ServiceDataContracts.UserGroup>> GetAllUserGroups(long userId)
        {
            try
            {
                var dbContext = new AlohaDb();

                var result = dbContext.UserUserGroups.Where(g => g.UserId == userId).Select(ug => new ServiceDataContracts.UserGroup
                {
                    Id = ug.GroupId,
                    Name = ug.UserGroup.Name
                }).ToList();

                return new OperationResultValue<List<ServiceDataContracts.UserGroup>>
                {
                    Success = true,
                    Result = result
                };
            }
            catch (Exception e)
            {
                log.Error("Error", e);
                return new OperationResultValue<List<ServiceDataContracts.UserGroup>>
                {
                    Success = false,
                    ErrorMessage = e.Message
                };
            }
        }

        public OperationResultValue<List<ServiceDataContracts.UserGroupAccess>> GetAllGroupFuncs(long groupId)
        {
            try
            {
                var dbContext = new AlohaDb();

                var result = dbContext.UserGroupAccesses.Where(uga => uga.UserGroupId == groupId).Select(ug => new ServiceDataContracts.UserGroupAccess
                {
                    Id = ug.Id,
                    UserFuncId = ug.UserFuncId,
                    UserGroupId = ug.UserGroupId,
                    UserFunc = new ServiceDataContracts.UserFunc
                    {
                        Id = ug.UserFunc.Id,
                        Name = ug.UserFunc.Name
                    },
                    FuncAccessType = (FuncAccessType)ug.AccessId
                }).ToList();

                return new OperationResultValue<List<ServiceDataContracts.UserGroupAccess>>
                {
                    Success = true,
                    Result = result
                };
            }
            catch (Exception e)
            {
                log.Error("Error", e);
                return new OperationResultValue<List<ServiceDataContracts.UserGroupAccess>>
                {
                    Success = false,
                    ErrorMessage = e.Message
                };
            }
        }

        public OperationResult AddFuncToGroup(long funcId, long usrGroupId, FuncAccessType accessType)
        {
            try
            {
                var dbContext = new AlohaDb();

                var result = dbContext.UserGroupAccesses.FirstOrDefault(uga => uga.UserFuncId == funcId && uga.UserGroupId == usrGroupId);

                if (result != null)
                {
                    return new OperationResult
                    {
                        Success = false,
                        ErrorMessage = "Func already presemted in this group"
                    };
                }

                var newLink =
                dbContext.UserGroupAccesses.Add(
                    new Entities.UserGroupAccess
                    {
                        AccessId = (int)accessType,
                        UserFuncId = funcId,
                        UserGroupId = usrGroupId
                    });

                dbContext.SaveChanges();

                return new OperationResult
                {
                    Success = true,
                    CreatedObjectId = newLink.Id
                };

            }
            catch (Exception e)
            {
                log.Error("Error", e);
                return new OperationResult
                {
                    Success = false,
                    ErrorMessage = e.Message
                };
            }
        }

        public OperationResult RemoveFuncFromGroup(long funcId, long usrGroupId)
        {
            try
            {
                var dbContext = new AlohaDb();

                var result = dbContext.UserGroupAccesses.FirstOrDefault(uga => uga.UserFuncId == funcId && uga.UserGroupId == usrGroupId);

                if (result == null)
                {
                    return new OperationResult
                    {
                        Success = false,
                        ErrorMessage = "Func not presemted in this group."
                    };
                }

                dbContext.UserGroupAccesses.Remove(result);

                dbContext.SaveChanges();

                return new OperationResult
                {
                    Success = true,
                };

            }
            catch (Exception e)
            {
                log.Error("Error", e);
                return new OperationResult
                {
                    Success = false,
                    ErrorMessage = e.Message
                };
            }
        }

        public OperationResult RemoveAllFuncsFromDb()
        {
            try
            {
                var dbContext = new AlohaDb();

                var result = dbContext.UserFuncs.ToList();
                dbContext.UserFuncs.RemoveRange(result);
                dbContext.SaveChanges();

                return new OperationResult
                {
                    Success = true,
                };
            }
            catch (Exception e)
            {
                log.Error("Error", e);
                return new OperationResult
                {
                    Success = false,
                    ErrorMessage = e.Message
                };
            }
        }

        public OperationResult RemoveAllUserGroupsFromDb()
        {
            try
            {
                var dbContext = new AlohaDb();

                var result = dbContext.UserGroups.ToList();
                dbContext.UserGroups.RemoveRange(result);
                dbContext.SaveChanges();

                return new OperationResult
                {
                    Success = true,
                };
            }
            catch (Exception e)
            {
                log.Error("Error", e);
                return new OperationResult
                {
                    Success = false,
                    ErrorMessage = e.Message
                };
            }
        }
    }
}