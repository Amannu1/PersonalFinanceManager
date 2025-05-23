﻿if ((Split-Path -Leaf (Get-Location)) -ne "UserManagerApi") {
    Set-Location UserManagerApi
}

Write-Host "Starting build..."
#Set-Location UserManagerApi
sam build -t serverless.template

Write-Host "Starting deploy..."
sam deploy --no-confirm-changeset

Read-Host -Prompt "Press Enter to exit"