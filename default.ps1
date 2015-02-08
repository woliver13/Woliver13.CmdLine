Framework "4.0"

properties {
    $version = "0.1.0"
    $version = $version + "." + (get-date -format "MMdd") 
	$projectName = "Woliver13.CmdLine"
	$unitTestAssembly = "$projectName.UnitTests.dll"
	$integrationTestAssembly = "$projectName.IntegrationTests.dll"
	$projectConfig="Release"
	$base_dir = Resolve-Path .
	$source_dir = [IO.Path]::Combine($base_dir, "src")
	$nunitPath = [IO.Path]::Combine($source_dir, "packages\NUnit.Runners.2.6.4\tools")
	
	$build_dir = [IO.Path]::Combine($base_dir, "build")
	$package_dir = [IO.Path]::Combine($build_dir,"package")
	$test_dir = [IO.Path]::Combine($base_dir, "test")
	$testCopyIgnorePath = "_Resharper"
	$solution_file = [IO.Path]::Combine($source_dir,$projectName + ".sln")
}

task default -depends Init, Compile, Test
task ci -depends Init, Compile, Test, Package, NugetPack

task Init {
    delete_file $package_file
    delete_directory $build_dir
    create_directory $test_dir
	create_directory $build_dir
	create_directory $package_dir
}

task Compile -depends Init {
    exec { & $source_dir\.nuget\nuget.exe restore  $solution_file }  
    exec { msbuild /t:clean /v:q /nologo /p:Configuration=$projectConfig $solution_file /p:VisualStudioVersion=12.0 } 
    exec { msbuild /t:build /v:q /nologo /p:Configuration=$projectConfig $solution_file /p:VisualStudioVersion=12.0 } 
}

task Test {
	copy_all_assemblies_for_test $test_dir
	exec {
		& $nunitPath\nunit-console.exe $test_dir\$unitTestAssembly /nologo /nodots /xml=$build_dir\TestResult.xml    
	}
}
task Package -depends Compile {
	copy-item $source_dir\$projectName\bin\Release\$projectName.dll $package_dir\$projectName.dll
}

task NugetPack -depends Package {
	write-host $version
	write-host $build_dir
	write-host $base_dir\nuget\$projectName.nuspec
	exec {
		& $source_dir\.nuget\nuget.exe pack -Version $version -outputdirectory $build_dir $base_dir\nuget\$projectName.nuspec
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

function global:Copy_and_flatten ($source,$filter,$dest) {
  ls $source -filter $filter  -r | Where-Object{!$_.FullName.Contains("$testCopyIgnorePath") -and !$_.FullName.Contains("packages") }| cp -dest $dest -force
}

function global:copy_all_assemblies_for_test($destination) {
  create_directory $destination
  Copy_and_flatten $source_dir *.exe $destination
  Copy_and_flatten $source_dir *.dll $destination
  Copy_and_flatten $source_dir *.config $destination
  Copy_and_flatten $source_dir *.xml $destination
  Copy_and_flatten $source_dir *.pdb $destination
  Copy_and_flatten $source_dir *.sql $destination
  Copy_and_flatten $source_dir *.xlsx $destination
}