

MERGE INTO [dbo].[pin_calc_methods] AS trgt
USING	(VALUES
		(0,'VISA Method'),
		(1,'IBM Method'),
		(2,'No Calculation'),
		(3,'Custom Method')
		) AS src([pin_calc_method_id],[pin_calc_method_name])
ON
	trgt.[pin_calc_method_id] = src.[pin_calc_method_id]
WHEN MATCHED THEN
	UPDATE SET
		[pin_calc_method_id] = src.[pin_calc_method_id]
		, [pin_calc_method_name] = src.[pin_calc_method_name]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([pin_calc_method_id],[pin_calc_method_name])
	VALUES ([pin_calc_method_id],[pin_calc_method_name])

;

