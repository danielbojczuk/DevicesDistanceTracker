import { RemovalPolicy } from 'aws-cdk-lib';
import { AttributeType, BillingMode, StreamViewType, Table } from 'aws-cdk-lib/aws-dynamodb';
import { DistanceTrackerStack } from '../distanceTrackerStack';
export const deviceDynamoDbTable = (scope: DistanceTrackerStack): Table => 
  new Table(scope,"DevicesLocationTable", {
      tableName: "DevicesLocation",
      partitionKey: {name: "macAddress", type: AttributeType.STRING},
      billingMode: BillingMode.PROVISIONED,
      readCapacity: 5,
      writeCapacity: 5,
      removalPolicy: RemovalPolicy.RETAIN,
      stream: StreamViewType.NEW_AND_OLD_IMAGES,
  });
