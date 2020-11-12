CREATE TABLE [dbo].[terminals] (
    [terminal_id]           INT             IDENTITY (1, 1) NOT NULL,
    [terminal_name]         VARCHAR (250)   NOT NULL,
    [terminal_model]        VARCHAR (250)   NULL,
    [device_id]             VARBINARY (MAX) NOT NULL,
    [branch_id]             INT             NOT NULL,
    [terminal_masterkey_id] INT             NOT NULL,
    [workstation]           NVARCHAR (250)  NULL,
    [date_created]          DATETIMEOFFSET        NULL,
    [date_changed]          DATETIMEOFFSET        NULL,
    [password]              VARBINARY (MAX) NULL,
    [IsMacUsed]             BIT             NULL,
    CONSTRAINT [PK_terminals] PRIMARY KEY CLUSTERED ([terminal_id] ASC),
    CONSTRAINT [FK_terminals_branch] FOREIGN KEY ([branch_id]) REFERENCES [dbo].[branch] ([branch_id]),
    CONSTRAINT [FK_terminals_masterkeys] FOREIGN KEY ([terminal_masterkey_id]) REFERENCES [dbo].[masterkeys] ([masterkey_id])
);

