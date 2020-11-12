CREATE TABLE [dbo].[external_systems] (
    [external_system_id]      INT           IDENTITY (1, 1) NOT NULL,
    [external_system_type_id] INT           NOT NULL,
    [system_name]             VARCHAR (100) NOT NULL,
    CONSTRAINT [PK_external_systems] PRIMARY KEY CLUSTERED ([external_system_id] ASC),
    CONSTRAINT [FK_external_systems_external_system_types] FOREIGN KEY ([external_system_type_id]) REFERENCES [dbo].[external_system_types] ([external_system_type_id])
);

