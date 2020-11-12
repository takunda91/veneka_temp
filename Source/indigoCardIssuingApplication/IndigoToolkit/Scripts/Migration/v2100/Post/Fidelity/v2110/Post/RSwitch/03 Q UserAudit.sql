USE [{DATABASE_NAME}]
GO

SET IDENTITY_INSERT [user_group] ON	
	INSERT INTO [dbo].[user_group]
           ([user_group_id], [user_role_id],[issuer_id],[can_create],[can_read],[can_update],[can_delete],[all_branch_access],[user_group_name],[mask_report_pan],[mask_screen_pan])
     VALUES
           (-4, 17, -1, 1, 1, 1, 1, 1, 'GROUP_USER_AUDIT', 1, 1)
SET IDENTITY_INSERT [user_group] OFF
GO

SET IDENTITY_INSERT [user] ON
	declare @objid int
	SET @objid = object_id('user')
	OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate
		INSERT INTO [dbo].[user]
			   ([user_id],[user_status_id],[user_gender_id],[username],[first_name],[last_name],[password],[online]
			   ,[employee_id],[last_login_date],[last_login_attempt],[number_of_incorrect_logins],[last_password_changed_date]
			   ,[workstation],[language_id],[username_index],[user_email],[time_zone_utcoffset],[time_zone_id])
		 VALUES
			   (-3, 0, 3, ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,'useraudit')),
			   ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,'useraudit')),
			   ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,'useraudit')),
			   ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,'WJy1rr/hG0Z7b4aFSiGmcA==')),
			   0, ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,'useraudit')),
			   SYSDATETIMEOFFSET(), SYSDATETIMEOFFSET(), 0, SYSDATETIMEOFFSET(), '', 0, 
			   dbo.MAC('useraudit', @objid),'sys', '+00:00', 'GMT Standard Time')
	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;
SET IDENTITY_INSERT [user] OFF
GO

INSERT INTO [dbo].[users_to_users_groups]
           ([user_id],[user_group_id])
     VALUES (-3, -4)
	 GO

	IF OBJECT_ID('tempdb.dbo.#temp', 'U') IS NOT NULL
	 DROP TABLE #temp; 
  GO
  ----Migrating the User

	 Create table #temp(ID INT identity(1,1) ,connect_parameter_id int,external_interface_id char(36))

insert into #temp(connect_parameter_id,external_interface_id)
select distinct connection_parameter_id,external_interface_id from [{SOURCE_DATABASE_NAME}].dbo.[user]
select * from #temp
DECLARE @LoopCounter INT , @MaxEmployeeId INT, 
        @connect_parameter_id NVARCHAR(100),
        @external_interface_id NVARCHAR(100)
SELECT @LoopCounter = min(id) , @MaxUserId = max(Id) 
FROM #temp
 Declare @authConfigId   INT;
WHILE(@LoopCounter IS NOT NULL
      AND @LoopCounter <= @MaxUserId)
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