CREATE TABLE [dbo].[pin_reissue_status_audit] (
    [pin_reissue_status_id]   BIGINT         IDENTITY (1, 1) NOT NULL,
    [pin_reissue_id]          BIGINT         NOT NULL,
    [pin_reissue_statuses_id] INT            NOT NULL,
    [status_date]             DATETIMEOFFSET  NOT NULL,
    [user_id]                 BIGINT         NOT NULL,
    [audit_workstation]       VARCHAR (100)  NOT NULL,
    [comments]                VARCHAR (1000) NULL,
    CONSTRAINT [PK_pin_reissue_status_audit] PRIMARY KEY CLUSTERED ([pin_reissue_status_id] ASC)
);

