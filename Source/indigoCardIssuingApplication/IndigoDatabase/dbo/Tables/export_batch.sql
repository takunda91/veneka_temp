CREATE TABLE [dbo].[export_batch] (
    [export_batch_id] BIGINT        IDENTITY (1, 1) NOT NULL,
    [issuer_id]       INT           NOT NULL,
    [batch_reference] VARCHAR (100) NOT NULL,
    [date_created]    DATETIMEOFFSET NOT NULL,
    [no_cards]        INT           NOT NULL,
    CONSTRAINT [PK_export_batch] PRIMARY KEY CLUSTERED ([export_batch_id] ASC),
    CONSTRAINT [FK_export_batch_issuer] FOREIGN KEY ([issuer_id]) REFERENCES [dbo].[issuer] ([issuer_id])
);

