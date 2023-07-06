FROM alpine:latest
RUN apk add --no-cache dotnet7-sdk
WORKDIR /src
COPY *.csproj .
RUN dotnet restore
COPY . .
ENTRYPOINT ["dotnet", "run", "examples/license.lic", "examples/machine.lic", "198e9fe586114844f6a4eaca5069b41a7ed43fb5a2df84892b69826d64573e39"]