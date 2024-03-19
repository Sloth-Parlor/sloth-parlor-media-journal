function Confirm-AzAccount {
    # Check if the user is already logged in
    $loginCheck = az account show 2>$null

    if ($loginCheck) {
        $account = az account show --output json | ConvertFrom-Json
        $subscriptionName = $account.name
        $accountName = $account.user.name
        Write-Host "Logged in as $accountName to subscription $subscriptionName."
    } else {
        throw "You are not logged in to Azure. Please login and try again."
    }

    # Prompt the user to continue
    Read-Host -Prompt "If this is not the correct subscription, ctrl + c to cancel; press enter to continue"
    Write-Host ""
}