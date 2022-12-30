import * as cdk from 'aws-cdk-lib';
import { Arn, ArnFormat, Duration, RemovalPolicy } from 'aws-cdk-lib';
import { AttributeType, BillingMode, StreamViewType, Table } from 'aws-cdk-lib/aws-dynamodb';
import { Effect, PolicyDocument, PolicyStatement, Role, ServicePrincipal } from 'aws-cdk-lib/aws-iam';
import { CfnTopicRule } from 'aws-cdk-lib/aws-iot';
import { Architecture, CfnEventSourceMapping, Code, Function, Runtime, StartingPosition, Tracing } from 'aws-cdk-lib/aws-lambda';
import { DynamoEventSource } from 'aws-cdk-lib/aws-lambda-event-sources';
import { Construct } from 'constructs';


export class ServiceStack extends cdk.Stack {
  private devicesDynamoDbTable: Table;
  private iotExecutionRole: Role;
  private lambdaExecutionRole: Role;
  private iotRule: CfnTopicRule;
  private distanceTrackerLambdaFunction: Function;
  private dynamoDbEventSource: CfnEventSourceMapping;

  constructor(scope: Construct, id: string, props?: cdk.StackProps) {
    super(scope, id, props);

    const lambdaFunctionName = "DistanceTrackerFunction";

    this.devicesDynamoDbTable =  new Table(this,"DevicesLocationTable", {
      tableName: "DevicesLocation",
      partitionKey: {name: "macAddress", type: AttributeType.STRING},
      billingMode: BillingMode.PROVISIONED,
      readCapacity: 5,
      writeCapacity: 5,
      removalPolicy: RemovalPolicy.RETAIN,
      stream: StreamViewType.NEW_AND_OLD_IMAGES,
      } );

    this.iotExecutionRole = new Role(this, "IotRuleExecutionRole", {
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
                  resourceName: this.devicesDynamoDbTable.tableName,
                }, this)
              ]

            }),
          ]
        }),
      }
    });

    this.lambdaExecutionRole = new Role(this, "LambdaFunctionExecutionRole", {
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
                }, this)
              ]
            }),
            new PolicyStatement({
              effect: Effect.ALLOW,
              actions: ["logs:CreateLogStream", "logs:PutLogEvents"],
              resources: [
                Arn.format({
                  service: "logs",
                  resource: "log-group",
                  resourceName: `/aws/lambda/${lambdaFunctionName}`,
                  arnFormat: ArnFormat.COLON_RESOURCE_NAME,
                }, this),
                 Arn.format({
                  service: "logs",
                  resource: "log-group",
                  resourceName: `/aws/lambda/${lambdaFunctionName}:*`,
                  arnFormat: ArnFormat.COLON_RESOURCE_NAME,
                }, this)
              ]
            }),
            new PolicyStatement({
              effect: Effect.ALLOW,
              actions: ["dynamodb:GetItem"],
              resources: [
                Arn.format({
                  service: "dynamodb",
                  resource: "table",
                  resourceName: this.devicesDynamoDbTable.tableName,
                }, this),
                Arn.format({
                  service: "dynamodb",
                  resource: "table",
                  resourceName: "DevicePairs",
                }, this)
              ]
            }),
            new PolicyStatement({
              effect: Effect.ALLOW,
              actions: ["sns:Publish"],
              resources: [
                Arn.format({
                  service: "sns",
                  resource: "PostNL_Topic",
                }, this)
              ]
            }),
            new PolicyStatement({
              effect: Effect.ALLOW,
              actions: ["dynamodb:DescribeStream", "dynamodb:GetRecords", "dynamodb:GetShardIterator"],
              resources: [
               this.devicesDynamoDbTable.tableStreamArn as string,
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

    this.iotRule = new CfnTopicRule(this, "DevicesLocationRule", {
      ruleName: "DevicesLocationRule",
      topicRulePayload: {
        actions: [
          {
            dynamoDBv2: {
              putItem: {
                tableName: this.devicesDynamoDbTable.tableName,
              },
              roleArn: this.iotExecutionRole.roleArn,
            }
          }
        ],
        sql: "SELECT CASE isUndefined(vehicleId) WHEN true then handheldId else vehicleId end AS macAddress, CASE isUndefined(vehicleId) WHEN true then 'handheld' else 'vehicle' end AS type, * FROM 'v1/gps/+/#'",
        awsIotSqlVersion: "2016-03-23",
      }
    });

    this.distanceTrackerLambdaFunction = new Function(this, "DistanceTrackerFunction", {
      runtime: Runtime.DOTNET_6,
      memorySize: 512,
      architecture: Architecture.ARM_64,
      handler: "DistanceTrackerFunction::DistanceTrackerFunction.Function::FunctionHandler",
      code: Code.fromAsset("deploy/DistanceTrackerFunction"),
      role: this.lambdaExecutionRole,
      functionName: lambdaFunctionName,
      timeout: Duration.seconds(15),
      tracing: Tracing.ACTIVE
    })


    this.dynamoDbEventSource = new CfnEventSourceMapping(this, "DynamoDbEventSource", {
      startingPosition: StartingPosition.LATEST,
      batchSize: 100,
      maximumBatchingWindowInSeconds: 10,
      maximumRetryAttempts: 0,
      functionName: this.distanceTrackerLambdaFunction.functionName,
      eventSourceArn: this.devicesDynamoDbTable.tableStreamArn
    });
    
    this.dynamoDbEventSource.addPropertyOverride("FilterCriteria", {
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
  }
}
