CREATE TABLE [dbo].[dist_batch] (
    [dist_batch_id]        BIGINT       IDENTITY (1, 1) NOT NULL,
    [branch_id]            INT          NULL,
    [no_cards]             INT          NOT NULL,
    [date_created]         DATETIMEOFFSET     NOT NULL,
    [dist_batch_reference] VARCHAR (50) NOT NULL,
    [card_issue_method_id] INT          NOT NULL,
    [dist_batch_type_id]   INT          NOT NULL,
    [issuer_id]            INT          NOT NULL,
    [dist_version] INT NOT NULL DEFAULT 0, 
    [origin_branch_id] INT NULL, 
    CONSTRAINT [PK_distribution_batch] PRIMARY KEY CLUSTERED ([dist_batch_id] ASC),
    CONSTRAINT [FK_dist_batch_card_issue_method] FOREIGN KEY ([card_issue_method_id]) REFERENCES [dbo].[card_issue_method] ([card_issue_method_id]),
    CONSTRAINT [FK_dist_batch_dist_batch_type] FOREIGN KEY ([dist_batch_type_id]) REFERENCES [dbo].[dist_batch_type] ([dist_batch_type_id]),
    CONSTRAINT [FK_dist_batch_issuer] FOREIGN KEY ([issuer_id]) REFERENCES [dbo].[issuer] ([issuer_id]),
    CONSTRAINT [FK_distribution_batch_branch] FOREIGN KEY ([branch_id]) REFERENCES [dbo].[branch] ([branch_id]),
	CONSTRAINT [FK_distribution_batch_origin_branch] FOREIGN KEY ([origin_branch_id]) REFERENCES [dbo].[branch] ([branch_id])
);

