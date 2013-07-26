@ECHO OFF
CLS

SETLOCAL
SET OUTDIR=C:\CP\TFS05\Sandcastle\Main\ProductionTools\

DEL /Q %OUTDIR%

"%WINDIR%\Microsoft.Net\Framework\v4.0.30319\msbuild.exe" "CommandLine\CommandLine.csproj" /t:Clean;Build /p:Configuration=Release;Platform=AnyCPU;OutDir=%OUTDIR%
"%WINDIR%\Microsoft.Net\Framework\v4.0.30319\msbuild.exe" "MRefBuilder\MRefBuilder.csproj" /t:Clean;Build /p:Configuration=Release;Platform=AnyCPU;OutDir=%OUTDIR%
"%WINDIR%\Microsoft.Net\Framework\v4.0.30319\msbuild.exe" "XslTransform\XslTransform.csproj" /t:Clean;Build /p:Configuration=Release;Platform=AnyCPU;OutDir=%OUTDIR%
"%WINDIR%\Microsoft.Net\Framework\v4.0.30319\msbuild.exe" "VersionBuilder\VersionBuilder.csproj" /t:Clean;Build /p:Configuration=Release;Platform=AnyCPU;OutDir=%OUTDIR%
"%WINDIR%\Microsoft.Net\Framework\v4.0.30319\msbuild.exe" "BuildAssembler\BuildAssemblerLibrary\BuildAssemblerLibrary.csproj" /t:Clean;Build /p:Configuration=Release;Platform=AnyCPU;OutDir=%OUTDIR%
"%WINDIR%\Microsoft.Net\Framework\v4.0.30319\msbuild.exe" "BuildAssembler\BuildComponents.csproj" /t:Clean;Build /p:Configuration=Release;Platform=AnyCPU;OutDir=%OUTDIR%
"%WINDIR%\Microsoft.Net\Framework\v4.0.30319\msbuild.exe" "BuildAssembler\CopyComponents\CopyComponents.csproj" /t:Clean;Build /p:Configuration=Release;Platform=AnyCPU;OutDir=%OUTDIR%
"%WINDIR%\Microsoft.Net\Framework\v4.0.30319\msbuild.exe" "BuildAssembler\SyntaxComponents\SyntaxComponents.csproj" /t:Clean;Build /p:Configuration=Release;Platform=AnyCPU;OutDir=%OUTDIR%
"%WINDIR%\Microsoft.Net\Framework\v4.0.30319\msbuild.exe" "BuildAssembler\BuildAssembler\BuildAssembler.csproj" /t:Clean;Build /p:Configuration=Release;Platform=AnyCPU;OutDir=%OUTDIR%
"%WINDIR%\Microsoft.Net\Framework\v4.0.30319\msbuild.exe" "ChmBuilder\ChmBuilder.csproj" /t:Clean;Build /p:Configuration=Release;Platform=AnyCPU;OutDir=%OUTDIR%
"%WINDIR%\Microsoft.Net\Framework\v4.0.30319\msbuild.exe" "DBCSFix\DBCSFix.csproj" /t:Clean;Build /p:Configuration=Release;Platform=AnyCPU;OutDir=%OUTDIR%
"%WINDIR%\Microsoft.Net\Framework\v4.0.30319\msbuild.exe" "AggregateByNamespace\AggregateByNamespace.csproj" /t:Clean;Build /p:Configuration=Release;Platform=AnyCPU;OutDir=%OUTDIR%
"%WINDIR%\Microsoft.Net\Framework\v4.0.30319\msbuild.exe" "SegregateByAssembly\SegregateByAssembly.csproj" /t:Clean;Build /p:Configuration=Release;Platform=AnyCPU;OutDir=%OUTDIR%
"%WINDIR%\Microsoft.Net\Framework\v4.0.30319\msbuild.exe" "SegregateByNamespace\SegregateByNamespace.csproj" /t:Clean;Build /p:Configuration=Release;Platform=AnyCPU;OutDir=%OUTDIR%
"%WINDIR%\Microsoft.Net\Framework\v4.0.30319\msbuild.exe" "MergeXml\MergeXml.csproj" /t:Clean;Build /p:Configuration=Release;Platform=AnyCPU;OutDir=%OUTDIR%
"%WINDIR%\Microsoft.Net\Framework\v4.0.30319\msbuild.exe" "MSHCPackager\MSHCPackager.csproj" /t:Clean;Build /p:Configuration=Release;Platform=AnyCPU;OutDir=%OUTDIR%
"%WINDIR%\Microsoft.Net\Framework\v4.0.30319\msbuild.exe" "SandcastleGUI\SandcastleGUI.csproj" /t:Clean;Build /p:Configuration=Release;Platform=AnyCPU;OutDir=%OUTDIR%..\Examples\Generic\

CD %OUTDIR%..\Examples\Sandcastle

"%WINDIR%\Microsoft.Net\Framework\v4.0.30319\csc.exe" /t:library /doc:comments.xml /out:test.dll test.cs

CD %OUTDIR%..\Source

COPY scbuild.ps1 %OUTDIR%

DEL /Q %OUTDIR%\*.pdb
