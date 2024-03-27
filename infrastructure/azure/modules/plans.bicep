// Params
// -------------

param isProd bool = false

param location string = resourceGroup().location

// Provisioned resources
// -------------

resource nonProdPlan 'Microsoft.Web/serverfarms@2023-01-01' =
  if (!isProd) {
    name: 'nonprod-linux-webapp-plan'
    location: location
    kind: 'linux'
    sku: {
      name: 'B1'
      tier: 'Basic'
    }
    properties: {
      reserved: true
    }
  }

resource prodPlan 'Microsoft.Web/serverfarms@2023-01-01' =
  if (isProd) {
    name: 'prod-linux-webapp-plan'
    location: location
    kind: 'linux'
    sku: {
      name: 'B1'
      tier: 'Basic'
    }
    properties: {
      reserved: true
    }
  }
