CREATE TABLE [dbo].[auth_configuration](
	[authentication_configuration_id] [int] IDENTITY(1,1) NOT NULL,
	[authentication_configuration] [varchar](100) NOT NULL,
 CONSTRAINT [PK_authentication_configuration] PRIMARY KEY CLUSTERED 
(
	[authentication_configuration_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
CREATE TABLE [dbo].[auth_configuration_connection_parameters](
	[authentication_configuration_id] [int] NOT NULL,
	[auth_type_id] [int] NULL,
	[connection_parameter_id] [int] NULL,
	[interface_guid] [char](36) NULL,
	[external_interface_id] [char](36) NULL
) ON [PRIMARY]

GO

SET ANSI_PADDING ON
GO

ALTER TABLE [dbo].[auth_configuration_connection_parameters]  WITH CHECK ADD  CONSTRAINT [FK_auth_configuration_connection_parameters_auth_configuration] FOREIGN KEY([authentication_configuration_id])
REFERENCES [dbo].[auth_configuration] ([authentication_configuration_id])
GO

ALTER TABLE [dbo].[auth_configuration_connection_parameters] CHECK CONSTRAINT [FK_auth_configuration_connection_parameters_auth_configuration]
GO
ALTER TABLE dbo.[user] ADD  authentication_configuration_id int
GO
Create table #temp(ID INT identity(1,1) ,connect_parameter_id int,external_interface_id char(36))

insert into #temp(connect_parameter_id,external_interface_id)
select distinct connection_parameter_id,external_interface_id from dbo.[user]
select * from #temp
DECLARE @LoopCounter INT , @MaxEmployeeId INT, 
        @connect_parameter_id NVARCHAR(100),
        @external_interface_id NVARCHAR(100)
SELECT @LoopCounter = min(id) , @MaxEmployeeId = max(Id) 
FROM #temp
 Declare @authConfigId   INT;
WHILE(@LoopCounter IS NOT NULL
      AND @LoopCounter <= @MaxEmployeeId)
BEGIN
   SELECT @connect_parameter_id = connect_parameter_id,@external_interface_id=external_interface_id
   FROM #temp WHERE Id = @LoopCounter
   SET @authConfigId=0
   IF ( @connect_parameter_id is null and @external_interface_id is null)
   BEGIN
 

   INSERT INTO auth_configuration
                         (authentication_configuration)
	VALUES        ('Indigo User Auth')
	SET @authConfigId=SCOPE_IDENTITY()
	INSERT INTO auth_configuration_connection_parameters
                         (authentication_configuration_id, auth_type_id, connection_parameter_id, interface_guid, external_interface_id)
VALUES        (@authConfigId,NULL,@connect_parameter_id,NULL,@external_interface_id)
  	update dbo.[user] set authentication_configuration_id=@authConfigId where  connection_parameter_id is null   and external_interface_id is null


END
 ELSE

 BEGIN

 INSERT INTO auth_configuration
                         (authentication_configuration)
	VALUES        ('User Auth Config'+CAST(@LoopCounter as varchar))

	SET @authConfigId=SCOPE_IDENTITY()
	
	INSERT INTO auth_configuration_connection_parameters
                         (authentication_configuration_id, auth_type_id, connection_parameter_id, interface_guid, external_interface_id)
VALUES        (@authConfigId,0,@connect_parameter_id,NULL,@external_interface_id)
	
  	update dbo.[user] set authentication_configuration_id=@authConfigId where  connection_parameter_id =  @connect_parameter_id and external_interface_id = @external_interface_id

 END
    
   SET @LoopCounter  = @LoopCounter  + 1 
          
END
drop table #temp
GO
ALTER TABLE [dbo].[user]  DROP CONSTRAINT  [FK__user__connection__3CD5A69B]

GO
ALTER TABLE [dbo].[user]  DROP  COLUMN [connection_parameter_id],[external_interface_id]
 