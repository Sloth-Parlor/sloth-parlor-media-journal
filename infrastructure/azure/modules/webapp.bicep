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
param environment string

param location string = resourceGroup().location

param appResourcesSubnetId string

param enabled bool = true

// Common values
// -------------
var aspNetCoreEnvNameMap = {
  lab: 'Development'
  dev: 'Development'
  staging: 'Staging'
  prod: 'Production'
}

var envQualified = '${appNamePrefix}-${environment}'
var isProd = environment == 'prod'
var alwaysOn = isProd
var aspNetCoreEnv = aspNetCoreEnvNameMap[environment]

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
    virtualNetworkSubnetId: appResourcesSubnetId
    siteConfig: {
      alwaysOn: alwaysOn
      linuxFxVersion: 'DOTNETCORE|8.0'
      minTlsVersion: '1.2'
      numberOfWorkers: 1
      scmType: 'GitHubAction'
      appSettings: [
        {
          name: 'WEBSITE_WEBDEPLOY_USE_SCM'
          value: string(!isProd)
        }
        {
          name: 'ASPNETCORE_ENVIRONMENT'
          value: aspNetCoreEnv
        }
      ]
    }
  }
  identity: {
    type: 'SystemAssigned'
  }
}

resource vnetConnection 'Microsoft.Web/sites/virtualNetworkConnections@2023-01-01' = {
  name: 'sp-mj-staging-webapp-subnet-connection'
  parent: webapp
  properties: {
    vnetResourceId: appResourcesSubnetId
    isSwift: true
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
