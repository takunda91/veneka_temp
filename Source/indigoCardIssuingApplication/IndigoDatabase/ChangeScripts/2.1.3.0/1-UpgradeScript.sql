ALTER TABLE [dbo].[issuer_product]
    ADD [remote_cms_update_YN] BIT DEFAULT 0 NOT NULL;
GO

CREATE TABLE [dbo].[remote_update_status] (
    [remote_update_status_id]   INT            NOT NULL,
    [remote_update_statuses_id] INT            NOT NULL,
    [card_id]                   BIGINT         NOT NULL,
    [status_date]               DATETIME2 (7)  NOT NULL,
    [comments]                  NVARCHAR (MAX) NOT NULL,
    [remote_component]          VARCHAR (250)  NOT NULL,
    [user_id]                   BIGINT         NOT NULL,
    PRIMARY KEY CLUSTERED ([remote_update_status_id] ASC)
);

GO
CREATE TABLE [dbo].[remote_update_statuses] (
    [remote_update_statuses_id]   INT           NOT NULL,
    [remote_update_statuses_name] VARCHAR (100) NOT NULL,
    PRIMARY KEY CLUSTERED ([remote_update_statuses_id] ASC)
);
GO

CREATE TABLE [dbo].[remote_update_statuses_language] (
    [remote_update_statuses_id] INT            NOT NULL,
    [language_id]               INT            NOT NULL,
    [language_text]             NVARCHAR (100) NOT NULL,
    CONSTRAINT [PK_remote_update_statuses_language] PRIMARY KEY CLUSTERED ([remote_update_statuses_id] ASC, [language_id] ASC)
);
GO

ALTER TABLE [dbo].[remote_update_status] WITH NOCHECK
    ADD CONSTRAINT [FK_remote_update_status_remote_component_statuses] FOREIGN KEY ([remote_update_statuses_id]) REFERENCES [dbo].[remote_update_statuses] ([remote_update_statuses_id]);
GO

ALTER TABLE [dbo].[remote_update_status] WITH NOCHECK
    ADD CONSTRAINT [FK_remote_update_status_cards] FOREIGN KEY ([card_id]) REFERENCES [dbo].[cards] ([card_id]);
GO

ALTER TABLE [dbo].[remote_update_status] WITH NOCHECK
    ADD CONSTRAINT [FK_remote_update_status_user] FOREIGN KEY ([user_id]) REFERENCES [dbo].[user] ([user_id]);
GO

ALTER TABLE [dbo].[remote_update_statuses_language] WITH NOCHECK
    ADD CONSTRAINT [FK_remote_update_statuses_language_languages] FOREIGN KEY ([language_id]) REFERENCES [dbo].[languages] ([id]);
GO

ALTER TABLE [dbo].[remote_update_statuses_language] WITH NOCHECK
    ADD CONSTRAINT [FK_remote_update_statuses_language_remote_update_statuses] FOREIGN KEY ([remote_update_statuses_id]) REFERENCES [dbo].[remote_update_statuses] ([remote_update_statuses_id]);
