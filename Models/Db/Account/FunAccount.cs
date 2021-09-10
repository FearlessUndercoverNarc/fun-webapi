using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Models.Db.Common;
using Models.Db.Sessions;
using Models.Db.Tree;

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

        public bool HasSubscription { get; set; }

        public virtual ICollection<Folder> AuthoredFolders { get; set; }
        
        public virtual ICollection<Desk> AuthoredDesks { get; set; }

        public virtual ICollection<DeskActionHistoryItem> FiredActions { get; set; }

        // public virtual ICollection<Folder> SharedFolders { get; set; }
        //
        // public virtual ICollection<FolderShare> SharedFoldersRelation { get; set; }

        public virtual ICollection<TokenSession> TokenSessions { get; set; }
    }
}