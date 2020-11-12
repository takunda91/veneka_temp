CREATE TABLE [dbo].[file_history] (
    [file_id]                   BIGINT        IDENTITY (1, 1) NOT NULL,
    [issuer_id]                 INT           NULL,
    [file_status_id]            INT           NOT NULL,
    [file_type_id]              INT           NOT NULL,
    [name_of_file]              VARCHAR (60)  NOT NULL,
    [file_created_date]         DATETIMEOFFSET      NOT NULL,
    [file_size]                 INT           NOT NULL,
    [load_date]                 DATETIMEOFFSET      NOT NULL,
    [file_directory]            VARCHAR (110) NOT NULL,
    [number_successful_records] INT           NULL,
    [number_failed_records]     INT           NULL,
    [file_load_comments]        VARCHAR (MAX) NULL,
    [file_load_id]              INT           NOT NULL,
    CONSTRAINT [PK_file_history] PRIMARY KEY CLUSTERED ([file_id] ASC),
    CONSTRAINT [FK_file_history_file_load] FOREIGN KEY ([file_load_id]) REFERENCES [dbo].[file_load] ([file_load_id]),
    CONSTRAINT [FK_file_history_file_history] FOREIGN KEY ([file_type_id]) REFERENCES [dbo].[file_types] ([file_type_id]),
    CONSTRAINT [FK_file_history_file_statuses] FOREIGN KEY ([file_status_id]) REFERENCES [dbo].[file_statuses] ([file_status_id]),
    CONSTRAINT [FK_file_history_issuer] FOREIGN KEY ([issuer_id]) REFERENCES [dbo].[issuer] ([issuer_id])
);

