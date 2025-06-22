// Azure Functions template 

param environment string
param resourcePrefix string
param location string

resource functionApp 'Microsoft.Web/sites@2022-03-01' = {
  name: '${resourcePrefix}-func'
  location: location
  kind: 'functionapp'
}
