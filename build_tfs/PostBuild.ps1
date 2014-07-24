$octopus_api_key = "API-GABK8JUSYWEJXCJBUDAPXBSP3U"
$octopus_project = "NoesisLabs.Fiddler.HawkAddOn"
$octopus_server = "http://octopus.internal.noesislabs.com/api"


$scriptPath = Split-Path $MyInvocation.MyCommand.Path
$VersionRegex = "\d+\.\d+\.\d+\.\d+"


$VersionData = [regex]::matches($Env:TF_BUILD_BUILDNUMBER,$VersionRegex)
switch($VersionData.Count)
{
    0		
        { 
            Write-Error "Could not find version number data in TF_BUILD_BUILDNUMBER."
            exit 1
        }
    1
        {}
    default 
        { 
            Write-Warning "Found more than instance of version data in TF_BUILD_BUILDNUMBER." 
            Write-Warning "Will assume first instance is version."
        }
}
$octopus_release_version = $VersionData[0]

& $scriptPath\Octo.exe create-release --project=$octopus_project --version=$octopus_release_version --server=$octopus_server --apiKey=$octopus_api_key