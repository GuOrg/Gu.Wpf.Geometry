## To restore Packages
1. PM> `.paket/paket.bootstrapper.exe` only needed for downloading or updating paket.exe
2. PM> `.paket/paket.exe restore` restore packages.

## To create packages:
1. Build in release
2. PM> `.paket/paket.bootstrapper.exe` only needed for downloading paket.exe
3. PM> `.paket/paket.exe pack output .\publish symbols`
4. Packages are in the publish folder.

(5.a). PM> .paket/paket.exe push url https://nuget.org file .\publish <- requires API-key 
(5.b). PM> .paket/paket.exe push url https://nuget.gw.symbolsource.org/Public/NuGet file .\publish <- requires API-key
Docs: https://fsprojects.github.io/Paket/getting-started.html
