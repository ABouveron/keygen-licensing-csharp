FROM alpine:latest as builder
RUN apk add dotnet7-sdk
WORKDIR /src
COPY . .
RUN dotnet build

FROM alpine:latest as runner
RUN apk add dotnet7-runtime
WORKDIR /src
COPY --from=builder /src .
ENTRYPOINT ["dotnet", "bin/Debug/net7.0/example-csharp-licensing-Docker.dll", "examples/license.lic", "examples/machine.lic", "198e9fe586114844f6a4eaca5069b41a7ed43fb5a2df84892b69826d64573e39"]