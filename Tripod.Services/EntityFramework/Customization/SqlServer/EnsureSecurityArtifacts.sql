-- create unique key for Security.User.Name
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[Security].[User]') AND name = N'UK_Security.User.Name')
ALTER TABLE [Security].[User] ADD  CONSTRAINT [UK_Security.User.Name] UNIQUE NONCLUSTERED
(
	[Name] ASC
)
GO

---- create unique key for Security.Permission.Name
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[Security].[Permission]') AND name = N'UK_Security.Permission.Name')
ALTER TABLE [Security].[Permission] ADD  CONSTRAINT [UK_Security.Permission.Name] UNIQUE NONCLUSTERED
(
	[Name] ASC
)
GO

---- create unique key for Security.EmailAddress.Value
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[Security].[EmailAddress]') AND name = N'UK_Security.EmailAddress.Value')
ALTER TABLE [Security].[EmailAddress] ADD  CONSTRAINT [UK_Security.EmailAddress.Value] UNIQUE NONCLUSTERED
(
	[Value] ASC
)
GO

-- collate case sensitively for Security.EmailVerification.Ticket
ALTER TABLE [Security].[EmailVerification]
ALTER COLUMN [Ticket] NVARCHAR(100)
COLLATE Latin1_General_CS_AS
GO

---- create unique key for Security.EmailVerification.Ticket
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[Security].[EmailVerification]') AND name = N'UK_Security.EmailVerification.Ticket')
ALTER TABLE [Security].[EmailVerification] ADD  CONSTRAINT [UK_Security.EmailVerification.Ticket] UNIQUE NONCLUSTERED
(
	[Ticket] ASC
)
GO
