using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RxBlockChain.Data.Database;
using RxBlockChain.Core.Interface.iRepositories;
using RxBlockChain.Core.Interface.iServices;
using RxBlockChain.Core.Services;
using RxBlockChain.Data.Repositories;
using RxBlockChain.Data.UnitOfWorks;

namespace RxBlockChain.Data
{
    public static class DiExtensions
    {
        public static void AddDependencies(this IServiceCollection services, IConfiguration config)
        {
            // Configure DbContext to use the connection string from appsettings.json
            string connectionString = config.GetConnectionString("BlockchainConnect");
            services.AddDbContext<BlockChainDb>(options =>
                options.UseSqlServer(connectionString));

            // Register repositories and UnitOfWork
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IWalletService, WalletService>();
            services.AddScoped<ITransactionService, TransactionService>();
            services.AddScoped<IBlockService, BlockService>();
            services.AddScoped<ISmartContractService, SmartContractService>();
            services.AddScoped<INodeService, NodeService>();




            //    services.AddControllers()
            //      .AddJsonOptions(options =>
            //      {
            //          // Prevent circular references globally
            //          options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;

            //          // If using Newtonsoft.Json, you can use:
            //          options.JsonSerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            //      });
            //}



        }

    }
}
