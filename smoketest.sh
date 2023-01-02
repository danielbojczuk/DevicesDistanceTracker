#!/bin/bash
set -e
aws lambda invoke --function-name DevicesDistanceTrackerSmoketest ./test.out > output.txt

if [ ! "$(grep -c FunctionError output.txt)" -eq "0" ]; then
  echo "Error on test execution. Check Lambda function logs."
  cat output.txt
  exit -1
fi
