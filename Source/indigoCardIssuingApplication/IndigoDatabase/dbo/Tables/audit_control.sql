CREATE TABLE [dbo].[audit_control] (
    [audit_id]            BIGINT        IDENTITY (1, 1) NOT NULL,
    [audit_action_id]     INT           NOT NULL,
    [user_id]             BIGINT        NOT NULL,
    [audit_date]          DATETIMEOFFSET      NOT NULL,
    [workstation_address] VARCHAR (100) NOT NULL,
    [action_description]  VARCHAR (MAX) NULL,
    [issuer_id]           INT           NULL,
    [data_changed]        VARCHAR (30)  NULL,
    [data_before]         VARCHAR (MAX) NULL,
    [data_after]          VARCHAR (MAX) NULL,
    CONSTRAINT [PK_audit_control] PRIMARY KEY CLUSTERED ([audit_id] ASC),
    CONSTRAINT [FK_audit_control_audit_action] FOREIGN KEY ([audit_action_id]) REFERENCES [dbo].[audit_action] ([audit_action_id]),
    CONSTRAINT [FK_audit_control_user] FOREIGN KEY ([user_id]) REFERENCES [dbo].[user] ([user_id])
);

