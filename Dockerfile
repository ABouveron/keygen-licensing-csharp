FROM mcr.microsoft.com/dotnet/sdk:7.0 as build-env
WORKDIR /src
COPY *.csproj .
RUN dotnet restore
COPY . .
ENTRYPOINT ["dotnet", "run"]