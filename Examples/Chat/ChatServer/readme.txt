Generate typed TypeScript

Update generator to last version
dotnet tool update --global UiRealTimeCommunicator.TypeScriptGenerator

dotnet-uirtc -p ".\Examples\Chat\ChatServer\Chat.csproj" -o ".\Examples\Chat\ChatClient\src\chatHub\chat.ts"