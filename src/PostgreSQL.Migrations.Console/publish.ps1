& "dotnet" publish -c Release -r win-x86 -p:PublishSingleFile=true --self-contained true
Compress-Archive -Path bin\Release\net7.0\win-x86\publish\dbmigrations.exe -DestinationPath win-x86.zip
& "dotnet" publish -c Release -r win-x64 -p:PublishSingleFile=true --self-contained true
Compress-Archive -Path bin\Release\net7.0\win-x64\publish\dbmigrations.exe -DestinationPath win-x64.zip
& "dotnet" publish -c Release -r win-arm -p:PublishSingleFile=true --self-contained true
Compress-Archive -Path bin\Release\net7.0\win-arm\publish\dbmigrations.exe -DestinationPath win-arm.zip
& "dotnet" publish -c Release -r win-arm64 -p:PublishSingleFile=true --self-contained true
Compress-Archive -Path bin\Release\net7.0\win-arm64\publish\dbmigrations.exe -DestinationPath win-arm64.zip
& "dotnet" publish -c Release -r osx-x64 -p:PublishSingleFile=true --self-contained true
Compress-Archive -Path bin\Release\net7.0\osx-x64\publish\dbmigrations -DestinationPath osx-x64.zip
& "dotnet" publish -c Release -r osx-arm64 -p:PublishSingleFile=true --self-contained true
Compress-Archive -Path bin\Release\net7.0\osx-arm64\publish\dbmigrations -DestinationPath osx-arm64.zip
& "dotnet" publish -c Release -r linux-x64 -p:PublishSingleFile=true --self-contained true
Compress-Archive -Path bin\Release\net7.0\linux-x64\publish\dbmigrations -DestinationPath linux-x64.zip
& "dotnet" publish -c Release -r linux-arm -p:PublishSingleFile=true --self-contained true
Compress-Archive -Path bin\Release\net7.0\linux-arm\publish\dbmigrations -DestinationPath linux-arm.zip
& "dotnet" publish -c Release -r linux-arm64 -p:PublishSingleFile=true --self-contained true
Compress-Archive -Path bin\Release\net7.0\linux-arm64\publish\dbmigrations -DestinationPath linux-arm64.zip