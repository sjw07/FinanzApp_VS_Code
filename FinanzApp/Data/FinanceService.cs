using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;

namespace FinanzApp.Data
{
    public class FinanceEntry : INotifyPropertyChanged
    {
        public DateTime Datum { get; set; }
        public decimal Betrag { get; set; }
        public string Name { get; set; } = string.Empty;

        public bool IsNegative => Betrag < 0;

        bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSelected)));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }

    public class FinanceService
    {
        const string DbFileName = "FinanzApp.db";
        readonly string _dbPath = Path.Combine(AppContext.BaseDirectory, "Data", DbFileName);

        public FinanceService()
        {
        }

        public async Task<List<FinanceEntry>> GetEntriesAsync(string? user)
        {
            var result = new List<FinanceEntry>();
            if (!File.Exists(_dbPath))
                return result;

            var table = user switch
            {
                "Stefan" => "Entries",
                "Stefan2" => "Entries2",
                "Stefan3" => "Entries3",
                "Stefan4" => "Entries4",
                _ => "Entries"
            };

            using var connection = new SqliteConnection($"Data Source={_dbPath}");
            await connection.OpenAsync();
            var cmd = connection.CreateCommand();
            cmd.CommandText = $"SELECT Datum, Betrag, Name FROM {table} ORDER BY Datum";
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

        public async Task<bool> AddEntryAsync(string? user, DateTime datum, decimal betrag, string name)
        {
            if (string.IsNullOrEmpty(user))
                return false;

            var (table, id) = user switch
            {
                "Stefan" => ("Entries", 1),
                "Stefan2" => ("Entries2", 2),
                "Stefan3" => ("Entries3", 3),
                "Stefan4" => ("Entries4", 4),
                _ => ("Entries", 1)
            };

            using var connection = new SqliteConnection($"Data Source={_dbPath}");
            await connection.OpenAsync();

            var check = connection.CreateCommand();
            check.CommandText = $"SELECT COUNT(*) FROM {table} WHERE Datum=@d AND Betrag=@b AND Name=@n";
            check.Parameters.AddWithValue("@d", datum.ToString("yyyy-MM-dd"));
            check.Parameters.AddWithValue("@b", betrag);
            check.Parameters.AddWithValue("@n", name);
            var count = Convert.ToInt32(await check.ExecuteScalarAsync());
            if (count > 0)
                return false;

            var insert = connection.CreateCommand();
            insert.CommandText = $"INSERT INTO {table} (Datum, Betrag, Name, UserId) VALUES (@d, @b, @n, @uid)";
            insert.Parameters.AddWithValue("@d", datum.ToString("yyyy-MM-dd"));
            insert.Parameters.AddWithValue("@b", betrag);
            insert.Parameters.AddWithValue("@n", name);
            insert.Parameters.AddWithValue("@uid", id);
            await insert.ExecuteNonQueryAsync();
            return true;
        }
    }
}
