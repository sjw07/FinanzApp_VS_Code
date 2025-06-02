using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;

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
        readonly string _dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), DbFileName);

        public static event EventHandler? EntriesChanged;
        static void RaiseEntriesChanged() => EntriesChanged?.Invoke(null, EventArgs.Empty);

        public FinanceService()
        {
        }

        string GetTableName(int userId) => userId == 1 ? "Entries" : $"Entries{userId}";

        public static string HashPassword(string password)
        {
            using var sha = System.Security.Cryptography.SHA256.Create();
            var bytes = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return string.Concat(bytes.Select(b => b.ToString("x2")));
        }

        async Task<int?> GetUserIdAsync(string username)
        {
            using var connection = new SqliteConnection($"Data Source={_dbPath}");
            await connection.OpenAsync();
            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT Id FROM Users WHERE Username=@u";
            cmd.Parameters.AddWithValue("@u", username);
            var val = await cmd.ExecuteScalarAsync();
            if (val == null || val == DBNull.Value)
                return null;
            return Convert.ToInt32(val);
        }

        public async Task<(int Id, string PasswordHash)?> GetUserAsync(string username)
        {
            using var connection = new SqliteConnection($"Data Source={_dbPath}");
            await connection.OpenAsync();
            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT Id, PasswordHash FROM Users WHERE Username=@u";
            cmd.Parameters.AddWithValue("@u", username);
            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
                return (reader.GetInt32(0), reader.GetString(1));
            return null;
        }

        public async Task<bool> RegisterUserAsync(string username, string password)
        {
            using var connection = new SqliteConnection($"Data Source={_dbPath}");
            await connection.OpenAsync();

            var check = connection.CreateCommand();
            check.CommandText = "SELECT COUNT(*) FROM Users WHERE Username=@u";
            check.Parameters.AddWithValue("@u", username);
            var count = Convert.ToInt32(await check.ExecuteScalarAsync());
            if (count > 0)
                return false;

            var hash = HashPassword(password);
            var insert = connection.CreateCommand();
            insert.CommandText = "INSERT INTO Users (Username, PasswordHash) VALUES (@u,@p)";
            insert.Parameters.AddWithValue("@u", username);
            insert.Parameters.AddWithValue("@p", hash);
            await insert.ExecuteNonQueryAsync();
            var getId = connection.CreateCommand();
            getId.CommandText = "SELECT last_insert_rowid();";
            long id = (long)(await getId.ExecuteScalarAsync())!;
            string table = GetTableName((int)id);
            var create = connection.CreateCommand();
            create.CommandText = $"CREATE TABLE IF NOT EXISTS {table} (Id INTEGER PRIMARY KEY AUTOINCREMENT, Datum DATE NOT NULL, Betrag REAL NOT NULL, Name TEXT NOT NULL, UserId INTEGER NOT NULL, FOREIGN KEY (UserId) REFERENCES Users(Id))";
            await create.ExecuteNonQueryAsync();
            var unique = connection.CreateCommand();
            unique.CommandText = $"CREATE UNIQUE INDEX IF NOT EXISTS idx_entries{(int)id}_unique ON {table}(Datum, Betrag, Name, UserId)";
            await unique.ExecuteNonQueryAsync();
            return true;
        }

        public async Task<List<FinanceEntry>> GetEntriesAsync(string? user)
        {
            var result = new List<FinanceEntry>();
            if (!File.Exists(_dbPath) || string.IsNullOrEmpty(user))
                return result;

            var id = await GetUserIdAsync(user);
            if (id is null)
                return result;

            var table = GetTableName(id.Value);

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

            var id = await GetUserIdAsync(user);
            if (id is null)
                return false;

            var table = GetTableName(id.Value);

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
            RaiseEntriesChanged();
            return true;
        }

        public async Task<bool> UpdateEntryAsync(string? user,
                                                 DateTime oldDatum, decimal oldBetrag, string oldName,
                                                 DateTime newDatum, decimal newBetrag, string newName)
        {
            if (string.IsNullOrEmpty(user))
                return false;

            var id = await GetUserIdAsync(user);
            if (id is null)
                return false;

            var table = GetTableName(id.Value);

            using var connection = new SqliteConnection($"Data Source={_dbPath}");
            await connection.OpenAsync();

            var check = connection.CreateCommand();
            check.CommandText = $"SELECT COUNT(*) FROM {table} WHERE Datum=@d AND Betrag=@b AND Name=@n";
            check.Parameters.AddWithValue("@d", newDatum.ToString("yyyy-MM-dd"));
            check.Parameters.AddWithValue("@b", newBetrag);
            check.Parameters.AddWithValue("@n", newName);
            var count = Convert.ToInt32(await check.ExecuteScalarAsync());
            if (count > 0 && (oldDatum != newDatum || oldBetrag != newBetrag || oldName != newName))
                return false;

            var update = connection.CreateCommand();
            update.CommandText = $"UPDATE {table} SET Datum=@nd, Betrag=@nb, Name=@nn WHERE Datum=@od AND Betrag=@ob AND Name=@on";
            update.Parameters.AddWithValue("@nd", newDatum.ToString("yyyy-MM-dd"));
            update.Parameters.AddWithValue("@nb", newBetrag);
            update.Parameters.AddWithValue("@nn", newName);
            update.Parameters.AddWithValue("@od", oldDatum.ToString("yyyy-MM-dd"));
            update.Parameters.AddWithValue("@ob", oldBetrag);
            update.Parameters.AddWithValue("@on", oldName);

            var affected = await update.ExecuteNonQueryAsync();
            if (affected > 0)
                RaiseEntriesChanged();
            return affected > 0;
        }

        public async Task<bool> DeleteEntryAsync(string? user,
                                                 DateTime datum, decimal betrag, string name)
        {
            if (string.IsNullOrEmpty(user))
                return false;

            var id = await GetUserIdAsync(user);
            if (id is null)
                return false;

            var table = GetTableName(id.Value);

            using var connection = new SqliteConnection($"Data Source={_dbPath}");
            await connection.OpenAsync();

            var cmd = connection.CreateCommand();
            cmd.CommandText = $"DELETE FROM {table} WHERE Datum=@d AND Betrag=@b AND Name=@n";
            cmd.Parameters.AddWithValue("@d", datum.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("@b", betrag);
            cmd.Parameters.AddWithValue("@n", name);

            var affected = await cmd.ExecuteNonQueryAsync();
            if (affected > 0)
                RaiseEntriesChanged();
            return affected > 0;
        }

        public Dictionary<(int Year, int Month), decimal> CalculateMonthlyBalances(List<FinanceEntry> entries)
        {
            var result = new Dictionary<(int, int), decimal>();
            var sorted = entries.OrderBy(e => e.Datum).ToList();

            int startYear = 2020;
            int endYear = 2030;
            int index = 0;
            decimal running = 0m;

            for (int year = startYear; year <= endYear; year++)
            {
                for (int month = 1; month <= 12; month++)
                {
                    while (index < sorted.Count && sorted[index].Datum.Year == year && sorted[index].Datum.Month == month)
                    {
                        running += sorted[index].Betrag;
                        index++;
                    }
                    result[(year, month)] = running;
                }
            }
            return result;
        }
    }
}
