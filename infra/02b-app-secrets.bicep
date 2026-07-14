// =========================================================
// STEP 2b — Additional app secrets into Key Vault
// (JWT signing key, Razorpay, Shiprocket)
// Run this any time after your vault exists — order relative
// to Postgres/Storage doesn't matter for this one.
// =========================================================

@description('Key Vault name you already created')
param existingKeyVaultName string

@secure()
param jwtKey string
param razorpayKeyId string
@secure()
param razorpayKeySecret string
param shiprocketEmail string
@secure()
param shiprocketPassword string

resource existingKeyVault 'Microsoft.KeyVault/vaults@2023-07-01' existing = {
  name: existingKeyVaultName
}

resource jwtSecret 'Microsoft.KeyVault/vaults/secrets@2023-07-01' = {
  parent: existingKeyVault
  name: 'JwtKey'
  properties: { value: jwtKey }
}

resource razorpayKeyIdSecret 'Microsoft.KeyVault/vaults/secrets@2023-07-01' = {
  parent: existingKeyVault
  name: 'RazorpayKeyId'
  properties: { value: razorpayKeyId }
}

resource razorpaySecret 'Microsoft.KeyVault/vaults/secrets@2023-07-01' = {
  parent: existingKeyVault
  name: 'RazorpayKeySecret'
  properties: { value: razorpayKeySecret }
}

resource shiprocketEmailSecret 'Microsoft.KeyVault/vaults/secrets@2023-07-01' = {
  parent: existingKeyVault
  name: 'ShiprocketEmail'
  properties: { value: shiprocketEmail }
}

resource shiprocketPasswordSecret 'Microsoft.KeyVault/vaults/secrets@2023-07-01' = {
  parent: existingKeyVault
  name: 'ShiprocketPassword'
  properties: { value: shiprocketPassword }
}
