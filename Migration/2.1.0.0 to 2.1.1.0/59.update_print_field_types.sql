
update print_field_types set print_field_name='Text' where print_field_type_id=0



INSERT INTO [dbo].[print_field_types_language]([print_field_type_id],[language_id],[language_text])
     VALUES (0,0,'Text')
GO
INSERT INTO [dbo].[print_field_types_language]([print_field_type_id],[language_id],[language_text])
     VALUES (0,1,'Text_fr')
GO
INSERT INTO [dbo].[print_field_types_language]([print_field_type_id],[language_id],[language_text])
     VALUES (0,2,'Text_pt')
GO
INSERT INTO [dbo].[print_field_types_language]([print_field_type_id],[language_id],[language_text])
     VALUES (0,3,'Text_es')
GO

INSERT INTO [dbo].[print_field_types_language]([print_field_type_id],[language_id],[language_text])
     VALUES (1,0,'Image')
GO
INSERT INTO [dbo].[print_field_types_language]([print_field_type_id],[language_id],[language_text])
     VALUES (1,1,'Image_fr')
GO
INSERT INTO [dbo].[print_field_types_language]([print_field_type_id],[language_id],[language_text])
     VALUES (1,2,'Image_pt')
GO
INSERT INTO [dbo].[print_field_types_language]([print_field_type_id],[language_id],[language_text])
     VALUES (1,3,'Image_es')
GO

