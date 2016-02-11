-- [[SchemaName:dbo]]
-- [[TableName:Customer]]
-- [[FileName:dbo.Customer.Table.sql]]

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF (NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  TABLE_NAME = 'Order'))
BEGIN
CREATE TABLE [dbo].[Order](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[AggregateRootId] [uniqueidentifier] NOT NULL,
	[RestaurantId] [uniqueidentifier] NOT NULL,
	[CustomerId] [uniqueidentifier] NOT NULL,
	[DeliverySuburbId] [int] NOT NULL,
	[DeliveryTime] [datetime2](7) NOT NULL,
	[CustomerMobile] [nvarchar](64) NOT NULL,
	[CustomerEmail] [nvarchar](256) NOT NULL,
	[CreateDateTime] [datetime2](7) NOT NULL,
	[LastModifiedDateTime] [datetime2](7) NOT NULL,
	[Status] [nvarchar](64) NOT NULL,
	[CompanyName] nvarchar(256) NULL,
	[Instructions] nvarchar(MAX) NULL 

) --ON [PRIMARY]
END
ELSE
BEGIN
    PRINT 'Skipped create table [dbo].[Order]'
END
GO
if not exists (select column_name from INFORMATION_SCHEMA.columns where table_name = 'Order' and column_name = 'CompanyName')
BEGIN
    ALTER TABLE [Order] ADD [CompanyName] nvarchar(256) NULL 
	PRINT 'created column [CompanyName] on table [dbo].[Order]'

	ALTER TABLE [Order] ADD [Instructions] nvarchar(MAX) NULL 
	PRINT 'created column [Instructions] on table [dbo].[Order]'
END
GO
