FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/SimpleApi/SimpleApi.csproj", "src/SimpleApi/"]
RUN dotnet restore "src/SimpleApi/SimpleApi.csproj"

COPY src/SimpleApi/. ./src/SimpleApi/
WORKDIR "/src/src/SimpleApi"
RUN dotnet publish "SimpleApi.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

RUN echo '#!/bin/bash\n\
    if grep -q -i "ubuntu\|microsoft" /proc/version; then\n\
    export ASPNETCORE_ENVIRONMENT="Production"\n\
    else\n\
    export ASPNETCORE_ENVIRONMENT="Development"\n\
    fi\n\
    \n\
    if [ -z "$ASPNETCORE_ENVIRONMENT_OVERRIDE" ]; then\n\
    echo "Auto-detected environment: $ASPNETCORE_ENVIRONMENT"\n\
    else\n\
    export ASPNETCORE_ENVIRONMENT=$ASPNETCORE_ENVIRONMENT_OVERRIDE\n\
    echo "Using provided environment: $ASPNETCORE_ENVIRONMENT"\n\
    fi\n\
    \n\
    exec dotnet SimpleApi.dll' > /app/entrypoint.sh \
    && chmod +x /app/entrypoint.sh

EXPOSE 8080

ENTRYPOINT ["/app/entrypoint.sh"]
