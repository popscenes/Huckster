--USE [BootLegger]
--GO

INSERT INTO [dbo].[Restaurant]
           ([AggregateRootId]
           ,[Name]
           ,[Description]
           ,[TimeZoneId]
           ,[TileImageUrl])
     VALUES
           ('8DEE7B67-E126-4AFC-82F3-918E038AB3A3'
           ,'Test Restaurant'
           ,'Test Restaurant is an award-winning South East Asian-style dining experience in the heart of Melbourne.'
           ,'AUS Eastern Standard Time'
           ,'http://redspiceroad.com/rsr-mck/assets/images/restaurant/Red-Spice-Road-Bar-3.jpg')
GO


INSERT INTO [dbo].[Address]
           ([ParentAggregateId]
           ,[Number]
           ,[Street]
           ,[Suburb]
           ,[Postcode]
           ,[City]
           ,[State]
           ,[Latitude]
           ,[Longitude])
     VALUES
           ('8DEE7B67-E126-4AFC-82F3-918E038AB3A3'
           ,'27'
           ,'McKillop St'
           ,'Melbourne'
           ,'3000'
           ,'Melbourne'
           ,'VIC'
           ,-37.815187
           ,144.961607)
GO

USE [BootLegger]
GO

INSERT INTO [dbo].[DeliverySuburb]
           ([ParentAggregateId]
           ,[SuburbId]
           ,[postcode]
           ,[suburb]
           ,[state]
           ,[latitude]
           ,[longitude])
     VALUES
           ('8DEE7B67-E126-4AFC-82F3-918E038AB3A3'
           ,5959
           ,'3162'
           ,'Caulfield South'
           ,'VIC'
           ,-37.880
           ,145.030)
GO

					
USE [BootLegger]
GO

INSERT INTO [dbo].[Menu]
           ([ParentAggregateId]
           ,[Title]
           ,[Description]
           ,[Order])
     VALUES
           ('8DEE7B67-E126-4AFC-82F3-918E038AB3A3'
           ,'Lunch a la Carte'
           ,null
           ,0)
GO

USE [BootLegger]
GO

INSERT INTO [dbo].[MenuItem]
           ([MenuId]
           ,[Name]
           ,[MenuGroup]
           ,[Description]
           ,[Price]
           ,[Order])
     VALUES (0,'Duck relish, watermelon','Small Bites',null,6.0,0),
      (0,'Betel leaf, smashed prawn, coconut, chilli, lime leaf','Small Bites',null,7.0,1),
     (0,'Local squid, malaysian sambal, pickled ginger','Small Bites',null,19.0,2),
     (0,'Sticky chicken rib, cumin, fennel, coriander','Small Bites',null,3.0,3),
     (0,'Citrus-cured snapper, pomelo, coconut, crispy wontons','Small Bites',null,21.0,4),
     (0,'Twice cooked crispy lamb ribs, chilli jam','Small Bites',null,19.0,5),
     (0,'Sweet corn & green onion fritters, iceberg, herb salad sweet chilli','Small Bites',null,18.0,5),

     (0,'Fried barramundi salad, watermelon, mint, shallots, red nam jim','Large Plates',null,36.0,6),
     (0,'Pickled tea leaf salad, sesame, peanuts, tomato, wombokm','Large Plates',null,34.0,7),
     (0,'Eggplant ma po, tofu, Szechuan, spring onion','Large Plates',null,30.0,8),
     (0,'Local fried sea bream, yellow curry, capsicum, gailan','Large Plates',null,36.0,9),
     (0,'Braised ox cheek, Southern Thai curry, sweet potato, carrot','Large Plates',null,36.0,10),
     (0,'Pork belly, apple slaw, chilli caramel, black vinegar','Large Plates',null,37.0,11),
     (0,'Sri lankan beetroot curry','Large Plates',null,34.0,12),
     (0,'Steamed Jasmine Rice','Large Plates',null,2.0,13)



GO

USE [BootLegger]
GO

INSERT INTO [dbo].[DeliveryHours]
           ([ParentAggregateId]
           ,[ServiceType]
           ,[DayOfWeek]
           ,[OpenTime]
           ,[CloseTime]
           ,[TimeZoneId])
     VALUES
           ('8DEE7B67-E126-4AFC-82F3-918E038AB3A3'
           ,'Dinner'
           ,'Monday'
           ,'18:30'
           ,'19:00'
           ,'AUS Eastern Standard Time'),
		   ('8DEE7B67-E126-4AFC-82F3-918E038AB3A3'
           ,'Dinner'
           ,'Monday'
           ,'19:00'
           ,'19:30'
           ,'AUS Eastern Standard Time'),
		   ('8DEE7B67-E126-4AFC-82F3-918E038AB3A3'
           ,'Dinner'
           ,'Monday'
           ,'19:30'
           ,'20:00'
           ,'AUS Eastern Standard Time'),
		   ('8DEE7B67-E126-4AFC-82F3-918E038AB3A3'
           ,'Dinner'
           ,'Monday'
           ,'20:00'
           ,'20:30'
           ,'AUS Eastern Standard Time'),
		   ('8DEE7B67-E126-4AFC-82F3-918E038AB3A3'
           ,'Dinner'
           ,'Monday'
           ,'20:30'
           ,'21:00'
           ,'AUS Eastern Standard Time')

GO



