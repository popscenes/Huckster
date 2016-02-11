-- [[SchemaName:dbo]]
-- [[TableName:MenuItem]]
-- [[FileName:dbo.MenuItem.Table.sql]]

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF (NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  TABLE_NAME = 'MenuItem'))
BEGIN
    CREATE TABLE [dbo].[MenuItem](
        [Id] bigint PRIMARY KEY IDENTITY(1,1),
		[MenuId] int not null,
		[Name] nvarchar(256) not null,
		[MenuGroup] nvarchar (256) null,
		[Description] nvarchar(max) null,
		[Price]  decimal not null,
		[Order] int null,

) --ON [PRIMARY]
END
ELSE
BEGIN
    PRINT 'Skipped create table [dbo].[MenuItem]'
END
GO