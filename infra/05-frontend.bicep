// =========================================================
// STEP 5 — Static Web App (Angular frontend)
// =========================================================

param location string = 'centralus' // Static Web Apps only available in a few regions
param uniqueSuffix string = uniqueString(resourceGroup().id)

var staticAppName = 'ecommerce-ui-${uniqueSuffix}'

resource staticWebApp 'Microsoft.Web/staticSites@2023-12-01' = {
  name: staticAppName
  location: location
  sku: {
    name: 'Free'
    tier: 'Free'
  }
  properties: {
    // No GitHub repo linked here — we'll deploy manually via CLI (swa deploy)
    // instead of CI/CD, since the repo isn't connected to this pipeline.
  }
}

output staticAppName string = staticWebApp.name
output staticAppUrl string = 'https://${staticWebApp.properties.defaultHostname}'
