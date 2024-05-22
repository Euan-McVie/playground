param resourceSuffix string

@secure()
param vpnClientRootCert string

param location string = resourceGroup().location

resource virtualNetwork 'Microsoft.Network/virtualNetworks@2023-09-01' = {
  name: 'vnet-${resourceSuffix}'
  location: location
  properties: {
    addressSpace: {
      addressPrefixes: [
        '10.0.0.0/16'
      ]
    }
    subnets: [
      {
        name: 'default'
        properties: {
          addressPrefix: '10.0.0.0/24'
        }
      }
      {
        name: 'flink'
        properties: {
          addressPrefix: '10.0.2.0/23'
        }
      }
      {
        name: 'GatewaySubnet'
        properties: {
          addressPrefix: '10.0.4.0/24'
        }
      }
    ]
  }
}

resource privateServiceBusDnsZone 'Microsoft.Network/privateDnsZones@2020-06-01' = {
  name: 'privatelink.servicebus.windows.net'
  location: 'global'
}

resource privateServiceBusDnsZoneVirtualNetworkLink 'Microsoft.Network/privateDnsZones/virtualNetworkLinks@2020-06-01' = {
  parent: privateServiceBusDnsZone
  name: 'pl-servicebus-${virtualNetwork.name}'
  location: 'global'
  properties: {
    registrationEnabled: false
    virtualNetwork: {
      id: virtualNetwork.id
    }
  }
}

resource publicIp 'Microsoft.Network/publicIPAddresses@2023-09-01' = {
  name: 'pip-${resourceSuffix}'
  location: location
  sku: {
    name: 'Basic'
    tier: 'Regional'
  }
  properties: {
    publicIPAllocationMethod: 'Dynamic'
  }
}

resource vnetGateway 'Microsoft.Network/virtualNetworkGateways@2023-09-01' = {
  name: 'vnetgw-${resourceSuffix}'
  location: location
  properties: {
    gatewayType: 'Vpn'
    vpnType: 'RouteBased'
    vpnGatewayGeneration: 'Generation1'
    sku: {
      name: 'Basic'
      tier: 'Basic'
    }
    ipConfigurations: [
      {
        name: 'default'
        properties: {
          privateIPAllocationMethod: 'Dynamic'
          publicIPAddress: {
            id: publicIp.id
          }
          subnet: {
            id: virtualNetwork.properties.subnets[2].id
          }
        }
      }
    ]
    vpnClientConfiguration: {
      vpnClientAddressPool: {
        addressPrefixes: [
          '172.66.0.0/24'
        ]
      }
      vpnClientRootCertificates: [
        {
          name: 'rootCert'
          properties: {
            publicCertData: vpnClientRootCert
          }
        }
      ]
    }
  }
}
