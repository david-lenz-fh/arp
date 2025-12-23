using Npgsql;
namespace DataAccess
{
    public class PostgresDB
    {
        private NpgsqlDataSource _dataSource;
        private PostgresDB(NpgsqlDataSource dataSource) {
            _dataSource = dataSource;
        }
        public static PostgresDB Initialize(string connectionString)   {
            var dataSource = NpgsqlDataSource.Create(connectionString);

            return new PostgresDB(dataSource);
        }
        public async Task<int> SQLWithoutReturns(string prepared_sql, Dictionary<string, object?> values)
        {
            try
            {
                await using var sql = _dataSource.CreateCommand(prepared_sql);
                foreach (var kv in values)
                {
                    sql.Parameters.AddWithValue(kv.Key, kv.Value ?? DBNull.Value);
                }
                return await sql.ExecuteNonQueryAsync();
            }
            catch (Exception)
            {
                return -1;
            }
        }
        public async Task<NpgsqlDataReader?> SQLWithReturns(string prepared_sql, Dictionary<string, object?> values)
        {
            try
            {
                await using var sql = _dataSource.CreateCommand(prepared_sql);
                foreach (var kv in values)
                {
                    sql.Parameters.AddWithValue(kv.Key, kv.Value ?? DBNull.Value);
                }
                return await sql.ExecuteReaderAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
