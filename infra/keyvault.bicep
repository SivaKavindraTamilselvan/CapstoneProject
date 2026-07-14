// =========================================================
// STEP 0 — Key Vault (RBAC-enabled)
// Mirrors what you did manually in the CLI lab:
//   az keyvault create --enable-rbac-authorization true
//   az role assignment create --role "Key Vault Secrets Officer" --assignee <you>
//
// NOTE: You already created this manually — you likely don't
// need to run this file. Keeping it here as a reference so you
// can see the CLI steps translated into Bicep, and as a backup
// if you ever need to recreate it from scratch.
// =========================================================

param location string = resourceGroup().location
param uniqueSuffix string = uniqueString(resourceGroup().id)

var kvName = 'ecommerce-kv-${uniqueSuffix}'

// Same as: az keyvault create --enable-rbac-authorization true
resource keyVault 'Microsoft.KeyVault/vaults@2023-07-01' = {
  name: kvName
  location: location
  properties: {
    sku: { family: 'A', name: 'standard' }
    tenantId: subscription().tenantId
    enableRbacAuthorization: true
  }
}

// Same as Lab 2 Step 2 in your CLI lab:
//   az role assignment create --role "Key Vault Secrets Officer" --assignee %MY_UPN%
// This grants YOU (not an app) full read/write on secrets in this vault.
// NOTE: this is the same kind of role-assignment write operation that failed
// earlier for the App Service identity — if your account has the same
// restriction, this block will also fail with the same 403/Authorization
// error. If it does, comment this resource out and ask your Presidio admin
// to run the equivalent `az role assignment create` for you manually
// (same command shown above, outside of Bicep).
@description('Your Azure AD object ID — get it via: az ad signed-in-user show --query id -o tsv')
param myPrincipalId string

resource selfKvRoleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(keyVault.id, myPrincipalId, 'KeyVaultSecretsOfficer')
  scope: keyVault
  properties: {
    principalId: myPrincipalId
    principalType: 'User'
    // Key Vault Secrets Officer role ID
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', 'b86a8fe4-44ce-4948-aee5-eccb2c155cd7')
  }
}

output keyVaultName string = kvName
