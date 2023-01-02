#!/bin/bash
set -e
ROOT_DIR=$(pwd)

rm -rf ./deploy/

echo "### Building DistanceTrackerFunction ###"
cd $ROOT_DIR/DistanceTrackerFunction/src
dotnet restore
dotnet build

echo "### Publishing DistanceTrackerFunctionSmoketest ###"
cd $ROOT_DIR/DistanceTrackerFunctionSmoketest/src
dotnet restore
dotnet build

echo "### Testing DistanceTrackerFunction ###"
cd $ROOT_DIR/DistanceTrackerFunction/test
dotnet test

echo "### Building Infrastrucutre ###"
cd $ROOT_DIR/Infrastructure/src
rm -rf .node_modules
npm ci

