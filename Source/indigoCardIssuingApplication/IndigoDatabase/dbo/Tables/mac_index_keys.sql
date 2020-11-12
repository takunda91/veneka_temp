CREATE TABLE [dbo].[mac_index_keys] (
    [table_id] INT             NOT NULL,
    [mac_key]  VARBINARY (MAX) NOT NULL,
    CONSTRAINT [PK_mac_index_keys] PRIMARY KEY CLUSTERED ([table_id] ASC)
);

