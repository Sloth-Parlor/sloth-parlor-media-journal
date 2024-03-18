// Params
// -------------

param isProd bool = false

param location string = resourceGroup().location


// Provisioned resources
// -------------

resource nonProd 'Microsoft.Web/serverfarms@2023-01-01' = if (!isProd) {
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

resource webAppPlans 'Microsoft.Web/serverfarms@2023-01-01' = if (isProd) {
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
