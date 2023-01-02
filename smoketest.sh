#!/bin/bash
aws lambda invoke --function-name DevicesDistanceTrackerSmoketest ./test.out > output.txt
grep FunctionError output.txt
if [ "$?" -eq 0 ]; then
  exit -1
fi
