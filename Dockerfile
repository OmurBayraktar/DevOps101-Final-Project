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
    # Varsayılan olarak Development çalıştır\n\
    export ASPNETCORE_ENVIRONMENT="${ASPNETCORE_ENVIRONMENT:-Development}"\n\
    \n\
    # Override değişkeni verilmişse onu kullan\n\
    if [ -n "$ASPNETCORE_ENVIRONMENT_OVERRIDE" ]; then\n\
      export ASPNETCORE_ENVIRONMENT="$ASPNETCORE_ENVIRONMENT_OVERRIDE"\n\
      echo "Using override environment: $ASPNETCORE_ENVIRONMENT"\n\
    else\n\
      echo "Using default environment: $ASPNETCORE_ENVIRONMENT"\n\
    fi\n\
    \n\
    exec dotnet SimpleApi.dll' > /app/entrypoint.sh \
    && chmod +x /app/entrypoint.sh

EXPOSE 8080

ENTRYPOINT ["/app/entrypoint.sh"]
