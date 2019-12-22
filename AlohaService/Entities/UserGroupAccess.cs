using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlohaService.Entities
{
    public class UserGroupAccess
    {
        public long Id { get; set; }

        public long UserGroupId { get; set; }
        [ForeignKey("UserGroupId")]
        public UserGroup UserGroup { get; set; }

        public long UserFuncId { get; set; }
        [ForeignKey("UserFuncId")]
        public UserFunc UserFunc { get; set; }

        public int AccessId { get; set; }
    }
}