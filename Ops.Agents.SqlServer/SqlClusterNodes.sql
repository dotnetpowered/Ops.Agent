SELECT
    NodeName,
    Status = status_description,
    CurrentNode = is_current_owner
FROM
    sys.dm_os_cluster_nodes