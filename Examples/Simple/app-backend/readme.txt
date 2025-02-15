Generate typed TypeScript

Update generator to last version
dotnet tool update --global UiRealTimeCommunicator.TypeScriptGenerator


dotnet-uirtc -p ".\Examples\Simple\App-backend\App-backend.csproj" -o ".\Examples\Simple\app-frontend\src\communication\contract.ts"



Start front end

npm run dev