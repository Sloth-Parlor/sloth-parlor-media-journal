/* Main bicep file incrementally provisioning essential 
 * resources for the media journal webapp */

// Params
// -------------

param appNamePrefix string

@allowed([
  'dev'
  'staging'
  'prod'
])
param envName string

param vnetResourceGroupName string

param vnetName string

param subnetName string

param location string = resourceGroup().location

// Common variables
// -------------

var envQualified = '${appNamePrefix}-${envName}'

// Existing resources
// -------------

resource vnetResourceGroup 'Microsoft.Resources/resourceGroups@2023-07-01' existing = {
  name: vnetResourceGroupName
  scope: subscription()
}

resource spAppsNetwork 'Microsoft.Network/virtualNetworks@2020-06-01' existing = {
  name: vnetName
  scope: vnetResourceGroup
}

resource appSubnet 'Microsoft.Network/virtualNetworks/subnets@2023-09-01' existing = {
  name: subnetName
  parent: spAppsNetwork
}

resource appKeyVault 'Microsoft.KeyVault/vaults@2023-07-01' existing = {
  name: '${envQualified}-kv'
}

// Provisioned resources
// -------------

module webapp './modules/webapp.bicep' = {
  name: 'webapp'
  params: {
    appNamePrefix: appNamePrefix
    appResourcesSubnetId: appSubnet.id
    keyvaultUri: appKeyVault.properties.vaultUri
    environment: envName
    location: location
  }
}

output webAppName string = webapp.outputs.webAppName
output appSubnetId string = appSubnet.id
