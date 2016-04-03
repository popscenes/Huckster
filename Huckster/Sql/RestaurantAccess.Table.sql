-- [[SchemaName:dbo]]
-- [[TableName:RestaurantAccess]]
-- [[FileName:dbo.RestaurantAccess.Table.sql]]

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF (NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  TABLE_NAME = 'RestaurantAccess'))
BEGIN
    CREATE TABLE [dbo].[RestaurantAccess](
        [Id] bigint PRIMARY KEY IDENTITY(1,1),
		[UserId] nvarchar(128) NOT NULL,
		[RestaurantAggrgateRootId] uniqueidentifier NOT NULL

    ) --ON [PRIMARY]
END
ELSE
BEGIN
    PRINT 'Skipped create table [dbo].[RestaurantAccess]'
END
GO