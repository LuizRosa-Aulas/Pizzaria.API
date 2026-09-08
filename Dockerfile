FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
RUN mkdir -p /data
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["Pizzaria.API.csproj", "."]
RUN dotnet restore
COPY . .
# O .csproj e explicito de proposito: a raiz tem o Pizzaria.API.sln, e sem o
# nome do projeto o SDK publicaria a SOLUCAO (warning NETSDK1194 com -o).
RUN dotnet publish Pizzaria.API.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Pizzaria.API.dll"]
