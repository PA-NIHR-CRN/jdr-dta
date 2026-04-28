FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base

USER root
RUN apt-get update \
    && apt-get install -y --no-install-recommends \
        python3 \
        python3-venv \
        python3-pip \
    && python3 -m venv /opt/carrot-venv \
    && /opt/carrot-venv/bin/pip install --no-cache-dir carrot-transform \
    && rm -rf /var/lib/apt/lists/*
    
ENV PATH="/opt/carrot-venv/bin:${PATH}"

WORKDIR /src
COPY CarrotCDM/ /app/CarrotCdm
RUN chown -R $APP_UID:$APP_UID /app

USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY . .
WORKDIR Nihr.Jdr.Dta.Job
RUN --mount=type=secret,id=nuget-credentials-pa-nihr-crn,env=NuGetPackageSourceCredentials_pa_nihr_crn \
dotnet publish -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Nihr.Jdr.Dta.Job.dll"]
