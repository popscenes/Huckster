-- [[SchemaName:dbo]]
-- [[TableName:Enquiry]]
-- [[FileName:dbo.Enquiry.Table.sql]]

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF (NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  TABLE_NAME = 'Enquiry'))
BEGIN
    CREATE TABLE [dbo].[Enquiry](
        [Id] bigint PRIMARY KEY IDENTITY(1,1),
		[Name] nvarchar(256) not null,
		[Phone] nvarchar(256) not null,
		[Email] nvarchar(256) not null,
		[Subject]  nvarchar(256) not null,
		[Message]  nvarchar(MAX) not null,
    ) --ON [PRIMARY]
END
ELSE
BEGIN
    PRINT 'Skipped create table [dbo].[Enquiry]'
END
GO