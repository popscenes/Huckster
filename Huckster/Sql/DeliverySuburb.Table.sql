-- [[SchemaName:dbo]]
-- [[TableName:DeliverySuburb]]
-- [[FileName:dbo.DeliverySuburb.Table.sql]]

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF (NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  TABLE_NAME = 'DeliverySuburb'))
BEGIN
    CREATE TABLE [dbo].[DeliverySuburb](
        [Id] bigint PRIMARY KEY IDENTITY(1,1),
		[ParentAggregateId] uniqueidentifier not null,
		[SuburbId] [int]  NOT NULL,
		[postcode] [varchar](5) NULL,
		[suburb] [varchar](100) NULL,
		[state] [varchar](4) NULL,
		[latitude] [decimal](6, 3) NULL DEFAULT (NULL),
		[longitude] [decimal](6, 3) NULL DEFAULT (NULL),

    ) --ON [PRIMARY]
END
ELSE
BEGIN
    PRINT 'Skipped create table [dbo].[DeliverySuburb]'
END
GO