CREATE TABLE [dbo].[customer_account] (
    [customer_account_id]     BIGINT          IDENTITY (1, 1) NOT NULL,
    [user_id]                 BIGINT          NOT NULL,
    [card_issue_reason_id]    INT             NOT NULL,
    [account_type_id]         INT             NOT NULL,
    [currency_id]             INT             NOT NULL,
    [resident_id]             INT             NOT NULL,
    [customer_type_id]        INT             NOT NULL,
    [customer_account_number] VARBINARY (MAX) NOT NULL,
    [customer_first_name]     VARBINARY (MAX) NOT NULL,
    [customer_middle_name]    VARBINARY (MAX) NOT NULL,
    [customer_last_name]      VARBINARY (MAX) NOT NULL,
    [name_on_card]            VARBINARY (MAX) NOT NULL,
    [date_issued]             DATETIMEOFFSET        NOT NULL,
    [cms_id]                  VARCHAR (50)    NULL,
    [contract_number]         VARCHAR (50)    NULL,
    [customer_title_id]       INT             NULL,
    [Id_number]               VARBINARY (MAX) NULL,
    [contact_number]          VARBINARY (MAX) NULL,
    [CustomerId]              VARBINARY (MAX) NULL,
    [domicile_branch_id]      INT             NOT NULL,
	cbs_account_type            VARCHAR(50)
    
    CONSTRAINT [PK_customer_account] PRIMARY KEY CLUSTERED ([customer_account_id] ASC),
    CONSTRAINT [FK_customer_customer_title] FOREIGN KEY ([customer_title_id]) REFERENCES [dbo].[customer_title] ([customer_title_id]),
    CONSTRAINT [FK_customer_account_branch] FOREIGN KEY ([domicile_branch_id]) REFERENCES [dbo].[branch] ([branch_id]),
    CONSTRAINT [FK_customer_account_card_issue_reason] FOREIGN KEY ([card_issue_reason_id]) REFERENCES [dbo].[card_issue_reason] ([card_issue_reason_id]),
    CONSTRAINT [FK_customer_account_type] FOREIGN KEY ([account_type_id]) REFERENCES [dbo].[customer_account_type] ([account_type_id]),
    CONSTRAINT [FK_customer_account_user] FOREIGN KEY ([user_id]) REFERENCES [dbo].[user] ([user_id]),
    CONSTRAINT [FK_customer_residency] FOREIGN KEY ([resident_id]) REFERENCES [dbo].[customer_residency] ([resident_id]),
    CONSTRAINT [FK_customer_type] FOREIGN KEY ([customer_type_id]) REFERENCES [dbo].[customer_type] ([customer_type_id]),
   
);


GO
ALTER TABLE [dbo].[customer_account] NOCHECK CONSTRAINT [FK_customer_account_branch];


GO
ALTER TABLE [dbo].[customer_account] NOCHECK CONSTRAINT [FK_customer_account_card_issue_reason];


GO
ALTER TABLE [dbo].[customer_account] NOCHECK CONSTRAINT [FK_customer_account_type];

GO

ALTER TABLE [dbo].[customer_account] NOCHECK CONSTRAINT [FK_customer_account_user];


GO
ALTER TABLE [dbo].[customer_account] NOCHECK CONSTRAINT [FK_customer_residency];


GO
ALTER TABLE [dbo].[customer_account] NOCHECK CONSTRAINT [FK_customer_type];

