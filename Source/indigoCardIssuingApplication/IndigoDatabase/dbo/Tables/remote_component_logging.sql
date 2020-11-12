CREATE TABLE [dbo].[remote_component_logging]
(
	[date_logged] DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(), 
    [remote_address] VARCHAR(150) NOT NULL, 
    [token] VARCHAR(MAX) NOT NULL, 
    [request] VARCHAR(MAX) NOT NULL, 
    [method_call_id] INT NOT NULL 
)
