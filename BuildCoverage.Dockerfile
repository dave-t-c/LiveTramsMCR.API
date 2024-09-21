FROM --platform=linux/amd64 mcr.microsoft.com/dotnet/sdk:6.0 AS build
COPY . /src

# Install dotnet-coverage
ENV PATH="${PATH}:/root/.dotnet/tools"
RUN dotnet tool install --global dotnet-coverage

# Install dotnet-coverage dependencies
RUN apt-get update && apt install libxml2

WORKDIR /src
ENTRYPOINT dotnet-coverage collect "dotnet test" -f xml -o "/Output/coverage.xml"