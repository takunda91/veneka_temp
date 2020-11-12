# Zebra Error Factory

Factory used to lookup error description and helpful hint when passed a Zebra error code.
Does the lookup against the `ZebraErrors.resx` file. If the error isnt found a generic response will be returned.

When addint entries into the resx files. Ensure that you stick to the following format:

| Type | Format | Example |
|-----|-----|------|
| Error Description | z**ErrorCode**_desc | z7008_desc |
| Helpful Hint | z**ErrorCode**_help | z7008_help |