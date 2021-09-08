using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Models.Db.Account;
using Models.Db.Common;

namespace Models.Db.Sessions
{
    public class TokenSession : IdEntity
    {
        [MaxLength(36)]
        public string Token { get; set; }

        [ForeignKey(nameof(FunAccount))]
        public long FunAccountId { get; set; }

        public virtual FunAccount FunAccount { get; set; }

        public DateTime StartDate { get; set; }

        // Not null, because token has an expiration date
        public DateTime EndDate { get; set; }
    }
}