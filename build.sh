#!/bin/bash
ROOT_DIR=$(pwd)

rm -rf ./deploy/

echo "### Building DistanceTrackerFunction ###"
cd $ROOT_DIR/DistanceTrackerFunction/src
dotnet restore
dotnet publish -c Release -r linux-arm64 --no-self-contained -o ../../deploy/DistanceTrackerFunction 

echo "### Building Infrastrucutre ###"
cd $ROOT_DIR/Infrastructure/src
dotnet restore
dotnet build