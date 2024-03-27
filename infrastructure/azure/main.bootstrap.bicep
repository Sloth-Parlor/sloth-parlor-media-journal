/* Provisions resources that are expected to be part
 * of the environment but not managed incrementally.
 * 
 * This can be considered a kind of living documentation. 
 * 
 * It's acceptable to bootstrap an environemnt and then modify 
 * these resources manually. */

// Params
// -------------

param appNamePrefix string

@allowed([
  'lab'
  'dev'
  'staging'
  'prod'
])
param envName string

param vnetResourceGroupName string = resourceGroup().name

param vnetName string

param subnetAddressPrefix string

param location string = resourceGroup().location

// Existing resources
// -------------

resource vnetRg 'Microsoft.Resources/resourceGroups@2023-07-01' existing = {
  name: vnetResourceGroupName
  scope: subscription()
}

// Provisioned resources
// -------------

module appSubnet 'modules/app-subnet.bicep' = {
  name: 'appSubnet'
  scope: vnetRg
  params: {
    appVnetName: vnetName
    subnetName: '${appNamePrefix}-${envName}-app-subnet'
    addressPrefix: subnetAddressPrefix
  }
}

module servicePlans 'modules/plans.bicep' = {
  name: 'servicePlans'
  params: {
    isProd: envName == 'prod'
    location: location
  }
}

module keyvault 'modules/keyvault.bicep' = {
  name: 'keyvault'
  params: {
    appNamePrefix: appNamePrefix
    environment: envName
    location: location
    appResourcesSubnetId: appSubnet.outputs.appSubnetId
  }
}

module webapp 'modules/webapp.bicep' = {
  name: 'webapp'
  params: {
    appNamePrefix: appNamePrefix
    appResourcesSubnetId: appSubnet.outputs.appSubnetId
    environment: envName
    location: location
    enabled: false
  }
}

output webAppName string = webapp.outputs.webAppName
