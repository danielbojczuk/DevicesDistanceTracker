#!/bin/bash
ROOT_DIR=$(pwd)

export SERVICE_VERSION=$(cat version)

echo "### Deploying Service ###"
cdk deploy --require-approval never --all