-- [[SchemaName:dbo]]
-- [[TableName:Menu]]
-- [[FileName:dbo.Menu.Table.sql]]

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF (NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  TABLE_NAME = 'Menu'))
BEGIN
    CREATE TABLE [dbo].[Menu](
        [Id] bigint PRIMARY KEY IDENTITY(1,1),
		[ParentAggregateId] uniqueidentifier not null,
		[Title] nvarchar(256) not null,
		[Description] nvarchar(MAX) null,
		[Order] int null,
		[Deleted] bit not null default(0)
) --ON [PRIMARY]
END
ELSE
BEGIN
    PRINT 'Skipped create table [dbo].[Menu]'
END
GO

if not exists (select column_name from INFORMATION_SCHEMA.columns where table_name = 'Menu' and column_name = 'Deleted')
BEGIN
    ALTER TABLE [Menu] ADD [Deleted] bit not null default(0)
	PRINT 'created column [Deleted] on table [dbo].[Menu]'
END
GO