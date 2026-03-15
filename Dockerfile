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

EXPOSE 8080

ENTRYPOINT ["dotnet", "SimpleApi.dll"]
