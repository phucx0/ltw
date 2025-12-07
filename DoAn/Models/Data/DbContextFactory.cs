using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace DoAn.Models.Data
{
    public interface IDbContextFactory
    {
        ModelContext Create(string database, string user, string password);
    }

    public class DbContextFactory : IDbContextFactory
    {
        public ModelContext Create(string database, string user, string password)
        {
            var connectionString = new SqlConnectionStringBuilder
            {
                DataSource = "localhost", // hoặc localhost\\SQLEXPRESS
                InitialCatalog = database,
                UserID = user,
                Password = password,
                TrustServerCertificate = true,
                Encrypt = false,
                Pooling = true,
                ConnectTimeout = 30
            }.ToString();

            var options = new DbContextOptionsBuilder<ModelContext>()
                .UseSqlServer(connectionString)
                .Options;

            return new ModelContext(options);
        }
    }
}