BEGIN TRANSACTION;
CREATE UNIQUE INDEX IF NOT EXISTS idx_entries_unique ON Entries(Datum, Betrag, Name, UserId);
CREATE UNIQUE INDEX IF NOT EXISTS idx_entries2_unique ON Entries2(Datum, Betrag, Name, UserId);
CREATE UNIQUE INDEX IF NOT EXISTS idx_entries3_unique ON Entries3(Datum, Betrag, Name, UserId);
CREATE UNIQUE INDEX IF NOT EXISTS idx_entries4_unique ON Entries4(Datum, Betrag, Name, UserId);
COMMIT;
