@echo off
rem goto upload

if not exist "..\Misc-Beta-Test\packages\OpenCover.4.6.519\tools\OpenCover.Console.exe" goto error_console1
if not exist "..\Misc-Beta-Test\packages\NUnit.ConsoleRunner.3.9.0\tools\nunit3-console.exe" goto error_console2
if not exist "..\Misc-Beta-Test\Test-FormattedNumber\bin\x64\Debug\Test-FormattedNumber.dll" goto error_not_built
if not exist "..\Misc-Beta-Test\Test-FormattedNumber\bin\x64\Release\Test-FormattedNumber.dll" goto error_not_built
if exist *.log del *.log
if exist ..\Misc-Beta-Test\Test-FormattedNumber\obj\x64\Debug\Coverage-FormattedNumber-Debug_coverage.xml del ..\Misc-Beta-Test\Test-FormattedNumber\obj\x64\Debug\Coverage-FormattedNumber-Debug_coverage.xml
if exist ..\Misc-Beta-Test\Test-FormattedNumber\obj\x64\Release\Coverage-FormattedNumber-Release_coverage.xml del ..\Misc-Beta-Test\Test-FormattedNumber\obj\x64\Release\Coverage-FormattedNumber-Release_coverage.xml

:runtests
"..\Misc-Beta-Test\packages\OpenCover.4.6.519\tools\OpenCover.Console.exe" -register:user -target:"..\Misc-Beta-Test\packages\NUnit.ConsoleRunner.3.9.0\tools\nunit3-console.exe" -targetargs:"..\Misc-Beta-Test\Test-FormattedNumber\bin\x64\Debug\Test-FormattedNumber.dll --trace=Debug --labels=All --where=cat==Coverage" -filter:"+[FormattedNumber*]* -[Test-FormattedNumber*]*" -output:"..\Misc-Beta-Test\Test-FormattedNumber\obj\x64\Debug\Coverage-FormattedNumber-Debug_coverage.xml"
"..\Misc-Beta-Test\packages\OpenCover.4.6.519\tools\OpenCover.Console.exe" -register:user -target:"..\Misc-Beta-Test\packages\NUnit.ConsoleRunner.3.9.0\tools\nunit3-console.exe" -targetargs:"..\Misc-Beta-Test\Test-FormattedNumber\bin\x64\Release\Test-FormattedNumber.dll --trace=Debug --labels=All --where=cat==Coverage" -filter:"+[FormattedNumber*]* -[Test-FormattedNumber*]*" -output:"..\Misc-Beta-Test\Test-FormattedNumber\obj\x64\Release\Coverage-FormattedNumber-Release_coverage.xml"

:upload
if exist ..\Misc-Beta-Test\Test-FormattedNumber\obj\x64\Debug\Coverage-FormattedNumber-Debug_coverage.xml ..\Misc-Beta-Test\packages\Codecov.1.1.1\tools\codecov -f "..\Misc-Beta-Test\Test-FormattedNumber\obj\x64\Debug\Coverage-FormattedNumber-Debug_coverage.xml" -t "e34a62fe-de27-408f-98db-a2700117d634"
if exist ..\Misc-Beta-Test\Test-FormattedNumber\obj\x64\Release\Coverage-FormattedNumber-Release_coverage.xml ..\Misc-Beta-Test\packages\Codecov.1.1.1\tools\codecov -f "..\Misc-Beta-Test\Test-FormattedNumber\obj\x64\Release\Coverage-FormattedNumber-Release_coverage.xml" -t "e34a62fe-de27-408f-98db-a2700117d634"
goto end

:error_console1
echo ERROR: OpenCover.Console not found.
goto end

:error_console2
echo ERROR: nunit3-console not found.
goto end

:error_not_built
echo ERROR: Test-FormattedNumber.dll not built (both Debug and Release are required).
goto end

:end
