Import-Module ..\modules\ProjectRoot.psm1
Import-Module ..\modules\AzureHelpers.psm1

Confirm-AzAccount

$projectRoot = Get-ProjectRoot

Write-Host "Deploying main bootstrap..."
# Sample command
# az deployment group create `
#     --resource-group "$resourceGroupName" `
#     --template-file "$projectRoot/infrastructure/azure/main.bootstrap.bicep" `
#     --parameters appNamePrefix=$appNamePrefix envName=$envName appVnetName=$appVnetName subnetAddressPrefix=$subnetAddressPrefix

Write-Host "Running post-deployment steps..."


Write-Host "Deployment completed successfully."