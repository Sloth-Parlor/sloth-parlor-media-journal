/* Main bicep file provisioning essential resources for 
 * the media journal webapp */

// Params

param appName string = 'spmj-app'

@allowed([
  'staging'
  'prod'
])
param environment string = 'staging'

param location string = resourceGroup().location


// Common values
var envQualified = '${appName}-${environment}'

// Provisioned resources

resource appServicePlan 'Microsoft.Web/serverfarms@2023-01-01' = {
  name: '${envQualified}-linux-webapp-plan'
  location: location
  kind: 'linux'
  sku: {
    name: 'F1'
    tier: 'Free'
  }
  properties: {
    reserved: true
  }
}

resource webapp 'Microsoft.Web/sites@2023-01-01' = {
  name: '${envQualified}-webapp'
  location: location
  kind: 'app,linux'
  properties: {
    serverFarmId: appServicePlan.id
    enabled: true
    httpsOnly: true
    publicNetworkAccess: 'Enabled'
    reserved: true // must be true when the app kind is linux
    siteConfig: {
      alwaysOn: false
      linuxFxVersion: 'DOTNETCORE|8.0'
      minTlsVersion: '1.2'
      numberOfWorkers: 1
      scmType: 'GitHubAction'
    }
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

output webAppName string = webapp.name
output webAppHostname string = webapp.properties.defaultHostName

