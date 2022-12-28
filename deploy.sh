#!/bin/bash
ROOT_DIR=$(pwd)

echo "### Deploying Service ###"
cdk deploy --require-approval never