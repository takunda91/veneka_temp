CREATE TABLE [dbo].[branch_card_status_audit] (
    [branch_card_status_id]   BIGINT         IDENTITY (1, 1) NOT NULL,
    [card_id]                 BIGINT         NOT NULL,
    [branch_card_statuses_id] INT            NOT NULL,
    [status_date]             DATETIMEOFFSET       NOT NULL,
    [user_id]                 BIGINT         NOT NULL,
    [operator_user_id]        BIGINT         NULL,
    [branch_card_code_id]     INT            NULL,
    [comments]                VARCHAR (1000) NULL,
    [pin_auth_user_id]        BIGINT         NULL,
    [branch_id]               INT            NOT NULL,
    CONSTRAINT [PK_branch_card_status_audit] PRIMARY KEY CLUSTERED ([branch_card_status_id] ASC)
);