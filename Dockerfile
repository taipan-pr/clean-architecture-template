#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["/src/Clean.Architecture.Template.Api/Clean.Architecture.Template.Api.csproj", "Clean.Architecture.Template.Api/"]
RUN dotnet restore "Clean.Architecture.Template.Api/Clean.Architecture.Template.Api.csproj"
COPY . .
WORKDIR "/src/src/Clean.Architecture.Template.Api"
RUN dotnet build "Clean.Architecture.Template.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Clean.Architecture.Template.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Clean.Architecture.Template.Api.dll"]
