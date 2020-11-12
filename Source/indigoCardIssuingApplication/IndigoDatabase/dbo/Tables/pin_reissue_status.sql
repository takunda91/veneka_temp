CREATE TABLE [dbo].[pin_reissue_status] (    
    [pin_reissue_id]          BIGINT         NOT NULL,
    [pin_reissue_statuses_id] INT            NOT NULL,
    [status_date]             DATETIMEOFFSET  NOT NULL,
    [user_id]                 BIGINT         NOT NULL,
    [audit_workstation]       VARCHAR (100)  NOT NULL,
    [comments]                VARCHAR (1000) NULL,
    CONSTRAINT [PK_pin_reissue_status] PRIMARY KEY CLUSTERED ([pin_reissue_id] ASC),
    CONSTRAINT [FK_pin_reissue_status_pin_reissue] FOREIGN KEY ([pin_reissue_id]) REFERENCES [dbo].[pin_reissue] ([pin_reissue_id]),
    CONSTRAINT [FK_pin_reissue_status_pin_reissue_statuses] FOREIGN KEY ([pin_reissue_statuses_id]) REFERENCES [dbo].[pin_reissue_statuses] ([pin_reissue_statuses_id])
);

