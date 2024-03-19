function Get-ProjectRoot {
    param ([int]$MaxTraversalDepth = 3, [string]$IndicatorFileName = '.scriptroot')

    $currentDirectory = $PSScriptRoot
    if (!$currentDirectory) {
        $currentDirectory = %$PWD.Path
    }

    $startingDirectory = $currentDirectory
    Write-Debug "Starting directory: $startingDirectory"

    for ($i = 0; $i -lt $MaxTraversalDepth; $i++) {
        $indicatorFile = Join-Path -Path $currentDirectory -ChildPath $IndicatorFileName

        $indicatorExists = Test-Path -Path $indicatorFile

        Write-Debug "Checking for indicator file at $indicatorFile. Exists: $indicatorExists"

        if ($indicatorExists) {
            return $currentDirectory
        }

        $currentDirectory = Resolve-Path (Join-Path -Path $currentDirectory -ChildPath '..')
    }

    throw "Could not find project root from $startingDirectory"
}
