CREATE TABLE [dbo].[departments]
(
	[id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [name] NVARCHAR(150) NOT NULL, 
    [id_parent] INT NULL, 
    [enabled] BIT NOT NULL DEFAULT 1, 
    [dattim1] DATETIME NOT NULL DEFAULT getdate(), 
    [dattim2] DATETIME NOT NULL DEFAULT '3.3.3333', 
    [id_chief] INT NULL
)

GO

CREATE INDEX [IX_departments_id_parent] ON [dbo].[departments] ([id_parent])
