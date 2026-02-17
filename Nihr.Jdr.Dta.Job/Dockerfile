FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
USER $APP_UID
WORKDIR /app

ARG AWS_SECRET
ARG AWS_REGION
ARG SECRETS_ENABLED

ENV AwsSecretsManagerSettings__SecretName=${AWS_SECRET}
ENV AwsSecretsManagerSettings__Enabled=${SECRETS_ENABLED}
ENV AwsSecretsManagerSettings__Region=${AWS_REGION}
ENV ASPNETCORE_Environment=Production

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG NUGET_PACKAGE_USERNAME
ARG NUGET_PACKAGE_TOKEN
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Nihr.Jdr.Dta.Job/Nihr.Jdr.Dta.Job.csproj", "Nihr.Jdr.Dta.Job/"]
RUN dotnet nuget add source --username $NUGET_PACKAGE_USERNAME --password $NUGET_PACKAGE_TOKEN --store-password-in-clear-text --name github "https://nuget.pkg.github.com/pa-nihr-crn/index.json"
RUN dotnet restore "Nihr.Jdr.Dta.Job/Nihr.Jdr.Dta.Job.csproj"
COPY . .
WORKDIR "/src/Nihr.Jdr.Dta.Job"
RUN dotnet build "./Nihr.Jdr.Dta.Job.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Nihr.Jdr.Dta.Job.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Nihr.Jdr.Dta.Job.dll"]
