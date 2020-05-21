FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
RUN apt-get update \
   && apt-get install -y --allow-unauthenticated \
       libc6-dev \
       libgdiplus \
       libx11-dev \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ImageProcessingWebApi.csproj ./
RUN dotnet restore "./ImageProcessingWebApi.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "ImageProcessingWebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ImageProcessingWebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ImageProcessingWebApi.dll"]
