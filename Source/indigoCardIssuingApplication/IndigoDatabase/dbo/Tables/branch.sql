CREATE TABLE [dbo].[branch] (
    [branch_id]             INT           IDENTITY (1, 1) NOT NULL,
    [branch_status_id]      INT           NOT NULL,
    [issuer_id]             INT           NOT NULL,
    [branch_code]           VARCHAR (10)  NOT NULL,
    [branch_name]           VARCHAR (30)  NOT NULL,
    [location]              VARCHAR (20)  NULL,
    [contact_person]        VARCHAR (30)  NULL,
    [contact_email]         VARCHAR (30)  NULL,
    [card_centre]           VARCHAR (10)  NULL,
    [emp_branch_code]       NVARCHAR (10) NULL,	
	[branch_type_id] [int] NULL,
	[main_branch_id] [int] NULL,
    CONSTRAINT [PK_branch] PRIMARY KEY CLUSTERED ([branch_id] ASC),
    CONSTRAINT [FK_branch_branch_statuses] FOREIGN KEY ([branch_status_id]) REFERENCES [dbo].[branch_statuses] ([branch_status_id]),
    CONSTRAINT [FK_branch_issuer] FOREIGN KEY ([issuer_id]) REFERENCES [dbo].[issuer] ([issuer_id]), 
    CONSTRAINT [FK_branch_branch_type] FOREIGN KEY ([branch_type_id]) REFERENCES [branch_type]([branch_type_id])
);

