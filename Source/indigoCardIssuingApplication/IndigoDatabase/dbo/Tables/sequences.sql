CREATE TABLE [dbo].[sequences] (
    [sequence_name]        VARCHAR (100) NOT NULL,
    [last_sequence_number] BIGINT        NOT NULL,
    [last_updated]          DATETIMEOFFSET (7)      NOT NULL,
    CONSTRAINT [PK_sequences] PRIMARY KEY CLUSTERED ([sequence_name] ASC)
);

