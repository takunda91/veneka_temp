CREATE TABLE [dbo].[load_batch] (
    [load_batch_id]        BIGINT        IDENTITY (1, 1) NOT NULL,
    [file_id]              BIGINT        NOT NULL,
    [load_batch_status_id] INT           NOT NULL,
    [no_cards]             INT           NOT NULL,
    [load_date]            DATETIMEOFFSET      NOT NULL,
    [load_batch_reference] VARCHAR (100) NOT NULL,
    [load_batch_type_id]   INT           DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_load_batch] PRIMARY KEY CLUSTERED ([load_batch_id] ASC),
    CONSTRAINT [FK_load_batch_batch_statuses] FOREIGN KEY ([load_batch_status_id]) REFERENCES [dbo].[load_batch_statuses] ([load_batch_statuses_id]),
    CONSTRAINT [FK_load_batch_file_history] FOREIGN KEY ([file_id]) REFERENCES [dbo].[file_history] ([file_id]),
    CONSTRAINT [FK_load_batch_load_batch] FOREIGN KEY ([load_batch_id]) REFERENCES [dbo].[load_batch] ([load_batch_id]),
    CONSTRAINT [FK_load_batch_type_id] FOREIGN KEY ([load_batch_type_id]) REFERENCES [dbo].[load_batch_types] ([load_batch_type_id])
);

