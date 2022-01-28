FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Lazarus/Lazarus.fsproj", "Lazarus/"]
RUN dotnet restore "Lazarus/Lazarus.fsproj"
COPY . .
WORKDIR "/src/Lazarus"
RUN dotnet build "Lazarus.fsproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Lazarus.fsproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Lazarus.dll"]
