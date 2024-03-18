/* Main bicep file incrementally provisioning essential 
 * resources for the media journal webapp */

// Params
// -------------

param appNamePrefix string = 'sp-mj'

@allowed([
  'dev'
  'staging'
  'prod'
])
param envName string

param location string = resourceGroup().location

// Common values
// -------------

var envQualified = '${appNamePrefix}-${envName}'

// Existing resources
// -------------

resource appResourcesVirtualNetwork 'Microsoft.Network/virtualNetworks@2023-09-01' existing = {
  name: '${envQualified}-app-vnet'
}

resource appResourcesSubnet 'Microsoft.Network/virtualNetworks/subnets@2023-09-01' existing = {
  parent: appResourcesVirtualNetwork
  name: '${appNamePrefix}-resources'
}

// Provisioned resources
// -------------

module webapp './modules/webapp.bicep' = {
  name: 'webapp'
  params: {
    appNamePrefix: appNamePrefix
    appResourcesSubnetId: appResourcesSubnet.id
    environment: envName
    location: location
  }
}

output webAppName string = webapp.outputs.webAppName
