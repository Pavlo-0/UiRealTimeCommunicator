   
**local run first
dotnet pack -c Release
dotnet tool install --global --add-source ./bin/Release UiRtc.TypeScriptGenerator
dotnet-uirtc -p path/to/project.csproj -o path/to/output

**update

dotnet pack -c Release
dotnet tool install -g UiRtc.TypeScriptGenerator --add-source ./bin/Release --force
dotnet-uirtc -p path/to/project.csproj -o path/to/output

or

dotnet pack -c Release
dotnet tool uninstall -g UiRtc.TypeScriptGenerator
dotnet tool install --global --add-source ./bin/Release UiRtc.TypeScriptGenerator
dotnet-uirtc -p path/to/project.csproj -o path/to/output

**note
Version: 
dotnet-uirtc --version

**debug
dotnet run -- Generator -p  ..\Examples\BroadcastStream\BroadcastStream.csproj   -o ..\Examples\app-frontend\src\communication\contract.ts 



# install
# TypedSignalR.Client.TypeScript CLI (dotnet tool) requires .NET 7, but your app TFM can use .NET 6, etc.
$ dotnet tool install --global UiRealTimeCommunicator-develop
$ dotnet tsrts help

# update
$ dotnet tool update --global UiRtc.TypeScriptGenerator