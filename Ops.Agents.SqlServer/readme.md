
# Ops.Agents.SqlServer agent

## Setup

Configuration example:
```json
    {
      "agent": "SqlServer",
      "connectionString": "Server=<<server>>;TrustServerCertificate=true;Database=master;user id=OpsMonitor;password=<<password>>"
    }
```

Learn more on [MS SQL Server connection strings](https://www.connectionstrings.com/microsoft-data-sqlclient/)

### Database login permissions

Sample script to create monitor user: (replace the xxxxx PASSWORD with the actual password)
```sql
USE [master]
GO
CREATE LOGIN [OpsMonitor] WITH PASSWORD=N'xxxxx', DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF
GO
GRANT VIEW SERVER STATE TO [OpsMonitor] 
GO
ALTER LOGIN [OpsMonitor] DISABLE
GO
```

If you don't grant user the "VIEW SERVER STATE" permission, you will receive an error:

VIEW SERVER STATE permission was denied on object 'server', database 'master'