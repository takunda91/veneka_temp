CREATE TABLE [dbo].[file_load] (
    [file_load_id]     INT      IDENTITY (1, 1) NOT NULL,
    [file_load_start]  DATETIMEOFFSET NOT NULL,
    [file_load_end]    DATETIMEOFFSET NULL,
    [user_id]          INT      NOT NULL,
    [files_to_process] INT      NOT NULL,
    CONSTRAINT [PK_file_load] PRIMARY KEY CLUSTERED ([file_load_id] ASC)
);

