# Etapa 1: Build da aplicação
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# 🔹 Copia todos os arquivos da solução (.sln) e de projetos (.csproj)
COPY *.sln ./
COPY PatientCare.API/*.csproj PatientCare.API/
COPY PatientCare.Application/*.csproj PatientCare.Application/
COPY PatientCare.Domain/*.csproj PatientCare.Domain/
COPY PatientCare.Infrastructure/*.csproj PatientCare.Infrastructure/
COPY PatientCare.Tests/*.csproj PatientCare.Tests/

# 🔹 Restaura as dependências
RUN dotnet restore PatientCare.API/PatientCare.API.csproj

# 🔹 Copia o restante do código
COPY . ./

# 🔹 Compila a aplicação
RUN dotnet publish PatientCare.API/PatientCare.API.csproj -c Release -o /publish

# Etapa 2: Runtime otimizado para execução
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# 🔹 Copia os arquivos compilados
COPY --from=build /publish .

# 🔹 Define o ambiente
ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_URLS=http://+:80

# 🔹 Expor a porta da API
EXPOSE 80

# 🔹 Comando para iniciar a API
ENTRYPOINT ["dotnet", "PatientCare.API.dll"]
