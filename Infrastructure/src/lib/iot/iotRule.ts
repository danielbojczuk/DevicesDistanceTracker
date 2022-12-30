import { CfnTopicRule } from 'aws-cdk-lib/aws-iot';
import { ServiceStack } from '../serviceStack';
export const iotRule = (scope: ServiceStack, deviceDynamoDbTableName: string, iotRuleExecutionRoleArn: string):CfnTopicRule => 
   new CfnTopicRule(scope, "DevicesLocationRule", {
      ruleName: "DevicesLocationRule",
      topicRulePayload: {
        actions: [
          {
            dynamoDBv2: {
              putItem: {
                tableName: deviceDynamoDbTableName,
              },
              roleArn: iotRuleExecutionRoleArn,
            }
          }
        ],
        sql: "SELECT CASE isUndefined(vehicleId) WHEN true then handheldId else vehicleId end AS macAddress, CASE isUndefined(vehicleId) WHEN true then 'handheld' else 'vehicle' end AS type, * FROM 'v1/gps/+/#'",
        awsIotSqlVersion: "2016-03-23",
      }
    });