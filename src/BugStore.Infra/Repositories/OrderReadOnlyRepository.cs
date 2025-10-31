using BugStore.Domain.Entities;
using BugStore.Domain.Interfaces;
using BugStore.Infra.Data;
using Microsoft.EntityFrameworkCore;
using Dapper;
using System.Data;

namespace BugStore.Infra.Repositories
{
    internal class OrderReadOnlyRepository : IOrderReadOnlyRepository
    {
        private readonly BugStoreDbContext _dbContext;

        public OrderReadOnlyRepository(BugStoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Order?> GetByIdAsync(Guid id)
        {
            return await _dbContext
                .Orders.Include(o => o.Lines)
                .ThenInclude(ol => ol.Product)
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<IEnumerable<Order>> GetAllAsync(int page, int pageSize)
        {
            return await _dbContext
                .Orders.Include(o => o.Lines)
                .ThenInclude(ol => ol.Product)
                .Include(o => o.Customer)
                .AsNoTracking()
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetByCustomerIdAsync(
            Guid customerId,
            int page,
            int pageSize
        )
        {
            return await _dbContext
                .Orders.Include(o => o.Lines)
                .ThenInclude(ol => ol.Product)
                .Include(o => o.Customer)
                .AsNoTracking()
                .Where(o => o.CustomerId == customerId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<long> GetTotalItemAsync()
        {
            return await _dbContext.Orders.CountAsync();
        }

        public async Task<(long TotalOrders, decimal TotalRevenue)> GetTotalByPeriod(DateTime start, DateTime end)
        {
            const string sql = @"
                SELECT
                    COUNT(DISTINCT o.""Id"") as TotalOrders,
                    COALESCE(SUM(ol.""Total""), 0) as TotalRevenue
                FROM ""Orders"" o
                LEFT JOIN ""OrderLines"" ol ON o.""Id"" = ol.""OrderId""
                WHERE o.""CreatedAt"" >= @Start AND o.""CreatedAt"" <= @End";

            using var connection = _dbContext.Database.GetDbConnection();
            if (connection.State != ConnectionState.Open)
                await connection.OpenAsync();

            var result = await connection.QuerySingleAsync<PeriodTotalResult>(sql, new { Start = start, End = end });

            return (result.TotalOrders, result.TotalRevenue);
        }

        public async Task<(long TotalOrders, decimal TotalSpent)> GetTotalByCustomerIdAsync(Guid customerId)
        {
            const string sql = @"
                SELECT
                    COUNT(DISTINCT o.""Id"") as TotalOrders,
                    COALESCE(SUM(ol.""Total""), 0) as TotalSpent
                FROM ""Orders"" o
                LEFT JOIN ""OrderLines"" ol ON o.""Id"" = ol.""OrderId""
                WHERE o.""CustomerId"" = @CustomerId";

            using var connection = _dbContext.Database.GetDbConnection();
            if (connection.State != ConnectionState.Open)
                await connection.OpenAsync();

            var result = await connection.QuerySingleAsync<CustomerTotalResult>(sql, new { CustomerId = customerId });

            return (result.TotalOrders, result.TotalSpent);
        }

        public async Task<IEnumerable<(Guid CustomerId, string CustomerName, long TotalOrders, decimal TotalSpent)>> GetBestCustomersAsync(int topCustomers)
        {
            const string sql = @"
                SELECT
                    o.""CustomerId"",
                    c.""Name"" as CustomerName,
                    COUNT(DISTINCT o.""Id"") as TotalOrders,
                    SUM(ol.""Total"") as TotalSpent
                FROM ""Orders"" o
                INNER JOIN ""Customers"" c ON o.""CustomerId"" = c.""Id""
                INNER JOIN ""OrderLines"" ol ON o.""Id"" = ol.""OrderId""
                GROUP BY o.""CustomerId"", c.""Name""
                ORDER BY SUM(ol.""Total"") DESC
                LIMIT @TopCustomers";

            using var connection = _dbContext.Database.GetDbConnection();
            if (connection.State != ConnectionState.Open)
                await connection.OpenAsync();

            var results = await connection.QueryAsync<BestCustomerResult>(sql, new { TopCustomers = topCustomers });

            return results.Select(r => (r.CustomerId, r.CustomerName, r.TotalOrders, r.TotalSpent));
        }

        private class BestCustomerResult
        {
            public Guid CustomerId { get; set; }
            public string CustomerName { get; set; } = string.Empty;
            public long TotalOrders { get; set; }
            public decimal TotalSpent { get; set; }
        }

        private class PeriodTotalResult
        {
            public long TotalOrders { get; set; }
            public decimal TotalRevenue { get; set; }
        }

        private class CustomerTotalResult
        {
            public long TotalOrders { get; set; }
            public decimal TotalSpent { get; set; }
        }
    }
}
