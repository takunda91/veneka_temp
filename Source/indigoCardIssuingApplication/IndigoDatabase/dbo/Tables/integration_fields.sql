CREATE TABLE [dbo].[integration_fields] (
    [integration_id]                  INT             NOT NULL,
    [integration_object_id]           INT             NOT NULL,
    [integration_field_id]            INT             NOT NULL,
    [integration_field_name]          VARCHAR (150)   NOT NULL,
    [accept_all_responses]            BIT             NOT NULL,
    [integration_field_default_value] VARBINARY (MAX) NULL,
    CONSTRAINT [PK_integration_fields] PRIMARY KEY CLUSTERED ([integration_id] ASC, [integration_object_id] ASC, [integration_field_id] ASC),
    CONSTRAINT [FK_integration_fields_integration_object] FOREIGN KEY ([integration_id], [integration_object_id]) REFERENCES [dbo].[integration_object] ([integration_id], [integration_object_id])
);

