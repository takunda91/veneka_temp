CREATE TABLE [dbo].[masterkeys] (
    [masterkey_id]   INT             IDENTITY (1, 1) NOT NULL,
    [masterkey_name] VARCHAR (250)   NOT NULL,
    [masterkey]      VARBINARY (MAX) NOT NULL,
    [issuer_id]      INT             NOT NULL,
    [date_created]   DATETIMEOFFSET(7)     NULL,
    [date_changed]   DATETIMEOFFSET(7)        NULL,
    CONSTRAINT [PK_masterkeys] PRIMARY KEY CLUSTERED ([masterkey_id] ASC),
    CONSTRAINT [FK_masterkeys_issuer] FOREIGN KEY ([issuer_id]) REFERENCES [dbo].[issuer] ([issuer_id])
);

