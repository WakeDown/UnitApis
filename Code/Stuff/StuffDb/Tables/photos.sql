CREATE TABLE [dbo].[photos]
(
	[id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [id_employee] INT NOT NULL, 
    [enabled] BIT NOT NULL DEFAULT 1, 
    [dattim1] DATETIME NOT NULL DEFAULT getdate(), 
    [dattim2] DATETIME NOT NULL DEFAULT '3.3.3333', 
    [path] NVARCHAR(4000) NULL, 
    [picture] VARBINARY(MAX) NULL, 
    [picture_name] NVARCHAR(100) NULL
)

GO

CREATE INDEX [IX_photos_id_employee] ON [dbo].[photos] ([id_employee])
