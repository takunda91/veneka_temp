USE [{DATABASE_NAME}]
GO

/****** Object:  Table [dbo].[mod_flex_parameters]    Script Date: 2016-07-19 11:57:46 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[mod_flex_parameters](
	[flex_parameter_id] [int] IDENTITY(1,1) NOT NULL,
	[source_code] [varchar](10) NOT NULL,
	[request_token] [varchar](100) NOT NULL,
	[request_type] [varchar](20) NOT NULL,
	[source_channel_id] [varchar](100) NOT NULL,
 CONSTRAINT [PK_flex_parameters] PRIMARY KEY CLUSTERED 
(
	[flex_parameter_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[mod_flex_response_values]    Script Date: 2016-07-19 11:57:46 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[mod_flex_response_values](
	[flex_response_value_id] [int] NOT NULL,
	[flex_response_id] [int] NOT NULL,
	[flex_response_value] [varchar](100) NOT NULL,
	[valid_response] [bit] NOT NULL,
 CONSTRAINT [PK_flex_response_values] PRIMARY KEY CLUSTERED 
(
	[flex_response_value_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[mod_flex_response_values_language]    Script Date: 2016-07-19 11:57:46 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[mod_flex_response_values_language](
	[flex_response_value_id] [int] NOT NULL,
	[language_id] [int] NOT NULL,
	[language_text] [varchar](max) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[flex_response_value_id] ASC,
	[language_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[mod_flex_responses]    Script Date: 2016-07-19 11:57:46 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[mod_flex_responses](
	[flex_response_id] [int] IDENTITY(1,1) NOT NULL,
	[flex_response_name] [varchar](100) NOT NULL,
 CONSTRAINT [PK_flex_responses] PRIMARY KEY CLUSTERED 
(
	[flex_response_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[mod_flex_response_values]  WITH CHECK ADD  CONSTRAINT [FK_flex_response_values_flex_responses] FOREIGN KEY([flex_response_id])
REFERENCES [dbo].[mod_flex_responses] ([flex_response_id])
GO
ALTER TABLE [dbo].[mod_flex_response_values] CHECK CONSTRAINT [FK_flex_response_values_flex_responses]
GO
ALTER TABLE [dbo].[mod_flex_response_values_language]  WITH CHECK ADD FOREIGN KEY([language_id])
REFERENCES [dbo].[languages] ([id])
GO
/****** Object:  StoredProcedure [dbo].[mod_usp_get_flex_parameter]    Script Date: 2016-07-19 11:57:46 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Fetch flexcube parameters
-- =============================================
CREATE PROCEDURE [dbo].[mod_usp_get_flex_parameter]
	@issuer_id int,	
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	--Fetch the parameters
	SELECT [mod_flex_parameters].*, [issuer].issuer_code
	FROM [mod_flex_parameters], [issuer]
	WHERE [mod_flex_parameters].flex_parameter_id = 1 AND
			[issuer].issuer_id = @issuer_id

END

GO
/****** Object:  StoredProcedure [dbo].[mod_usp_get_flex_responses]    Script Date: 2016-07-19 11:57:46 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[mod_usp_get_flex_responses]
	@responseCode varchar(20),
	@accountType varchar(20),
	@accountStatus varchar(20),
	@atmFlag varchar(20),
	@responseMessage varchar(50),
	@language_id int,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT [mod_flex_responses].flex_response_name, 
			[mod_flex_response_values].*, [mod_flex_response_values_language].language_text
	FROM [mod_flex_responses] 
			INNER JOIN [mod_flex_response_values] 		
				ON [mod_flex_responses].flex_response_id = [mod_flex_response_values].flex_response_id
			INNER JOIN [mod_flex_response_values_language]
				ON [mod_flex_response_values].flex_response_value_id = [mod_flex_response_values_language].flex_response_value_id
					AND [mod_flex_response_values_language].language_id = @language_id
	WHERE ([mod_flex_responses].flex_response_id = 1 AND [mod_flex_response_values].flex_response_value = @responseCode)
		  OR ([mod_flex_responses].flex_response_id = 2 AND [mod_flex_response_values].flex_response_value = @accountType)
		  OR ([mod_flex_responses].flex_response_id = 3 AND [mod_flex_response_values].flex_response_value = @accountStatus)
		  OR ([mod_flex_responses].flex_response_id = 4 AND [mod_flex_response_values].flex_response_value = @atmFlag)
		  OR ([mod_flex_responses].flex_response_id = 5 AND [mod_flex_response_values].flex_response_value = @responseMessage)	 

END

GO
/****** Object:  StoredProcedure [dbo].[mod_usp_insert_Flexcube_audit]    Script Date: 2016-07-19 11:57:46 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[mod_usp_insert_Flexcube_audit] 
	@audit_user_id BIGINT,
	@audit_workstation VARCHAR(100),
	@audit_description varchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;


	EXEC usp_insert_audit @audit_user_id, 
						 3,
						 NULL, 
						 @audit_workstation, 
						 @audit_description, 
						 NULL, NULL, NULL, NULL
END
GO