### Build- und Start-Anleitung
1. dotnet tool install --global dotnet-ef
2. dotnet restore
3. dotnet ef migrations add InitialCreate
4. dotnet ef database update   # optional – wird sonst zur Laufzeit ausgeführt
5. dotnet run --project FinanceApp.csproj
6. Mit einem SQLite-Tool (z.B. VS Code Extension "SQLite (alexcvzz)") die Datei Data/finanzapp.db 
   öffnen und z. B. abfragen:
   SELECT * FROM Transactions WHERE UserId = 1 AND strftime('%Y-%m', Date) = '2025-05';
