CREATE TYPE [dbo].[notifications_lang_messages] AS TABLE (
    [language_id]       INT           NOT NULL,
    [channel_id]        INT           NULL,
    [notification_text] VARCHAR (MAX) NOT NULL,
    [subject_text]      VARCHAR (MAX) NOT NULL,
	[from_address]     NVARCHAR(50) NOT NULL,
    PRIMARY KEY CLUSTERED ([language_id] ASC));

