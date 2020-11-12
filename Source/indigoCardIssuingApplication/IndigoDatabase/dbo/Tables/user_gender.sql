CREATE TABLE [dbo].[user_gender] (
    [user_gender_id]   INT          IDENTITY (1, 1) NOT NULL,
    [user_gender_text] VARCHAR (15) NOT NULL,
    CONSTRAINT [PK_user_gender] PRIMARY KEY CLUSTERED ([user_gender_id] ASC)
);

GO
EXEC sp_addextendedproperty @name = N'VENEKA_TABLE_TYPE',
    @value = N'lookup',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'user_gender'