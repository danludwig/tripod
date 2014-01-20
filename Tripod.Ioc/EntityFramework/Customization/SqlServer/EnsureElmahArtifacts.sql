-- the elmah table will be created by entity framework

-- remove the clustered pk index and replace with nonclustered pk index
IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[Audit].[ExceptionAudit]') AND name = N'PK_Audit.ExceptionAudit' AND type_desc = N'CLUSTERED')
BEGIN
	ALTER TABLE [Audit].[ExceptionAudit] DROP CONSTRAINT [PK_Audit.ExceptionAudit]
	ALTER TABLE [Audit].[ExceptionAudit] WITH NOCHECK ADD
		  CONSTRAINT [PK_Audit.ExceptionAudit] PRIMARY KEY NONCLUSTERED ([ErrorId])
END
GO

-- add an index on application, time, and sequence
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[Audit].[ExceptionAudit]') AND name = N'IX_Audit.ExceptionAudit.App.Time.Seq')
BEGIN
	CREATE NONCLUSTERED INDEX [IX_Audit.ExceptionAudit.App.Time.Seq] ON [Audit].[ExceptionAudit]
	(
		[Application] ASC,
		[TimeUtc] DESC,
		[Sequence] DESC
	)
	END
GO

-- add a clustered index on the sequence identity column
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[Audit].[ExceptionAudit]') AND name = N'UK_Audit.ExceptionAudit.Sequence')
	ALTER TABLE [Audit].[ExceptionAudit] ADD  CONSTRAINT [UK_Audit.ExceptionAudit.Sequence] UNIQUE CLUSTERED
	(
		[Sequence] ASC
	)

GO

-- create 3 stored procedures used internally by elmah
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Audit].[GetExceptionXml]') AND type in (N'P', N'PC'))
	exec('CREATE PROCEDURE [Audit].[GetExceptionXml] AS BEGIN SET NOCOUNT ON; END')
GO
ALTER PROCEDURE [Audit].[GetExceptionXml]
(
    @Application NVARCHAR(60),
    @ErrorId UNIQUEIDENTIFIER
)
AS
    SET NOCOUNT ON

    SELECT
        [AllXml]
    FROM
        [Audit].[ExceptionAudit]
    WHERE
        [ErrorId] = @ErrorId
    AND
        [Application] = @Application
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Audit].[GetExceptionsXml]') AND type in (N'P', N'PC'))
    exec('CREATE PROCEDURE [Audit].[GetExceptionsXml] AS BEGIN SET NOCOUNT ON; END')
GO
ALTER PROCEDURE [Audit].[GetExceptionsXml]
(
    @Application NVARCHAR(60),
    @PageIndex INT = 0,
    @PageSize INT = 15,
    @TotalCount INT OUTPUT
)
AS

    SET NOCOUNT ON

    DECLARE @FirstTimeUTC DATETIME
    DECLARE @FirstSequence INT
    DECLARE @StartRow INT
    DECLARE @StartRowIndex INT

    SELECT
        @TotalCount = COUNT(1)
    FROM
        [Audit].[ExceptionAudit]
    WHERE
        [Application] = @Application

    -- Get the ID of the first error for the requested page

    SET @StartRowIndex = @PageIndex * @PageSize + 1

    IF @StartRowIndex <= @TotalCount
    BEGIN

        SET ROWCOUNT @StartRowIndex

        SELECT
            @FirstTimeUTC = [TimeUtc],
            @FirstSequence = [Sequence]
        FROM
            [Audit].[ExceptionAudit]
        WHERE
            [Application] = @Application
        ORDER BY
            [TimeUtc] DESC,
            [Sequence] DESC

    END
    ELSE
    BEGIN

        SET @PageSize = 0

    END

    -- Now set the row count to the requested page size and get
    -- all records below it for the pertaining application.

    SET ROWCOUNT @PageSize

    SELECT
        errorId     = [ErrorId],
        application = [Application],
        host        = [Host],
        type        = [Type],
        source      = [Source],
        message     = [Message],
        [user]      = [User],
        statusCode  = [StatusCode],
        time        = CONVERT(VARCHAR(50), [TimeUtc], 126) + 'Z'
    FROM
        [Audit].[ExceptionAudit] error
    WHERE
        [Application] = @Application
    AND
        [TimeUtc] <= @FirstTimeUTC
    AND
        [Sequence] <= @FirstSequence
    ORDER BY
        [TimeUtc] DESC,
        [Sequence] DESC
    FOR
        XML AUTO
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Audit].[AuditException]') AND type in (N'P', N'PC'))
	exec('CREATE PROCEDURE [Audit].[AuditException] AS BEGIN SET NOCOUNT ON; END')
GO
ALTER PROCEDURE [Audit].[AuditException]
(
    @ErrorId UNIQUEIDENTIFIER,
    @Application NVARCHAR(60),
    @Host NVARCHAR(30),
    @Type NVARCHAR(100),
    @Source NVARCHAR(60),
    @Message NVARCHAR(500),
    @User NVARCHAR(50),
    @AllXml NTEXT,
    @StatusCode INT,
    @TimeUtc DATETIME
)
AS

    SET NOCOUNT ON

    INSERT
    INTO
        [Audit].[ExceptionAudit]
        (
            [ErrorId],
            [Application],
            [Host],
            [Type],
            [Source],
            [Message],
            [User],
            [AllXml],
            [StatusCode],
            [TimeUtc]
        )
    VALUES
        (
            @ErrorId,
            @Application,
            @Host,
            @Type,
            @Source,
            @Message,
            @User,
            @AllXml,
            @StatusCode,
            @TimeUtc
        )
GO

-- duplicate Audit.ExceptionAudit at dbo.ELMAH_Error (for ELMAH)
IF EXISTS (SELECT * FROM sys.synonyms WHERE name = N'ELMAH_Error')
	DROP SYNONYM [dbo].[ELMAH_Error]
CREATE SYNONYM [dbo].[ELMAH_Error] FOR [Audit].[ExceptionAudit]
GO

-- duplicate Audit.GetExceptionXml at dbo.ELMAH_GetErrorXml (for ELMAH)
IF EXISTS (SELECT * FROM sys.synonyms WHERE name = N'ELMAH_GetErrorXml')
	DROP SYNONYM [dbo].[ELMAH_GetErrorXml]
CREATE SYNONYM [dbo].[ELMAH_GetErrorXml] FOR [Audit].[GetExceptionXml]
GO

-- duplicate Audit.GetExceptionsXml at dbo.ELMAH_GetErrorsXml (for ELMAH)
IF EXISTS (SELECT * FROM sys.synonyms WHERE name = N'ELMAH_GetErrorsXml')
	DROP SYNONYM [dbo].[ELMAH_GetErrorsXml]
CREATE SYNONYM [dbo].[ELMAH_GetErrorsXml] FOR [Audit].[GetExceptionsXml]
GO

-- duplicate Audit.AuditException at dbo.ELMAH_LogError (for ELMAH)
IF EXISTS (SELECT * FROM sys.synonyms WHERE name = N'ELMAH_LogError')
	DROP SYNONYM [dbo].[ELMAH_LogError]
CREATE SYNONYM [dbo].[ELMAH_LogError] FOR [Audit].[AuditException]
GO

