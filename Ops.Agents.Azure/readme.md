Azure Setup

az ad sp create-for-rbac --name OpsAgent

results:
{
    "appId": "<<app Id>>",
    "displayName": "OpsAgent",
    "password": "<< secret >>",
    "tenant": "<< tenant >>"
}

az role assignment create --assignee "<<app Id>>" --role "Log Analytics Contributor" --resource-group "<<resource group>>"


