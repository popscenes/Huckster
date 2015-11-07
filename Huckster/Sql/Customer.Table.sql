-- [[SchemaName:dbo]]
-- [[TableName:Customer]]
-- [[FileName:dbo.Customer.Table.sql]]

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF (NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  TABLE_NAME = 'Customer'))
BEGIN
    CREATE TABLE [dbo].[Customer](
		[Id] bigint PRIMARY KEY IDENTITY(1,1),
        [AggregateRootId] uniqueidentifier NOT NULL,
		Name nvarchar(MAX),
		Mobile nvarchar(MAX),
		Email  nvarchar(MAX)

) --ON [PRIMARY]
END
ELSE
BEGIN
    PRINT 'Skipped create table [dbo].[Customer]'
END
GO