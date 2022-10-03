SELECT
	@@SERVERNAME as ServerName,
	SERVERPROPERTY('MachineName') as MachineName,
	@@Version as Version,
	@@LANGUAGE as [Language],
	sqlserver_start_time as StartTime,
	cpu_count as CpuCount, 
	physical_memory_kb/1024 as PhysicalMemoryMB, 
	physical_memory_in_use_kb/1024 AS UsedMemoryMB,
	SERVERPROPERTY('ProductVersion') as ProductVersion,
	SERVERPROPERTY('Edition') as Edition,
	SERVERPROPERTY('ProductLevel') as ProductLevel,
	SERVERPROPERTY('ProductUpdateReference') as ProductUpdateReference,
	SERVERPROPERTY('EngineEdition') as EngineEdition,
	SERVERPROPERTY('InstanceName') as InstanceName,
	SERVERPROPERTY('IsIntegratedSecurityOnly') as SecurityMode,
	SERVERPROPERTY('IsHadrEnabled') as AlwaysOnEnabled,
	SERVERPROPERTY('HadrManagerStatus') as AlwaysOnManager,
	SERVERPROPERTY('IsClustered') as IsClustered,
	AgentStatus=(SELECT status_desc
		FROM sys.dm_server_services  WITH(NOLOCK)
		where servicename like '%Agent%'),
	OnlineDatabases = (SELECT count(*) FROM sys.databases WITH(NOLOCK) where state=0),
	OfflineDatabases = (SELECT count(*) FROM sys.databases WITH(NOLOCK) where state<>0),
	Blocks = (SELECT count(*) FROM sys.dm_exec_requests WHERE blocking_session_id <> 0),
    process_physical_memory_low,
	process_virtual_memory_low
from sys.dm_os_sys_info si
cross join sys.dm_os_process_memory pm WITH(NOLOCK) 
OPTION(RECOMPILE);
