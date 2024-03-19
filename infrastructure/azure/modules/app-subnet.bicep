
param appVnetName string
param subnetName string
param addressPrefix string

resource spAppsNetwork 'Microsoft.Network/virtualNetworks@2020-06-01' existing = {
  name: appVnetName
}

resource appSubnet 'Microsoft.Network/virtualNetworks/subnets@2023-09-01' = {
  name: subnetName
  parent: spAppsNetwork
  properties: {
    addressPrefix: addressPrefix
    delegations: [
      {
        name: 'app-services-delegation'
        properties: {
          serviceName: 'Microsoft.Web/serverFarms'
        }
      }
    ]
    serviceEndpoints: [
      {
        service: 'Microsoft.KeyVault'
      }
    ]
  }
}

output appSubnetId string = appSubnet.id
