EXEC sp_rename 'branch_card_status', 'branch_card_status_audit'
GO
EXEC sp_rename 'PK_branch_card_status', 'PK_branch_card_status_audit'
GO

--DROP contraints
ALTER TABLE branch_card_status_audit DROP CONSTRAINT FK_branch_card_status_branch_card_codes;
ALTER TABLE branch_card_status_audit DROP CONSTRAINT FK_branch_card_status_branch_card_statuses;
ALTER TABLE branch_card_status_audit DROP CONSTRAINT FK_branch_card_status_branch_id;
ALTER TABLE branch_card_status_audit DROP CONSTRAINT FK_branch_card_status_cards;
ALTER TABLE branch_card_status_audit DROP CONSTRAINT FK_branch_card_status_user;
ALTER TABLE branch_card_status_audit DROP CONSTRAINT FK_branch_card_status_user1;
GO

--DROP indexes
DROP INDEX [INDEX_CARD_OPERATOR] ON [dbo].[branch_card_status_audit];
GO
DROP INDEX [INDEX_CARD_STATUS_DATE] ON [dbo].[branch_card_status_audit];

--Create new current table
GO
PRINT N'Creating [dbo].[branch_card_status]...';


GO
CREATE TABLE [dbo].[branch_card_status] (
    [card_id]                 BIGINT             NOT NULL,
    [branch_card_statuses_id] INT                NOT NULL,
    [status_date]             DATETIMEOFFSET (7) NOT NULL,
    [user_id]                 BIGINT             NOT NULL,
    [operator_user_id]        BIGINT             NULL,
    [branch_card_code_id]     INT                NULL,
    [comments]                VARCHAR (1000)     NULL,
    [pin_auth_user_id]        BIGINT             NULL,
    [branch_id]               INT                NOT NULL,
    CONSTRAINT [PK_branch_card_status] PRIMARY KEY CLUSTERED ([card_id] ASC)
);


GO
PRINT N'Creating [dbo].[branch_card_status].[INDEX_CARD_STATUS_DATE]...';


GO
CREATE NONCLUSTERED INDEX [INDEX_CARD_STATUS_DATE]
    ON [dbo].[branch_card_status]([card_id] ASC)
    INCLUDE([status_date]);


GO
PRINT N'Creating [dbo].[branch_card_status].[INDEX_CARD_OPERATOR]...';


GO
CREATE NONCLUSTERED INDEX [INDEX_CARD_OPERATOR]
    ON [dbo].[branch_card_status]([branch_card_statuses_id] ASC, [status_date] ASC)
    INCLUDE([card_id], [operator_user_id]);


GO
PRINT N'Creating [dbo].[FK_branch_card_status_branch_card_codes]...';


GO
ALTER TABLE [dbo].[branch_card_status] WITH NOCHECK
    ADD CONSTRAINT [FK_branch_card_status_branch_card_codes] FOREIGN KEY ([branch_card_code_id]) REFERENCES [dbo].[branch_card_codes] ([branch_card_code_id]);


GO
PRINT N'Creating [dbo].[FK_branch_card_status_branch_card_statuses]...';


GO
ALTER TABLE [dbo].[branch_card_status] WITH NOCHECK
    ADD CONSTRAINT [FK_branch_card_status_branch_card_statuses] FOREIGN KEY ([branch_card_statuses_id]) REFERENCES [dbo].[branch_card_statuses] ([branch_card_statuses_id]);


GO
PRINT N'Creating [dbo].[FK_branch_card_status_branch_id]...';


GO
ALTER TABLE [dbo].[branch_card_status] WITH NOCHECK
    ADD CONSTRAINT [FK_branch_card_status_branch_id] FOREIGN KEY ([branch_id]) REFERENCES [dbo].[branch] ([branch_id]);


GO
PRINT N'Creating [dbo].[FK_branch_card_status_cards]...';


GO
ALTER TABLE [dbo].[branch_card_status] WITH NOCHECK
    ADD CONSTRAINT [FK_branch_card_status_cards] FOREIGN KEY ([card_id]) REFERENCES [dbo].[cards] ([card_id]);


GO
PRINT N'Creating [dbo].[FK_branch_card_status_user]...';


GO
ALTER TABLE [dbo].[branch_card_status] WITH NOCHECK
    ADD CONSTRAINT [FK_branch_card_status_user] FOREIGN KEY ([operator_user_id]) REFERENCES [dbo].[user] ([user_id]);


GO
PRINT N'Creating [dbo].[FK_branch_card_status_user1]...';


GO
ALTER TABLE [dbo].[branch_card_status] WITH NOCHECK
    ADD CONSTRAINT [FK_branch_card_status_user1] FOREIGN KEY ([user_id]) REFERENCES [dbo].[user] ([user_id]);
GO

--Extract current status of card from audit to current table
INSERT INTO dbo.branch_card_status (card_id, branch_card_statuses_id, status_date, [user_id], operator_user_id, branch_card_code_id, comments, pin_auth_user_id, branch_id)
SELECT        
dbo.branch_card_status_audit.card_id, 
dbo.branch_card_status_audit.branch_card_statuses_id, 
dbo.branch_card_status_audit.status_date, 
dbo.branch_card_status_audit.[user_id], 
dbo.branch_card_status_audit.operator_user_id, 
dbo.branch_card_status_audit.branch_card_code_id, 
dbo.branch_card_status_audit.comments,
dbo.branch_card_status_audit.pin_auth_user_id,
dbo.branch_card_status_audit.branch_id
FROM	   dbo.branch_card_status_audit 
WHERE        (dbo.branch_card_status_audit.status_date =
                             (SELECT        MAX(status_date) AS Expr1
                               FROM            dbo.branch_card_status_audit AS bcs2
                               WHERE        (card_id = dbo.branch_card_status_audit.card_id)))

-- Delete current status from audit - THIS MUST ONLY BE RUN ONCE
DELETE FROM dbo.branch_card_status_audit
WHERE branch_card_status_id IN (
SELECT        
dbo.branch_card_status_audit.branch_card_status_id
FROM	   dbo.branch_card_status_audit 
WHERE        (dbo.branch_card_status_audit.status_date =
                             (SELECT        MAX(status_date) AS Expr1
                               FROM            dbo.branch_card_status_audit AS bcs2
                               WHERE        (card_id = dbo.branch_card_status_audit.card_id))))



