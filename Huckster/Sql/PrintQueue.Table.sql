-- [[SchemaName:dbo]]
-- [[TableName:PrintQueue]]
-- [[FileName:dbo.PrintQueue.Table.sql]]

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF (NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  TABLE_NAME = 'PrintQueue'))
BEGIN
    CREATE TABLE [dbo].[PrintQueue](
        [Id] bigint PRIMARY KEY IDENTITY(1,1),
		[RestaurantId] int not null,
		[RestaurantAggrgateRootId] uniqueidentifier NOT NULL,
		[OrderAggrgateRootId] uniqueidentifier NOT NULL,
		[DateTimeAdded] datetime2 NOT NULL,
		[Printed] bit NOT NULL,
		[PrintRequestXML] nvarchar(max) NOT NULL

    ) --ON [PRIMARY]
END
ELSE
BEGIN
    PRINT 'Skipped create table [dbo].[PrintQueue]'
END
GO