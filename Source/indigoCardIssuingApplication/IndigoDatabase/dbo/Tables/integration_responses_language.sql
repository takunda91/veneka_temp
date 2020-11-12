CREATE TABLE [dbo].[integration_responses_language] (
    [integration_id]          INT            NOT NULL,
    [integration_object_id]   INT            NOT NULL,
    [integration_field_id]    INT            NOT NULL,
    [integration_response_id] INT            NOT NULL,
    [language_id]             INT            NOT NULL,
    [response_text]           NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_integration_responses_language] PRIMARY KEY CLUSTERED ([integration_id] ASC, [integration_object_id] ASC, [integration_field_id] ASC, [integration_response_id] ASC, [language_id] ASC),
    CONSTRAINT [FK_integration_responses_language_integration_responses] FOREIGN KEY ([integration_id], [integration_object_id], [integration_field_id], [integration_response_id]) REFERENCES [dbo].[integration_responses] ([integration_id], [integration_object_id], [integration_field_id], [integration_response_id])
);

