CREATE TABLE [dbo].[load_card_failed] (
    [failed_card_id]       BIGINT       IDENTITY (1, 1) NOT NULL,
    [card_number]          VARCHAR (19) NOT NULL,
    [card_sequence]        NCHAR (13)   NOT NULL,
    [load_batch_reference] VARCHAR (25) NOT NULL,
    [card_status]          VARCHAR (25) NOT NULL,
    [load_batch_id]        BIGINT       NOT NULL,
    CONSTRAINT [PK_load_card_failed] PRIMARY KEY CLUSTERED ([failed_card_id] ASC),
    CONSTRAINT [FK_load_card_failed_load_batch] FOREIGN KEY ([load_batch_id]) REFERENCES [dbo].[load_batch] ([load_batch_id])
);

