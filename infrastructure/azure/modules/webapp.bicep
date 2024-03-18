/* Main bicep file provisioning essential resources for 
 * the media journal webapp */

// Params
// -------------

param appNamePrefix string

@allowed([
  'lab'
  'dev'
  'staging'
  'prod'
])
param environment string = 'staging'

param location string = resourceGroup().location

param appResourcesSubnetId string

param enabled bool = true

// Common values
// -------------

var envQualified = '${appNamePrefix}-${environment}'
var isProd = environment == 'prod'
var alwaysOn = isProd

// Existing resources
// -------------

resource appServicePlan 'Microsoft.Web/serverfarms@2023-01-01' existing = {
  name: '${isProd ? 'prod' : 'nonprod'}-linux-webapp-plan'
}

// Provisioned resources
// -------------

resource webapp 'Microsoft.Web/sites@2023-01-01' = {
  name: '${envQualified}-webapp'
  location: location
  kind: 'app,linux'
  properties: {
    enabled: enabled
    httpsOnly: true
    publicNetworkAccess: 'Enabled'
    reserved: true // must be true when the app kind is linux
    serverFarmId: appServicePlan.id

    siteConfig: {
      alwaysOn: alwaysOn
      linuxFxVersion: 'DOTNETCORE|8.0'
      minTlsVersion: '1.2'
      numberOfWorkers: 1
      scmType: 'GitHubAction'
      vnetRouteAllEnabled: true
    }
  }
  identity: {
    type: 'SystemAssigned'
  }
}

resource vnetIntegration 'Microsoft.Web/sites/networkConfig@2021-01-01' = {
  name: 'virtualNetwork'
  parent: webapp
  properties: {
    subnetResourceId: appResourcesSubnetId
  }
}

resource webappFtpPublishingPolicy 'Microsoft.Web/sites/basicPublishingCredentialsPolicies@2023-01-01' = {
  name: 'ftp'
  parent: webapp
  properties: {
    allow: false
  }
}

resource webappScmPublishingPolicy 'Microsoft.Web/sites/basicPublishingCredentialsPolicies@2023-01-01' = {
  name: 'scm'
  parent: webapp
  properties: {
    allow: false
  }
}

output managedIdentityPrincipalId string = webapp.identity.principalId
output webAppName string = webapp.name
