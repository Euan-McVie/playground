#!/bin/bash

sudo apt-get update
sudo apt-get install -y aspnetcore-runtime-8.0

dotnet new install Aspire.ProjectTemplates::9.3.1 --force

dotnet restore
