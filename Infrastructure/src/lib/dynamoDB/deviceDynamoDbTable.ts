import { RemovalPolicy } from 'aws-cdk-lib';
import { AttributeType, BillingMode, StreamViewType, Table } from 'aws-cdk-lib/aws-dynamodb';
import { ServiceStack } from '../serviceStack';
export const deviceDynamoDbTable = (scope: ServiceStack): Table => 
  new Table(scope,"DevicesLocationTable", {
      tableName: "DevicesLocation",
      partitionKey: {name: "macAddress", type: AttributeType.STRING},
      billingMode: BillingMode.PROVISIONED,
      readCapacity: 5,
      writeCapacity: 5,
      removalPolicy: RemovalPolicy.RETAIN,
      stream: StreamViewType.NEW_AND_OLD_IMAGES,
  });
