CREATE TABLE [dbo].[external_system_fields] (
    [external_system_field_id] INT           IDENTITY (1, 1) NOT NULL,
    [external_system_id]       INT           NOT NULL,
    [field_name]               VARCHAR (100) NOT NULL,
    CONSTRAINT [PK_external_system_fields] PRIMARY KEY CLUSTERED ([external_system_field_id] ASC),
    CONSTRAINT [FK_external_system_fields_external_systems] FOREIGN KEY ([external_system_id]) REFERENCES [dbo].[external_systems] ([external_system_id])
);

