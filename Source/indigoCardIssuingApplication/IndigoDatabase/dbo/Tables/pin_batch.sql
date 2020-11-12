CREATE TABLE [dbo].[pin_batch] (
    [pin_batch_id]         BIGINT        IDENTITY (1, 1) NOT NULL,
    [no_cards]             INT           NOT NULL,
    [date_created]         DATETIMEOFFSET      NOT NULL,
    [pin_batch_reference]  VARCHAR (100) NOT NULL,
    [pin_batch_type_id]    INT           NOT NULL,
    [card_issue_method_id] INT           NOT NULL,
    [issuer_id]            INT           NOT NULL,
    [branch_id]            INT           NULL,
    CONSTRAINT [PK_pin_batch] PRIMARY KEY CLUSTERED ([pin_batch_id] ASC),
    CONSTRAINT [FK_pin_batch_branch] FOREIGN KEY ([branch_id]) REFERENCES [dbo].[branch] ([branch_id]),
    CONSTRAINT [FK_pin_batch_card_issue_method] FOREIGN KEY ([card_issue_method_id]) REFERENCES [dbo].[card_issue_method] ([card_issue_method_id]),
    CONSTRAINT [FK_pin_batch_issuer] FOREIGN KEY ([issuer_id]) REFERENCES [dbo].[issuer] ([issuer_id]),
    CONSTRAINT [FK_pin_batch_pin_batch_type] FOREIGN KEY ([pin_batch_type_id]) REFERENCES [dbo].[pin_batch_type] ([pin_batch_type_id])
);

