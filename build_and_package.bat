powershell.exe -NoProfile -ExecutionPolicy unrestricted -Command "& { Import-Module '.\src\packages\psake.4.2.0.1\tools\psake.psm1'; Invoke-psake ci -parameters @{%*}; if ($psake.build_success -eq $false) { exit 1 } else { exit 0 } }" 
pause