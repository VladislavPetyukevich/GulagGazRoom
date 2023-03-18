FROM alpine/git as clone

ARG GIT_BRANCH
ARG GIT_REPOSITORY

WORKDIR /app

RUN git clone --branch main https://github.com/VladislavPetyukevich/GulagGazRoom.git

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /app

# Copy everything
COPY --from=clone /app/GulagGazRoom/Backend .

COPY pre-production.json /app/Interview.Backend/
COPY dev.json /app/Interview.Backend/

RUN cp -rf /app/Interview.Backend/dev.json /app/Interview.Backend/appsettings.Development.json
RUN cp -rf /app/Interview.Backend/pre-production.json /app/Interview.Backend/appsettings.PreProduction.json 

RUN dotnet restore

RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:7.0

WORKDIR /app

COPY --from=build-env /app/out .

EXPOSE 5043

ENTRYPOINT ["dotnet", "Interview.Backend.dll"]