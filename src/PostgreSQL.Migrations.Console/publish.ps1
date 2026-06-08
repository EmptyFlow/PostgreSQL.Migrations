& "dotnet" publish -c Release -r win-x86 -p:PublishSingleFile=true --self-contained true
Compress-Archive -Path bin\Release\net10.0\win-x86\publish\fmigrations.exe -DestinationPath win-x86.zip
& "dotnet" publish -c Release -r win-x64 -p:PublishSingleFile=true --self-contained true
Compress-Archive -Path bin\Release\net10.0\win-x64\publish\fmigrations.exe -DestinationPath win-x64.zip
& "dotnet" publish -c Release -r win-arm64 -p:PublishSingleFile=true --self-contained true
Compress-Archive -Path bin\Release\net10.0\win-arm64\publish\fmigrations.exe -DestinationPath win-arm64.zip
