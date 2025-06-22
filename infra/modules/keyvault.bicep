// Key Vault template 

param environment string
param resourcePrefix string
param location string

resource keyVault 'Microsoft.KeyVault/vaults@2022-07-01' = {
  name: '${resourcePrefix}-kv'
  location: location
  properties: {
    sku: {
      family: 'A'
      name: 'standard'
    }
    tenantId: '<tenant-id>'
    accessPolicies: []
  }
}
