$server = Start-Process -PassThru -NoNewWindow -FilePath "dotnet" -ArgumentList "run --project Sybon.Auth"
Start-sleep -Seconds 5
Set-Location Sybon.Auth.Client
java -jar .\swagger-codegen-cli-3.0.0-20170904.171256-3.jar generate -l csharp --additional-properties targetFramework=v5.0,netCoreProjectFile=true,packageName=Sybon.Auth.Client -i http://localhost:5000/swagger/v1/swagger.json -o sybon.auth.client
Stop-Process -Id $server.Id