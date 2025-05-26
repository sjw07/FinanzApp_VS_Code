using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace FinanzApp.Data
{
    public class FinanceEntry
    {
        public DateTime Datum { get; set; }
        public decimal Betrag { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class FinanceService
    {
        const string DbFileName = "StefanFinanceMai2025.db";
        readonly string _dbPath = Path.Combine(AppContext.BaseDirectory, "Data", DbFileName);

        public FinanceService()
        {
        }

        public async Task<List<FinanceEntry>> GetEntriesAsync()
        {
            var result = new List<FinanceEntry>();
            if (!File.Exists(_dbPath))
                return result;
            using var connection = new SqliteConnection($"Data Source={_dbPath}");
            await connection.OpenAsync();
            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT Datum, Betrag, Name FROM Entries ORDER BY Datum";
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var entry = new FinanceEntry
                {
                    Datum = DateTime.Parse(reader.GetString(0)),
                    Betrag = reader.GetDecimal(1),
                    Name = reader.GetString(2)
                };
                result.Add(entry);
            }
            return result;
        }
    }
}
