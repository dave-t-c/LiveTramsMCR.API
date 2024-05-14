FROM public.ecr.aws/lambda/dotnet:6 AS base

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build

COPY . /source

WORKDIR /source/LiveTramsMCR

ARG TARGETARCH

RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages \
    dotnet publish -a ${TARGETARCH/amd64/x64} --use-current-runtime --self-contained false -o /app

FROM base AS final
COPY --from=build /app ${LAMBDA_TASK_ROOT}
CMD ["LiveTramsMCR::LiveTramsMCR.LambdaEntryPoint::FunctionHandlerAsync"]