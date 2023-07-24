FROM alpine@sha256:25fad2a32ad1f6f510e528448ae1ec69a28ef81916a004d3629874104f8a7f70 as builder
RUN apk add dotnet7-sdk
WORKDIR /src
COPY . .
RUN dotnet build

FROM alpine@sha256:25fad2a32ad1f6f510e528448ae1ec69a28ef81916a004d3629874104f8a7f70 as runner
ENV COMPlus_EnableDiagnostics=0
RUN apk add --no-cache dotnet7-runtime curl &&  \
    addgroup -S nonrootgroup && \
    adduser -S nonroot -G nonrootgroup
USER nonroot
WORKDIR /home/nonroot/app/src
COPY --from=builder /src .
RUN find / -perm +6000 -type f -exec chmod a-s {} \; || true
HEALTHCHECK --interval=5s --timeout=3s --start-period=5s --retries=3 CMD curl --fail http://localhost:5000/health || exit 1
ENTRYPOINT ["dotnet", "bin/Debug/net7.0/example-csharp-licensing-Docker.dll", "examples/license.lic", "examples/machine.lic", "198e9fe586114844f6a4eaca5069b41a7ed43fb5a2df84892b69826d64573e39"]