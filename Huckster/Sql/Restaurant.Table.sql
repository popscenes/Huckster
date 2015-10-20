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
    ) --ON [PRIMARY]
END
ELSE
BEGIN
    PRINT 'Skipped create table [dbo].[Restaurant]'
END
GO