Connect-VIServer -Server {{SERVER}} -Protocol https `
    -User {{USER_NAME}} -Password {{PASSWORD}} -Force
Get-VM * | Select-Object `
    Name, NumCpu, MemoryGB, PowerState, Id, Notes, ProvisionedSpaceGB, UsedSpaceGB, CreateDate, DrsAutomationLevel, `
    @{Name='OSName';Expression={$_.Guest.OSFullName};}, `
    @{Name='IpAddress';Expression={$_.Guest.IPAddress};}, `
    @{Name='HostName';Expression={$_.Guest.HostName};}, `
    @{Name='GuestState';Expression={$_.Guest.State};}, `
    @{Name='GuestFamily';Expression={$_.Guest.GuestFamily};}, `
    @{Name='VmVersion';Expression={$_.Version};}, `
    @{Name='VmHostId';Expression={$_.VMHost.Id};}, `
    @{Name='VmHostName';Expression={$_.VMHost.Name};} `
    | ConvertTo-Json 
