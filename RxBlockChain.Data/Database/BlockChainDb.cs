using Microsoft.EntityFrameworkCore;
using RxBlockChain.Model.Entities;

namespace RxBlockChain.Data.Database
{
    public class BlockChainDb : DbContext
    {
        public BlockChainDb(DbContextOptions<BlockChainDb> options) : base(options)
        {
        }

        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<Transactions> Transactions { get; set; }
        public DbSet<Block> Blocks { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<Validators> Validators { get; set; }
        public DbSet<Node>  Nodes { get; set; }
        //public DbSet<PosConsensus> PosConsensus { get; set; }
       // public DbSet<WalletKeyId> WalletKeyIds { get; set; }
        public DbSet<SmartContract> smartContracts { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Explicitly specify the SQL column type for decimals to avoid truncation issues.
            modelBuilder.Entity<Transactions>()
                .Property(t => t.Amount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Transactions>()
                .Property(t => t.Fee)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Wallet>()
                .Property(w => w.Balance)
                .HasColumnType("decimal(18,2)");
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Validators>().HasIndex(v => v.WalletAddress).IsUnique();

            modelBuilder.Entity<SmartContract>()
                .Ignore(sc => sc.State);

        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var item in ChangeTracker.Entries<BaseEntity>())
            {
                switch (item.State)
                {
                    case EntityState.Modified:
                        item.Entity.ModifiedAt = DateTime.UtcNow;
                        break;
                    case EntityState.Deleted:
                        item.Entity.IsDeleted = true;
                        break;
                    case EntityState.Added:
                        item.Entity.Id = Guid.NewGuid();
                        item.Entity.CreatedAt = DateTime.UtcNow;
                        break;
                    default:
                        break;
                }
            }
            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
