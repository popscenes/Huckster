-- [[SchemaName:dbo]]
-- [[TableName:Address]]
-- [[FileName:dbo.Address.Table.sql]]

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF (NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  TABLE_NAME = 'Address'))
BEGIN
    CREATE TABLE [dbo].[Address](
        [Id] bigint PRIMARY KEY IDENTITY(1,1),
		[ParentAggregateId] uniqueidentifier not null,
		[Number] nvarchar(64) not null,
		[Street] nvarchar(256) not null,
		[Suburb] nvarchar(256) not null,
		[Postcode] nvarchar(64) not null,
		[City]  nvarchar(256) not null,
		[State]  nvarchar(64) not null,
		[Latitude] float null,
		[Longitude] float null,

    ) --ON [PRIMARY]
END
ELSE
BEGIN
    PRINT 'Skipped create table [dbo].[Address]'
END
GO