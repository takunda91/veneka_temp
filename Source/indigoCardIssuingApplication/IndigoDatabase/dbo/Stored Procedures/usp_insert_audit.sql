CREATE PROCEDURE [dbo].[usp_insert_audit]
	@audit_user_id bigint,
	@audit_action_id int,
	@audit_date datetimeoffset = NULL,
	@workstation_address nvarchar(100),
	@action_description varchar(max),
	@issuer_id int = NULL,
	@data_changed varchar(max) = NULL,
	@data_before varchar(max) = NULL,
	@data_after varchar(max) = NULL

AS
BEGIN
	INSERT INTO [audit_control]
           ([audit_action_id]
           ,[user_id]
           ,[audit_date]
           ,[workstation_address]
           ,[action_description]
           ,[issuer_id]
           ,[data_changed]
           ,[data_before]
           ,[data_after])
     VALUES
           (@audit_action_id
           ,@audit_user_id
           ,COALESCE(@audit_date, SYSDATETIMEOFFSET())
           ,@workstation_address
           ,@action_description
           ,@issuer_id
           ,@data_changed
           ,@data_before
           ,@data_after)
	
END