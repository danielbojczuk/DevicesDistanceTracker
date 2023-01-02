#!/bin/bash
set -e
ROOT_DIR=$(pwd)
rm -rf ./deploy/

echo "### Publishing DistanceTrackerFunction ###"
cd $ROOT_DIR/DistanceTrackerFunction/src
dotnet publish -c Release -r linux-arm64 --no-self-contained -o ../../deploy/DistanceTrackerFunction 

echo "### Publishing DistanceTrackerFunctionSmoketest ###"
cd $ROOT_DIR/DistanceTrackerFunctionSmoketest/src
dotnet publish -c Release -r linux-arm64 --no-self-contained -o ../../deploy/DistanceTrackerFunctionSmoketest

cd $ROOT_DIR/deploy/DistanceTrackerFunction

cd $ROOT_DIR/deploy/DistanceTrackerFunctionSmoketest
