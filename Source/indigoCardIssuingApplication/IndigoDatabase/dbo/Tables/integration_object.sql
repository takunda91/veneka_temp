CREATE TABLE [dbo].[integration_object] (
    [integration_id]          INT           NOT NULL,
    [integration_object_id]   INT           NOT NULL,
    [integration_object_name] VARCHAR (150) NOT NULL,
    CONSTRAINT [PK_integration_object] PRIMARY KEY CLUSTERED ([integration_id] ASC, [integration_object_id] ASC),
    CONSTRAINT [FK_integration_object_integration] FOREIGN KEY ([integration_id]) REFERENCES [dbo].[integration] ([integration_id])
);

