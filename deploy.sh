#!/bin/bash
ROOT_DIR=$(pwd)

export SERVICE_VERSION=$(cat version)

cd $ROOT_DIR/Infrastructure/src
rm -rf .node_modules
npm ci

echo "### Deploying Service ###"
cd $ROOT_DIR
cdk deploy --require-approval never --all