using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Models.Db.Common;
using Models.Db.Sessions;

namespace Models.Db.Account
{
    public class FunAccount : IdEntity
    {
        [MaxLength(32)]
        public string Login { get; set; }

        [MaxLength(256)]
        public string Fio { get; set; }

        [MaxLength(32)]
        public string Password { get; set; }
        
        public virtual ICollection<TokenSession> TokenSessions { get; set; }
    }
}