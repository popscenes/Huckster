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
	[Instructions] nvarchar(MAX) NULL,
	[PickUpTime] [datetime2](7) NULL,
	[DeliveryUserId] nvarchar(128) NULL,
	[DeliveryFee] DECIMAL(8,2) NOT NULL DEFAULT(0.0)

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

if not exists (select column_name from INFORMATION_SCHEMA.columns where table_name = 'Order' and column_name = 'PickUpTime')
BEGIN
    ALTER TABLE [Order] ADD [PickUpTime] [datetime2](7) NULL 
	PRINT 'created column [PickUpTime] on table [dbo].[Order]'

	ALTER TABLE [Order] ADD [DeliveryUserId] nvarchar(128) NULL
	PRINT 'created column [DeliveryUserId] on table [dbo].[Order]'
END


if not exists (select column_name from INFORMATION_SCHEMA.columns where table_name = 'Order' and column_name = 'Surge')
BEGIN
	ALTER TABLE [Order] ADD [SurgePct] int NOT NULL DEFAULT(0)
	PRINT 'created column [SurgePct] on table [dbo].[Order]'
END
GO
if not exists (select column_name from INFORMATION_SCHEMA.columns where table_name = 'Order' and column_name = 'Surge')
BEGIN
	ALTER TABLE [Order] ADD [DeliveryFee] DECIMAL(8,2) NOT NULL DEFAULT(0.0)
	PRINT 'created column [DeliveryFee] on table [dbo].[Order]'
END
GO