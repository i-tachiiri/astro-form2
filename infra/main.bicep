// Bicep main template 

param environment string
param resourcePrefix string
param location string = 'japaneast'

module swa 'modules/swa.bicep' = {
  name: 'swa'
  params: {
    environment: environment
    resourcePrefix: resourcePrefix
    location: location
  }
}

module function 'modules/function.bicep' = {
  name: 'function'
  params: {
    environment: environment
    resourcePrefix: resourcePrefix
    location: location
  }
}

module keyvault 'modules/keyvault.bicep' = {
  name: 'keyvault'
  params: {
    environment: environment
    resourcePrefix: resourcePrefix
    location: location
  }
}

module cosmos 'modules/cosmos.bicep' = {
  name: 'cosmos'
  params: {
    environment: environment
    resourcePrefix: resourcePrefix
    location: location
  }
} 
