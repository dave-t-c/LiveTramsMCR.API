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
2. [Using the API](#using-the-api)
3. [Coming Soon](#coming-soon)
4. [Project Status](#project-status)
5. [Declaration](#declaration)


## Introduction
C# ASP.NET API to retrieve live service data from TfGM Open Data API and adds more detailed stop information.

This API is used in the [LiveTramsMCR](https://github.com/dave-t-c/LiveTramsMCR) app.

## Using the API
To use the API, you can sign up through [Azure](https://livetramsmcr-apim.developer.azure-api.net/).

As the API is a side project, it will not support many requests over a short period of time and will return 429s.
If you are expecting to make heavy usage, I would reccommend self hosting a version of the project. 

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