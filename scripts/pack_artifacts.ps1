param([String]$zipName = "miritore.zip")

[String]$configuration = $env:CONFIGURATION
[String]$basePath = $env:APPVEYOR_BUILD_FOLDER

$subPaths = @(
    [String]::Format("src/AcbPack/bin/{0}", $configuration),
    [String]::Format("src/HcaDec/bin/{0}", $configuration),
    [String]::Format("src/ManifestExport/bin/{0}", $configuration),
    [String]::Format("src/MillionDance/bin/x86/{0}", $configuration), # temporarily fixed to x86 because of AssetStudioUtilities
    [String]::Format("src/MillionDanceView/bin/{0}", $configuration),
    [String]::Format("src/MiriTore.Common/bin/{0}", $configuration),
    [String]::Format("src/MiriTore.Logging/bin/{0}", $configuration),
    [String]::Format("src/MltdInfoViewer/bin/{0}", $configuration),
    #[String]::Format("src/ScenarioEdit/bin/{0}", $configuration),
    [String]::Format("src/TDFacial/bin/{0}", $configuration), # remember to include facial_expr.json
    [String]::Format("src/ManifestTools/bin/{0}", $configuration),
    [String]::Empty
);

foreach ($subPath in $subPaths)
{
    if ( [String]::IsNullOrEmpty($subPath))
    {
        continue
    }

    [String]$fullDirPath = [System.IO.Path]::Combine($basePath, $subPath);
    [String]$xmlFilesPattern = [System.IO.Path]::Combine($fullDirPath, "*.xml")

    Remove-Item $xmlFilesPattern

    [String]$allFilesPattern = [System.IO.Path]::Combine($fullDirPath, "*")

    & 7z a $zipName -r $allFilesPattern
}

& 7z a miritore.zip "${env:APPVEYOR_BUILD_FOLDER}\README.html"

Push-AppveyorArtifact $zipName -FileName "miritore-appveyor-v${env:APPVEYOR_BUILD_VERSION}${env:RELEASE_SUFFIX}.zip" -DeploymentName "Versioned"
Push-AppveyorArtifact $zipName -FileName "miritore-appveyor-latest.zip" -DeploymentName "Latest"
