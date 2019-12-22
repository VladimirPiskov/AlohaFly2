using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlohaService.Entities
{
    public class UserUserGroup
    {
        public long Id { get; set; }

        public long UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }

        public long GroupId { get; set; }
        [ForeignKey("GroupId")]
        public UserGroup UserGroup { get; set; }
    }
}