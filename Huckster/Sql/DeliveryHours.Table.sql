-- [[SchemaName:dbo]]
-- [[TableName:DeliveryHours]]
-- [[FileName:dbo.DeliveryHours.Table.sql]]

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF (NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  TABLE_NAME = 'DeliveryHours'))
BEGIN
    CREATE TABLE [dbo].[DeliveryHours](
        [Id] bigint PRIMARY KEY IDENTITY(1,1),
		[ParentAggregateId] uniqueidentifier not null,
		ServiceType nvarchar(64) not null,
		[DayOfWeek] nvarchar(64) not null,
		OpenTime time not null,
		CloseTime time not null,
		TimeZoneId nvarchar(256)
) --ON [PRIMARY]
END
ELSE
BEGIN
    PRINT 'Skipped create table [dbo].[DeliveryHours]'
END
GO