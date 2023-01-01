import { CfnEventSourceMapping, StartingPosition } from 'aws-cdk-lib/aws-lambda';
import { DistanceTrackerStack } from '../distanceTrackerStack';
export const eventSourceMapping = (scope: DistanceTrackerStack, deviceTrackerFunctionName:string, deviceDynamoDbTableStreamArn: string): CfnEventSourceMapping => {
  const resource = new CfnEventSourceMapping(scope, "DynamoDbEventSource", {
      startingPosition: StartingPosition.LATEST,
      batchSize: 100,
      maximumBatchingWindowInSeconds: 10,
      maximumRetryAttempts: 0,
      functionName: deviceTrackerFunctionName,
      eventSourceArn: deviceDynamoDbTableStreamArn
    });

    resource.addPropertyOverride("FilterCriteria", {
    Filters: [
      {
        Pattern:
          JSON.stringify({
            dynamodb: {
              NewImage: {
                type: {S: ['vehicle']},
              },
            },
            eventName: ['MODIFY'],
          }),
      },
    ],
  });
  return resource;    
}