Import-Module ..\modules\ProjectRoot.psm1
Import-Module ..\modules\AzureHelpers.psm1

$appNamePrefix = 'sp-mj'
$resourceGroupName = 'rg-sp-useast2-mj-preprod'
$envName = 'staging'
$subnetAddressPrefix = '10.0.2.0/26'
$appVnetName = 'sp-apps-vnet'

Confirm-AzAccount

$projectRoot = Get-ProjectRoot

Write-Host "Deploying main bootstrap..."
az deployment group create `
    --resource-group "$resourceGroupName" `
    --template-file "$projectRoot/infrastructure/azure/main.bootstrap.bicep" `
    --parameters appNamePrefix=$appNamePrefix envName=$envName appVnetName=$appVnetName subnetAddressPrefix=$subnetAddressPrefix

Write-Host "Running post-deployment steps..."


Write-Host "Deployment completed successfully."