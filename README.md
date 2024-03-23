# LiveTramsMCR-API

[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=dave-t-c_TfGM-API-Wrapper&metric=coverage)](https://sonarcloud.io/summary/new_code?id=dave-t-c_TfGM-API-Wrapper)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=dave-t-c_TfGM-API-Wrapper&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=dave-t-c_TfGM-API-Wrapper)
[![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=dave-t-c_TfGM-API-Wrapper&metric=code_smells)](https://sonarcloud.io/summary/new_code?id=dave-t-c_TfGM-API-Wrapper)
[![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=dave-t-c_TfGM-API-Wrapper&metric=sqale_rating)](https://sonarcloud.io/summary/new_code?id=dave-t-c_TfGM-API-Wrapper)
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=dave-t-c_TfGM-API-Wrapper&metric=security_rating)](https://sonarcloud.io/summary/new_code?id=dave-t-c_TfGM-API-Wrapper)
[![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=dave-t-c_TfGM-API-Wrapper&metric=vulnerabilities)](https://sonarcloud.io/summary/new_code?id=dave-t-c_TfGM-API-Wrapper)
[![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=dave-t-c_TfGM-API-Wrapper&metric=reliability_rating)](https://sonarcloud.io/summary/new_code?id=dave-t-c_TfGM-API-Wrapper)

## Contents
1. [Introduction](#introduction)
2. [Running locally](#running-the-system-locally)
3. [Coming Soon](#coming-soon)
4. [Project Status](#project-status)
5. [Declaration](#declaration)


## Introduction
C# ASP.NET API to retrieve live service data from TfGM Open Data API and adds more detailed stop information.

This API is used in the [LiveTramsMCR](https://github.com/dave-t-c/LiveTramsMCR) app.

## Running the system locally
To run the system locally, you will need docker and docker-compose installed.

First, add your `TfGM OcpApimSubscriptionKey` key into the `docker-compose.run-local.yaml` file.

After doing this, run `docker-compose -f docker-compose.run-local.yml up -d` from the route repo directory.

This will start the system locally on `localhost:8080`. Head to `localhost:8080/swagger/index.html` to view endpoints.

You will also be able to view any changes you make to data using
mongo express, which will be started on `localhost:8081`. This can be accessed using the default mentioned `ME_CONFIG_MONGODB_AUTH_USERNAME` username and `ME_CONFIG_MONGODB_AUTH_PASSWORD` password specified [here](https://github.com/mongo-express/mongo-express?tab=readme-ov-file#usage-docker).

## Coming soon
- Expected journey time and stops for route planning
- Ticketing prices for a given journey

## Project Goals
- Create a wrapper for the TfGM API that adds additional data
- Create a route planning system
- Gain experience using ASP.NET
- Create a project using Azure

## Project Status
The project currently features:
- Detailed stop information
- Live service data for stops ordered by destination


## Declaration

This program is in no way associated with TfGM.

The data in this application is used under the Open Government Licence v3.0

Contains Transport for Greater Manchester data

This project uses the [Geolocation](https://www.nuget.org/packages/Geolocation/1.2.1) package under the M.I.T license. A copy of this license has been included [here](./Licenses/Geolocation.md)