
#set nmake_loc="C:\Program Files\Visual Studio\2017\Community\VC\Tools\MSVC\14.16.27023\bin\Hostx64\x64\nmake.exe"
#set nmake_loc="C:\VisualStudio\2017\Community\VC\Tools\MSVC\14.16.27023\bin\Hostx64\x64\nmake.exe"
#set nmake_loc="C:\VisualStudio\2017\Community\VC\Tools\MSVC\14.16.27023\bin\Hostx64\x64\nmake.exe"
#set nmake_loc="C:\Program Files\Microsoft Visual Studio\2022\Community\VC\Tools\MSVC\14.30.30705\bin\Hostx64\x64\nmake.exe"
set nmake_loc="C:\Program Files\Microsoft Visual Studio\2022\Community\SDK\ScopeCppSDK\vc15\VC\bin\nmake.exe"

%nmake_loc% /f scripts\NMakefile SolutionDir="%CD%"

