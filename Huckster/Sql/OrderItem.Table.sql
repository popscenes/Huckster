-- [[SchemaName:dbo]]
-- [[TableName:OrderItem]]
-- [[FileName:dbo.OrderItem.Table.sql]]

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF (NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  TABLE_NAME = 'OrderItem'))
BEGIN
    CREATE TABLE [dbo].[OrderItem](
        [Id] bigint PRIMARY KEY IDENTITY(1,1),
        ParentAggregateId uniqueidentifier NOT NULL,
		Name nvarchar(256) NOT NULL,
        Price decimal NOT NULL,
        Quantity int NOT NULL,
        Notes nvarchar(max) NULL
) --ON [PRIMARY]
END
ELSE
BEGIN
    PRINT 'Skipped create table [dbo].[OrderItem]'
END
GO