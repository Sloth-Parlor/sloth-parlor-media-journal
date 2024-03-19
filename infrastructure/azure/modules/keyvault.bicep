// Params
// ------

param appNamePrefix string

@allowed([
  'lab'
  'dev'
  'staging'
  'prod'
])
param environment string

param location string = resourceGroup().location

param appResourcesSubnetId string

// Common variables
// ----------------

var envQualified = '${appNamePrefix}-${environment}'

// Provisioned resources
// ---------------------

resource appKeyVault 'Microsoft.KeyVault/vaults@2023-07-01' = {
  name: '${envQualified}-kv'
  location: location
  properties: {
    enabledForTemplateDeployment: true
    enableRbacAuthorization: true
    tenantId: subscription().tenantId
    networkAcls: {
      defaultAction: 'Deny'
      bypass: 'AzureServices'
      virtualNetworkRules: [
        {
          id: appResourcesSubnetId
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
