#!/bin/bash
dotnet publish -c Release -r linux-x64 --self-contained=true -p:PublishSingleFile=true -p:GenerateRuntimeConfigurationFiles=true -o /opt/discatsharp-support DisCatSharp.Support.csproj
