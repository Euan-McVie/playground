$certPath = 'Cert:\CurrentUser\My'
$rootCaCertSubject = 'CN=KafkaAndFlinkP2SRootCert'
$clientDnsName = 'KafkaAndFlinkP2SChildCert'
$clientCertSubject = "CN=${clientDnsName}"

# Get Existing Root CA Certificate if exists
$rootCaCerts = Get-ChildItem -Path $certPath | Where-Object { $_.Subject -eq $rootCaCertSubject }

if ($rootCaCerts) {
    $rootCaCert = $rootCaCerts[0]
    Write-Host "Using existing root CA certificate ${rootCaCert.Subject}"
}
else {
    # Generate a self-signed certificate for the root CA
    $rootCaParams = @{
        Type              = 'Custom'
        Subject           = $rootCaCertSubject
        KeySpec           = 'Signature'
        KeyExportPolicy   = 'Exportable'
        KeyUsage          = 'CertSign'
        KeyUsageProperty  = 'Sign'
        KeyLength         = 2048
        HashAlgorithm     = 'sha256'
        NotAfter          = (Get-Date).AddMonths(24)
        CertStoreLocation = $certPath
    }
    $rootCaCert = New-SelfSignedCertificate @rootCaParams
    Write-Host "Using new root CA certificate ${rootCaCert.Subject}"
}

# Get Existing Client Certificate if exists
$clientCerts = Get-ChildItem -Path $certPath | Where-Object { $_.Subject -eq $clientCertSubject }

if ($clientCerts) {
    $clientCert = $clientCerts[0]
    Write-Host "Using existing client certificate ${clientCert.Subject}"
}
if (!$clientCerts) {
    # Generate a self-signed certificate for client authentication
    $clientCertParams = @{
        Type              = 'Custom'
        Subject           = $clientCertSubject
        DnsName           = $clientDnsName
        KeySpec           = 'Signature'
        KeyExportPolicy   = 'Exportable'
        KeyLength         = 2048
        HashAlgorithm     = 'sha256'
        NotAfter          = (Get-Date).AddMonths(18)
        CertStoreLocation = $certPath
        Signer            = $rootCaCert
        TextExtension     = @('2.5.29.37={text}1.3.6.1.5.5.7.3.2')
    }
    $clientCert = New-SelfSignedCertificate @clientCertParams
    Write-Host "Using new client certificate ${clientCert.Subject}"
}

# Export the certificates
return @{
    RootCaCert = $rootCaCert
    ClientCert = $clientCert
}
