#!/bin/bash
set -e
ROOT_DIR=$(pwd)

rm -rf ./deploy/

echo "### Building DistanceTrackerFunction ###"
cd $ROOT_DIR/DistanceTrackerFunction/src
dotnet restore
dotnet build

# echo "### Testing DistanceTrackerFunction ###"
cd $ROOT_DIR/DistanceTrackerFunction/test
dotnet test

echo "### Publishing DistanceTrackerFunction ###"
cd $ROOT_DIR/DistanceTrackerFunction/src
dotnet publish -c Release -r linux-arm64 --no-self-contained -o ../../deploy/DistanceTrackerFunction 

echo "### Building Infrastrucutre ###"
cd $ROOT_DIR/Infrastructure/src
rm -rf .node_modules
npm ci
