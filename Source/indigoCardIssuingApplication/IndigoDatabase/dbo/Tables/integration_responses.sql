CREATE TABLE [dbo].[integration_responses] (
    [integration_id]                      INT           NOT NULL,
    [integration_object_id]               INT           NOT NULL,
    [integration_field_id]                INT           NOT NULL,
    [integration_response_id]             INT           NOT NULL,
    [integration_response_value]          VARCHAR (MAX) NOT NULL,
    [integration_response_valid_response] BIT           NOT NULL,
    CONSTRAINT [PK_integration_responses] PRIMARY KEY CLUSTERED ([integration_id] ASC, [integration_object_id] ASC, [integration_field_id] ASC, [integration_response_id] ASC),
    CONSTRAINT [FK_integration_responses_integration_fields] FOREIGN KEY ([integration_id], [integration_object_id], [integration_field_id]) REFERENCES [dbo].[integration_fields] ([integration_id], [integration_object_id], [integration_field_id])
);

