powershell.exe -NoProfile -ExecutionPolicy unrestricted -Command "& { Import-Module '.\src\packages\psake.4.2.0.1\tools\psake.psm1'; Invoke-psake .\default.ps1; }"
pause