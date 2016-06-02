CREATE TABLE [dbo].[Article]
(
	[ID] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
	[VisitDate] DATETIME NOT NULL, 
    [CreateUser] UNIQUEIDENTIFIER NOT NULL, 
	[Hospital] NVARCHAR(50) NOT NULL, 
	[VisitTarget] NVARCHAR(50),
	[CallNotes]	NVARCHAR(MAX) ,
	[VisitFee]  INT ,
	[TransportFee] INT ,
	[Misc]	NVARCHAR(MAX) ,
	
    [PublishDate] DATETIME,
    [ViewCount] INT,
    [CreateDate] DATETIME, 
    [UpdateUser] UNIQUEIDENTIFIER NULL, 
    [UpdateDate] DATETIME
)

