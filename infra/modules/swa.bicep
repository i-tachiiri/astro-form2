// Static Web Apps template 

param environment string
param resourcePrefix string
param location string

resource swa 'Microsoft.Web/staticSites@2022-03-01' = {
  name: '${resourcePrefix}-swa'
  location: location
  sku: {
    name: 'Free'
  }
}
