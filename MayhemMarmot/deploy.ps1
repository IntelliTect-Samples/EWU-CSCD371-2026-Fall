# Usage: $env:GITHUB_TOKEN = "<PAT with read:packages>"; ./deploy.ps1 -AcrName <globally-unique-name>
param(
  [Parameter(Mandatory)][string]$AcrName,
  [string]$ResourceGroup = "rg-mayhemmarmot",
  [string]$Location = "westus2",
  [string]$AppName = "mayhem-marmot-api",
  [string]$EnvName = "mayhem-marmot-env"
)
$ErrorActionPreference = "Stop"
$image = "$AcrName.azurecr.io/mayhem-marmot:latest"

az group create -n $ResourceGroup -l $Location | Out-Null
az acr create -n $AcrName -g $ResourceGroup --sku Basic | Out-Null
az acr login -n $AcrName

# Build locally so the GitHub token stays a BuildKit secret (ACR cloud builds can't do that).
docker build --secret id=GITHUB_TOKEN,env=GITHUB_TOKEN -t $image .
docker push $image

az containerapp env create -n $EnvName -g $ResourceGroup -l $Location | Out-Null
az containerapp create -n $AppName -g $ResourceGroup --environment $EnvName `
  --image $image --registry-server "$AcrName.azurecr.io" --registry-identity system `
  --ingress external --target-port 8080 --cpu 0.25 --memory 0.5Gi --min-replicas 0 --max-replicas 2
az containerapp show -n $AppName -g $ResourceGroup --query properties.configuration.ingress.fqdn -o tsv
