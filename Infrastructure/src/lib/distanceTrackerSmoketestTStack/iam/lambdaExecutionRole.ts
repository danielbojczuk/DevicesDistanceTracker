import { Arn, ArnFormat } from 'aws-cdk-lib';
import { Effect, PolicyDocument, PolicyStatement, Role, ServicePrincipal } from 'aws-cdk-lib/aws-iam';
import { DistanceTrackerSmoketestStack } from '../distanceTrackerSmoketestStack';
export const lambdaExecutionRole = (scope: DistanceTrackerSmoketestStack, deviceDistanceTrackerSmoketestFunctionName:string, vehicleId:string, queueArn: string): Role => 
   new Role(scope, "LambdaFunctionSmoketestExecutionRole", {
      assumedBy: new ServicePrincipal("lambda.amazonaws.com"),
      roleName: "LambdaFunctionSmoketestExecutionRole",
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
                  resourceName: `/aws/lambda/${deviceDistanceTrackerSmoketestFunctionName}`,
                  arnFormat: ArnFormat.COLON_RESOURCE_NAME,
                }, scope),
                 Arn.format({
                  service: "logs",
                  resource: "log-group",
                  resourceName: `/aws/lambda/${deviceDistanceTrackerSmoketestFunctionName}:*`,
                  arnFormat: ArnFormat.COLON_RESOURCE_NAME,
                }, scope)
              ]
            }),
            new PolicyStatement({
              effect: Effect.ALLOW,
              actions: ["iot:Connect"],
              resources: [
                Arn.format({
                  service: "iot",
                  resource: "client",
                  resourceName: "*"
                }, scope),
              ]
            }),
            new PolicyStatement({
              effect: Effect.ALLOW,
              actions: ["iot:Publish"],
              resources: [
                Arn.format({
                  service: "iot",
                  resource: "topic",
                  resourceName: `v1/gps/vehicle/${vehicleId}`
                }, scope)
              ]
            }),
            new PolicyStatement({
              effect: Effect.ALLOW,
              actions: ["sqs:ReceiveMessage", "sqs:DeleteMessage"],
              resources: [
               queueArn
              ]
            }),
          ]
        }),
      }
    });
