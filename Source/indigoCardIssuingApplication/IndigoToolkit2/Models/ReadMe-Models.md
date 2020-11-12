# Models

## DacProgress

This model contains Progress and Message properties which are used to capture the progress of a DACPAC service call and display back onto the UI

```csharp
public void AddProgress(string message)
```
This method will add a newline to the progress Propery. Checking if a new line is required or not.

## DacVariables

This model hold a collection of Key/Value pairs for any variables needed by the DacService. The primary use for this is to hold the CMDVAR's that may be in the publish profile. Present them to the UI for the user to add a value and then allocate them back to the publish profile before the DacService is run.

```Key``` Property may only be set via constructor. To prevent they Key from being changed once set.

## IndigoDatabase

This model will hold information about the Indigo Database 

- ```ConnectionString``` The connection string to the SQL Server and Instance the database will be hosted on
- ```TargetDatabaseName``` This is the name of the Database to be created or updated on the SQL Server instance
-  ```DacPacPath``` Path to where the .dacpac file is.
-  ```PublishProfilePath``` Path to the publish profile to use. Publish profile is important for what should be checked etc.

```csharp
public bool CheckAllFieldPopulated()
```
This method check that all required information has been populated. Used for controlling if buttons may execute or not.

## Settings

Not used