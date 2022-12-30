import { Arn } from 'aws-cdk-lib';
import { Effect, PolicyDocument, PolicyStatement, Role, ServicePrincipal } from 'aws-cdk-lib/aws-iam';
import { ServiceStack } from '../serviceStack';
export const iotExecutionRole = (scope: ServiceStack, deviceDynamoDbTableName: string):Role => 
  new Role(scope, "IotRuleExecutionRole", {
      assumedBy: new ServicePrincipal("iot.amazonaws.com"),
      roleName: "IotRuleExecutionRole",
      inlinePolicies: {
        "IoTRuleExecutionRoleDynamoDbPolicy": new PolicyDocument({
          statements:  [
            new PolicyStatement({
              effect: Effect.ALLOW,
              actions: ["dynamodb:PutItem"],
              resources: [
                Arn.format({
                  service: "dynamodb",
                  resource: "table",
                  resourceName: deviceDynamoDbTableName,
                }, scope)
              ]
            }),
          ]
        }),
      }
    });
