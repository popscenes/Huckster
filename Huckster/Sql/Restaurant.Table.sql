-- [[SchemaName:dbo]]
-- [[TableName:Restaurant]]
-- [[FileName:dbo.Restaurant.Table.sql]]

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF (NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  TABLE_NAME = 'Restaurant'))
BEGIN
    CREATE TABLE [dbo].[Restaurant](
        [Id] bigint PRIMARY KEY IDENTITY(1,1),
		[AggregateRootId] uniqueidentifier not null,
		[Name] nvarchar(256) NOT NULL,
		[Description] nvarchar(MAX) NOT NULL,
		[TimeZoneId] nvarchar(256) NOT NULL,
		[TileImageUrl] nvarchar(256) NOT NULL,
		[ContactPhone] nvarchar(256) NULL,
		[Email] nvarchar(256) NULL,
    ) --ON [PRIMARY]
END
ELSE
BEGIN
    PRINT 'Skipped create table [dbo].[Restaurant]'
END
GO

if not exists (select column_name from INFORMATION_SCHEMA.columns where table_name = 'Restaurant' and column_name = 'ContactPhone')
BEGIN
    ALTER TABLE Restaurant ADD [ContactPhone] nvarchar(256) NULL
	PRINT 'created column [ContactPhone] on table [dbo].[Restaurant]'

	ALTER TABLE Restaurant ADD [Email] nvarchar(256) NULL
	PRINT 'created column [Email] on table [dbo].[Restaurant]'
END
GO

if not exists (select column_name from INFORMATION_SCHEMA.columns where table_name = 'Restaurant' and column_name = 'Surge')
BEGIN
    ALTER TABLE Restaurant ADD [Surge] bit NOT NULL DEFAULT(0)
	PRINT 'created column [Surge] on table [dbo].[Restaurant]'

	ALTER TABLE Restaurant ADD [SurgePct] int NOT NULL DEFAULT(0)
	PRINT 'created column [SurgePct] on table [dbo].[Restaurant]'
END
GO