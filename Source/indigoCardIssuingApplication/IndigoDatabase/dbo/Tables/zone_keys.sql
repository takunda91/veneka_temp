CREATE TABLE [dbo].[zone_keys] (
    [issuer_id] INT             NOT NULL,
    [zone]      VARBINARY (MAX) NOT NULL,
    [final]     VARBINARY (MAX) NOT NULL,
    CONSTRAINT [PK_zone_keys] PRIMARY KEY CLUSTERED ([issuer_id] ASC)
);

