CREATE TABLE [dbo].[organizations]
(
	[id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [name] NVARCHAR(50) NOT NULL,      
    [sys_name] NVARCHAR(50) NOT NULL, 
	[enaled] BIT NOT NULL DEFAULT 1,
    [dattim1] DATETIME NOT NULL DEFAULT getdate(), 
    [dattim2] DATETIME NOT NULL DEFAULT '3.3.3333'
)
