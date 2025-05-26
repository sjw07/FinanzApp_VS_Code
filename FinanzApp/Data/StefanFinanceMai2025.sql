-- Schema for StefanFinanceMai2025
CREATE TABLE Entries (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Datum TEXT NOT NULL,
    Betrag REAL NOT NULL,
    Name TEXT NOT NULL
);

INSERT INTO Entries (Datum, Betrag, Name) VALUES
('2025-05-01', 3000.0, 'Gehalt'),
('2025-05-02', -50.0, 'Tanken'),
('2025-05-03', -30.0, 'Restaurant'),
('2025-05-04', -5.0, 'Kaffee'),
('2025-05-05', -3.0, 'ÖPNV'),
('2025-05-06', -12.0, 'Abo: Streaming'),
('2025-05-07', -80.0, 'Supermarkt'),
('2025-05-08', -25.0, 'Fitnessstudio'),
('2025-05-09', -60.0, 'Strom'),
('2025-05-10', -40.0, 'Wasser'),
('2025-05-11', -700.0, 'Miete'),
('2025-05-12', -35.0, 'Internet'),
('2025-05-13', -20.0, 'Handy'),
('2025-05-14', -55.0, 'Tanken'),
('2025-05-15', -40.0, 'Restaurant'),
('2025-05-16', -6.0, 'Kaffee'),
('2025-05-17', -3.0, 'ÖPNV'),
('2025-05-18', -10.0, 'Abo: Musik'),
('2025-05-19', -90.0, 'Supermarkt'),
('2025-05-20', -100.0, 'Versicherungen'),
('2025-05-21', -60.0, 'Kleidung'),
('2025-05-22', -50.0, 'Geschenke'),
('2025-05-23', -15.0, 'Kino'),
('2025-05-24', -60.0, 'Tanken'),
('2025-05-25', -45.0, 'Restaurant'),
('2025-05-26', -5.0, 'Kaffee'),
('2025-05-27', -3.0, 'ÖPNV'),
('2025-05-28', -20.0, 'Abo: Software'),
('2025-05-29', -85.0, 'Supermarkt'),
('2025-05-30', -500.0, 'Sparen');
