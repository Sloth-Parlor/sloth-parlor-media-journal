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

param location string = resourceGroup().location

// Common values
// -------------

var envQualified = '${appNamePrefix}-${envName}'

// Provisioned resources
// -------------

module servicePlans 'modules/plans.bicep' = {
  name: 'servicePlans'
  params: {
    isProd: envName == 'prod'
    location: location
  }
}

resource appVirtualNetwork 'Microsoft.Network/virtualNetworks@2023-09-01' = {
  name: '${envQualified}-app-vnet'
  location: location
  properties: {
    addressSpace: {
      addressPrefixes: ['10.0.0.0/16']
    }
  }
}

resource appResourcesSubnet 'Microsoft.Network/virtualNetworks/subnets@2020-06-01' = {
  parent: appVirtualNetwork
  name: '${appNamePrefix}-resources'
  properties: {
    addressPrefix: '10.0.0.0/26'
    serviceEndpoints: [
      {
        service: 'Microsoft.KeyVault'
      }
    ]
    delegations: [
      {
        name: '${appNamePrefix}-app-services-delegation'
        properties: {
          serviceName: 'Microsoft.Web/serverFarms'
        }
      }
    ]
  }
}

resource appKeyVault 'Microsoft.KeyVault/vaults@2023-07-01' = {
  name: '${envQualified}-kv'
  location: location
  properties: {
    tenantId: subscription().tenantId
    accessPolicies: [
      {
        tenantId: subscription().tenantId
        objectId: webapp.outputs.managedIdentityPrincipalId
        permissions: {
          secrets: [
            'get'
            'list'
          ]
        }
      }
    ]
    networkAcls: {
      defaultAction: 'Deny'
      bypass: 'AzureServices'
      virtualNetworkRules: [
        {
          id: appResourcesSubnet.id
          ignoreMissingVnetServiceEndpoint: false
        }
      ]
    }
    sku: {
      family: 'A'
      name: 'standard'
    }
  }
}

module webapp 'modules/webapp.bicep' = {
  name: 'webapp'
  params: {
    appNamePrefix: appNamePrefix
    appResourcesSubnetId: appResourcesSubnet.id
    environment: envName
    location: location
    enabled: false
  }
}

output webAppName string = webapp.outputs.webAppName
