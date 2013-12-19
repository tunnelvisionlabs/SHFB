@ECHO OFF
REM This can be used to regenerate new copies of the WiX files to see if anything needs adding

"%WIX%\bin\heat.exe" dir ..\..\Data -out Temp\Data.wxs -cg Data -gg -sfrag -template fragment -dr INSTALLDIR -var var.RootFolder -indent 2
"%WIX%\bin\heat.exe" dir ..\..\Examples -out Temp\Examples.wxs -cg Examples -gg -sfrag -template fragment -dr INSTALLDIR -var var.RootFolder -indent 2
"%WIX%\bin\heat.exe" dir ..\..\Presentation -out Temp\Presentation.wxs -cg Presentation -gg -sfrag -template fragment -dr INSTALLDIR -var var.RootFolder -indent 2
"%WIX%\bin\heat.exe" dir ..\..\ProductionTools -out Temp\ProductionTools.wxs -cg ProductionTools -gg -sfrag -template fragment -dr INSTALLDIR -var var.RootFolder -indent 2
"%WIX%\bin\heat.exe" dir ..\..\ProductionTransforms -out Temp\ProductionTransforms.wxs -cg ProductionTransforms -gg -sfrag -template fragment -dr INSTALLDIR -var var.RootFolder -indent 2
"%WIX%\bin\heat.exe" dir ..\..\Schemas -out Temp\Schemas.wxs -cg Schemas -gg -sfrag -template fragment -dr INSTALLDIR -var var.RootFolder -indent 2
"%WIX%\bin\heat.exe" dir ..\..\Snippets -out Temp\Snippets.wxs -cg Snippets -gg -sfrag -template fragment -dr INSTALLDIR -var var.RootFolder -indent 2
