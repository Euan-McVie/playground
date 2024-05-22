#Login
$context = Get-AzContext
if (!$context -or $context.Subscription.Id -ne "d3689a8c-e70c-4539-8277-0f8089f46134") {
    Connect-AzAccount -Subscription "d3689a8c-e70c-4539-8277-0f8089f46134"
    Write-Host "Connected to $($context.Subscription.Name)"
}
else {
    Write-Host "Already connected to $($context.Subscription.Name)"
}
