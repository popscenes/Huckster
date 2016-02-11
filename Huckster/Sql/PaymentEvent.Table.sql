-- [[SchemaName:dbo]]
-- [[TableName:PaymentEvent]]
-- [[FileName:dbo.PaymentEvent.Table.sql]]

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF (NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  TABLE_NAME = 'PaymentEvent'))
BEGIN
    CREATE TABLE [dbo].[PaymentEvent](
		[Id] bigint PRIMARY KEY IDENTITY(1,1),
        [ParentAggregateId] uniqueidentifier NOT NULL,
		[ExternalId] nvarchar(256) NOT NULL,
		[Gateway] nvarchar(256) NOT NULL,
		[Status] nvarchar(256) NOT NULL,
		[Type] nvarchar(256) NOT NULL,
		[TransactionSuccess] bit NOT NULL,
		[PaymentDateTime] datetime2 NOT NULL,
		[ExtraInfo] nvarchar(max) NULL
) --ON [PRIMARY]
END
ELSE
BEGIN
    PRINT 'Skipped create table [dbo].[PaymentEvent]'
END
GO