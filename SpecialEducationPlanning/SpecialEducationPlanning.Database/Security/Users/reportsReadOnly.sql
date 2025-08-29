IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = N'reportsReadOnly')
    CREATE USER [reportsReadOnly] FOR LOGIN [reportsReadOnly];
GO