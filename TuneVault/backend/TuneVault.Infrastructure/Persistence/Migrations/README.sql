-- Migration 1: Initial schema (run via EF: dotnet ef migrations add InitialCreate)
-- Migration 2: Indexes (run via EF: dotnet ef migrations add AddIndexes)
-- For local demo, DbSeeder uses EnsureCreatedAsync() automatically.

CREATE TABLE IF NOT EXISTS __MigrationNote (
    Id INT PRIMARY KEY,
    Note NVARCHAR(200)
);
INSERT INTO __MigrationNote VALUES (1, 'Use EF migrations for submission - see README');
