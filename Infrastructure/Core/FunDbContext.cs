using System;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Models.Db.Account;
using Models.Db.Common;
using Models.Db.Sessions;

namespace Infrastructure.Core
{
    public class FunDbContext : DbContext
    {
        public FunDbContext()
        {
        }

        public FunDbContext(DbContextOptions<FunDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder.IsConfigured) return;
#if DEBUG
            // Console.WriteLine("Using debug connection");
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=FUN;Username=postgres;Password=root");
#else
            // Console.WriteLine("Using Environment variable connection");
            var connectionString = Environment.GetEnvironmentVariable("CONN_STR");

            if (connectionString == null) throw new ArgumentNullException("env:CONN_STR NOT PASSED");
            optionsBuilder.UseNpgsql(connectionString);
#endif
            // TODO: Remove this line for prod
            optionsBuilder.EnableSensitiveDataLogging();
        }

        private static LambdaExpression IsDeletedRestriction(Type type)
        {
            var propMethod = typeof(EF).GetMethod(nameof(EF.Property),
                BindingFlags.Static |
                BindingFlags.Public)?.MakeGenericMethod(typeof(bool));

            var parameterExpression = Expression.Parameter(type, "it");
            var constantExpression = Expression.Constant(nameof(IdEntity.IsSoftDeleted));

            var methodCallExpression = Expression.Call(
                propMethod ??
                throw new InvalidOperationException(), parameterExpression, constantExpression);

            var falseConst = Expression.Constant(false);
            var expressionCondition = Expression.MakeBinary(ExpressionType.Equal, methodCallExpression, falseConst);

            return Expression.Lambda(expressionCondition, parameterExpression);
        }

        private static void SetupSoftDelete(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                //other automated configurations left out
                if (!typeof(IdEntity).IsAssignableFrom(entityType.ClrType)) continue;

                entityType.AddIndex(entityType.FindProperty(nameof(IdEntity.IsSoftDeleted)));

                modelBuilder
                    .Entity(entityType.ClrType)
                    .HasQueryFilter(IsDeletedRestriction(entityType.ClrType));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            SetupSoftDelete(modelBuilder);

            modelBuilder.Entity<TokenSession>().HasIndex(t => t.Token);
        }

        public DbSet<FunAccount> FunAccounts { get; set; }

        public DbSet<TokenSession> TokenSessions { get; set; }
    }
}