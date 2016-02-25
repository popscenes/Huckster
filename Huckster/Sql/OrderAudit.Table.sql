-- [[SchemaName:dbo]]
-- [[TableName:Customer]]
-- [[FileName:dbo.Customer.Table.sql]]

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF (NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  TABLE_NAME = 'OrderAudit'))
BEGIN
CREATE TABLE [dbo].[OrderAudit](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ParentAggregateId] [uniqueidentifier] NOT NULL,
	[Action] nvarchar(MAX) NOT NULL,
	[CreatedDateTime] [datetime2](7) NOT NULL,
	[UserName] [nvarchar](64) NOT NULL

) --ON [PRIMARY]
END
GO