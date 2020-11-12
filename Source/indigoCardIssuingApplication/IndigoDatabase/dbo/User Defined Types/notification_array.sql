CREATE TYPE [dbo].[notification_array] AS TABLE (
    [message_id]   UNIQUEIDENTIFIER NULL,
    [message_text] VARCHAR (MAX)    NULL);

