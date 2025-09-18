using Npgsql;
namespace MRP.Data
{
    internal class PostgresDB
    {
        private static string connectionString = "Host=localhost;Port=5432;Username=postgres;Password=1234;Database=mrp_db;";
        private NpgsqlDataSource _dataSource;
        private PostgresDB(NpgsqlDataSource dataSource) {
            _dataSource = dataSource;
        }
        public static PostgresDB Initialize(){
            var dataSource = NpgsqlDataSource.Create(connectionString);

            return new PostgresDB(dataSource);
        }
        public async Task<int> ChangeData(string prepared_sql, Dictionary<string, object> values)
        {
            await using var sql = _dataSource.CreateCommand(prepared_sql);
            foreach(var kv in values) {
                sql.Parameters.AddWithValue(kv.Key, kv.Value ?? DBNull.Value);
            }
            return await sql.ExecuteNonQueryAsync();
        }
        public async Task<NpgsqlDataReader> ReadData(string prepared_sql, Dictionary<string, object> values)
        {
            await using var sql = _dataSource.CreateCommand(prepared_sql);
            foreach (var kv in values)
            {
                sql.Parameters.AddWithValue(kv.Key, kv.Value ?? DBNull.Value);
            }
            return await sql.ExecuteReaderAsync();
        }
    }
}
