// =========================================================
// STEP 3 — App Service Plan + Web App (your Ecommerce.API)
// Only 'KeyVaultName' goes into App Settings. Everything else
// (Postgres connection string, Blob connection string, JWT key,
// Razorpay, Shiprocket) is read directly from Key Vault at
// startup via DefaultAzureCredential — see Program.cs snippet
// in STEP_BY_STEP.md.
//
// After this deploys, you MUST run one CLI command (shown in
// the guide) to grant this app's Managed Identity an access
// policy on the vault — Bicep can't do that here since it also
// requires a data-plane call, but 'az keyvault set-policy' is
// NOT a roleAssignments/write, so it works on your account.
// =========================================================

param location string = resourceGroup().location
param uniqueSuffix string = uniqueString(resourceGroup().id)

@description('Key Vault name you already created')
param existingKeyVaultName string

var appServicePlanName = 'ecommerce-plan'
var apiAppName = 'ecommerce-api-${uniqueSuffix}'

resource appServicePlan 'Microsoft.Web/serverfarms@2023-12-01' = {
  name: appServicePlanName
  location: location
  sku: { name: 'B1', tier: 'Basic' }
  properties: { reserved: true } // Linux
}

resource apiApp 'Microsoft.Web/sites@2023-12-01' = {
  name: apiAppName
  location: location
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: appServicePlan.id
    siteConfig: {
      linuxFxVersion: 'DOTNETCORE|10.0'
      appSettings: [
        { name: 'KeyVaultName', value: existingKeyVaultName }
        { name: 'ASPNETCORE_ENVIRONMENT', value: 'Production' }
      ]
    }
  }
}

output apiAppName string = apiApp.name
output apiAppUrl string = 'https://${apiApp.properties.defaultHostName}'
output apiPrincipalId string = apiApp.identity.principalId

