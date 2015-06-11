CREATE TABLE [dbo].[employees]
(
	[id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [ad_sid] VARCHAR(36) NOT NULL DEFAULT '', 
    [id_manager] INT NOT NULL, 
    [surname] NVARCHAR(50) NOT NULL, 
    [name] NVARCHAR(50) NOT NULL, 
    [patronymic] NVARCHAR(50) NULL, 
    [full_name] NVARCHAR(150) NOT NULL, 
    [display_name] NVARCHAR(150) NOT NULL, 
    [id_position] INT NOT NULL, 
    [id_organization] INT NOT NULL, 
    [email] NVARCHAR(150) NOT NULL, 
    [work_num] NVARCHAR(50) NOT NULL, 
    [mobil_num] NVARCHAR(50) NOT NULL, 
    [id_emp_state] SMALLINT NOT NULL,
	[id_department] INT NOT NULL, 
    [id_city] INT NOT NULL , 
    [enabled] BIT NOT NULL DEFAULT 1, 
    [dattim1] DATETIME NOT NULL DEFAULT getdate(), 
    [dattim2] DATETIME NOT NULL DEFAULT '3.3.3333', 
    [date_came] DATE NULL     
)

GO

CREATE INDEX [IX_employee_id_department] ON [dbo].[employees] ([id_department])

GO

CREATE INDEX [IX_employee_id_manager] ON [dbo].[employees] ([id_manager])

GO

CREATE INDEX [IX_employee_ad_sid] ON [dbo].[employees] ([ad_sid])

GO

CREATE INDEX [IX_employee_id_emp_state] ON [dbo].[employees] ([id_emp_state])

GO

CREATE INDEX [IX_employee_enabled] ON [dbo].[employees] ([enabled] DESC)
