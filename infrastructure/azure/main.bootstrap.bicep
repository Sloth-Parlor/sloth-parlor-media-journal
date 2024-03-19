/* Provisions resources that are expected to be part
 * of the environment but not managed incrementally.
 * This can be considered a kind of living documentation. 
 * It's acceptable to bootstrap an environemnt and then modify 
 * these resources manually. */

// Params
// -------------

param appNamePrefix string = 'sp-mj'

@allowed([
  'lab'
  'dev'
  'staging'
  'prod'
])
param envName string

param appVnetName string

param subnetAddressPrefix string

param location string = resourceGroup().location

// Existing resources
// -------------
resource appCommonRg 'Microsoft.Resources/resourceGroups@2023-07-01' existing = {
  name: 'rg-sp-useast2-app-common-preprod'
  scope: subscription()
}

// Provisioned resources
// -------------

module appSubnet 'modules/app-subnet.bicep' = {
  name: 'appSubnet'
  scope: appCommonRg
  params: {
    appVnetName: appVnetName
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
