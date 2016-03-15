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
		[UseEach] bit not null default(0),
		[Deleted] bit not null default(0)

) --ON [PRIMARY]
END
ELSE
BEGIN
    PRINT 'Skipped create table [dbo].[MenuItem]'
END
GO

if not exists (select column_name from INFORMATION_SCHEMA.columns where table_name = 'MenuItem' and column_name = 'UseEach')
BEGIN
    ALTER TABLE [MenuItem] ADD [UseEach] bit not null default(0)
	PRINT 'created column [UseEach] on table [dbo].[MenuItem]'
END
GO

if not exists (select column_name from INFORMATION_SCHEMA.columns where table_name = 'MenuItem' and column_name = 'Deleted')
BEGIN
    ALTER TABLE [MenuItem] ADD [Deleted] bit not null default(0)
	PRINT 'created column [Deleted] on table [dbo].[MenuItem]'
END
GO