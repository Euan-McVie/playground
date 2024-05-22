param location string = resourceGroup().location

@secure()
param vpnClientRootCert string

var common = loadJsonContent('../common.json')
var resourceSuffix = common.resourceSuffix

module virtualNetwork './network.bicep' = {
  params: {
    location: location
    resourceSuffix: resourceSuffix
    vpnClientRootCert: vpnClientRootCert
  }
}

module logging './logging.bicep' = {
  params: {
    location: location
    resourceSuffix: resourceSuffix
  }
}
