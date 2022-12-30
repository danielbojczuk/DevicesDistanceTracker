import { Arn, ArnFormat } from 'aws-cdk-lib';
import { Effect, PolicyDocument, PolicyStatement, Role, ServicePrincipal } from 'aws-cdk-lib/aws-iam';
import { ServiceStack } from '../serviceStack';
export const lambdaExecutionRole = (scope: ServiceStack, deviceDynamoDbTableName: string, deviceDynamoDbStreamArn: string, deviceDistanceTrackerFunctionName:string): Role => 
   new Role(scope, "LambdaFunctionExecutionRole", {
      assumedBy: new ServicePrincipal("lambda.amazonaws.com"),
      roleName: "LambdaFunctionExecutionRole",
      inlinePolicies: {
        "IoTRuleExecutionRolePolicy": new PolicyDocument({
          statements:  [
            new PolicyStatement({
              effect: Effect.ALLOW,
              actions: ["logs:CreateLogGroup"],
              resources: [
                Arn.format({
                  service: "logs",
                  resource: "*",
                }, scope)
              ]
            }),
            new PolicyStatement({
              effect: Effect.ALLOW,
              actions: ["logs:CreateLogStream", "logs:PutLogEvents"],
              resources: [
                Arn.format({
                  service: "logs",
                  resource: "log-group",
                  resourceName: `/aws/lambda/${deviceDistanceTrackerFunctionName}`,
                  arnFormat: ArnFormat.COLON_RESOURCE_NAME,
                }, scope),
                 Arn.format({
                  service: "logs",
                  resource: "log-group",
                  resourceName: `/aws/lambda/${deviceDistanceTrackerFunctionName}:*`,
                  arnFormat: ArnFormat.COLON_RESOURCE_NAME,
                }, scope)
              ]
            }),
            new PolicyStatement({
              effect: Effect.ALLOW,
              actions: ["dynamodb:GetItem"],
              resources: [
                Arn.format({
                  service: "dynamodb",
                  resource: "table",
                  resourceName: deviceDynamoDbTableName
                }, scope),
                Arn.format({
                  service: "dynamodb",
                  resource: "table",
                  resourceName: "DevicePairs",
                }, scope)
              ]
            }),
            new PolicyStatement({
              effect: Effect.ALLOW,
              actions: ["sns:Publish"],
              resources: [
                Arn.format({
                  service: "sns",
                  resource: "PostNL_Topic",
                }, scope)
              ]
            }),
            new PolicyStatement({
              effect: Effect.ALLOW,
              actions: ["dynamodb:DescribeStream", "dynamodb:GetRecords", "dynamodb:GetShardIterator"],
              resources: [
               deviceDynamoDbStreamArn,
              ]
            }),
            new PolicyStatement({
              effect: Effect.ALLOW,
              actions: ["dynamodb:ListStreams"],
              resources: [
               "*"
              ]
            })
          ]
        }),
      }
    });
