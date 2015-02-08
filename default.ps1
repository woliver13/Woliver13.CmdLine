Framework "4.0"

properties {
	$projectName = "Woliver13.CmdLine"
	$unitTestAssembly = "$projectName.UnitTests.dll"
	$integrationTestAssembly = "$projectName.IntegrationTests.dll"
	$projectConfig="Release"
	$base_dir = Resolve-Path .
	$source_dir = [IO.Path]::Combine($base_dir, "src")
	$nunitPath = [IO.Path]::Combine($source_dir, "packages\NUnit.Runners.2.6.3\tools")
	
	$build_dir = [IO.Path]::Combine($base_dir, "build")
	$test_dir = [IO.Path]::Combine($base_dir, "test")
	$testCopyIgnorePath = "_Resharper"
	$package_dir = [IO.Path]::Combine($build_dir, "package")
	$package_file = [IO.Path]::Combine($build_dir, "\latestVersion\" + $projectName + "_Package.zip")
	$solution_file = [IO.Path]::Combine($source_dir,$projectName + ".sln")
}

task default -depends Init, Compile, Test

task Init {
    delete_file $package_file
    delete_directory $build_dir
    create_directory $test_dir
	create_directory $build_dir
}

task Compile -depends Init {
	echo $base_dir
	echo $source_dir
    exec { & $source_dir\packages\NuGet.CommandLine.2.5.0\tools\NuGet.exe restore  $solution_file }  
    exec { msbuild /t:clean /v:q /nologo /p:Configuration=$projectConfig $solution_file /p:VisualStudioVersion=12.0 } 
    exec { msbuild /t:build /v:q /nologo /p:Configuration=$projectConfig $solution_file /p:VisualStudioVersion=12.0 } 
}

task Test {
	copy_all_assemblies_for_test $test_dir
	exec {
		& $nunitPath\nunit-console.exe $test_dir\$unitTestAssembly $test_dir\$integrationTestAssembly /nologo /nodots /xml=$build_dir\TestResult.xml    
	}
}

function global:delete_file($file) {
    if($file) { remove-item $file -force -ErrorAction SilentlyContinue | out-null } 
}

function global:delete_directory($directory_name) {
	rd $directory_name -recurse -force  -ErrorAction SilentlyContinue | out-null
}

function global:create_directory($directory_name) {
	mkdir $directory_name  -ErrorAction SilentlyContinue  | out-null
}