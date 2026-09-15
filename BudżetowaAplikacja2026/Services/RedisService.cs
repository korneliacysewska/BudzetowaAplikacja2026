namespace BudżetowaAplikacja2026.Services
{
    using StackExchange.Redis;

    public class RedisService
    {
        private readonly ConnectionMultiplexer _connection;

        public RedisService(string connectionString)
        {
            _connection = ConnectionMultiplexer.Connect(connectionString);
        }

        public IDatabase GetDatabase()
        {
            return _connection.GetDatabase();
        }
    }
}
