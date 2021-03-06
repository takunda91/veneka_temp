SELECT 
	ig.*
	, irl.[integration_object_id]
	, irl.[integration_field_id]
	, irl.[integration_response_id]
	, [response_text]
	, ir.integration_response_id
	, ir.integration_response_value
	, ir.integration_response_valid_response
	, inf.*
FROM 
	integration ig
	LEFT JOIN integration_fields inf ON inf.integration_id = ig.integration_id
	LEFT JOIN integration_responses ir ON ir.integration_id = ig.integration_id
	LEFT JOIN [integration_responses_language] irl ON ir.integration_id = ig.integration_id 