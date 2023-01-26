FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Interview.Backend/Interview.Backend.csproj", "Interview.Backend/"]
RUN dotnet restore "Interview.Backend/Interview.Backend.csproj"
COPY . .
WORKDIR "/src/Interview.Backend"
RUN dotnet build "Interview.Backend.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Interview.Backend.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Interview.Backend.dll"]
