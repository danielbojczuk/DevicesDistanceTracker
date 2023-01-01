import * as cdk from 'aws-cdk-lib';
import { Table } from 'aws-cdk-lib/aws-dynamodb';
import { Role } from 'aws-cdk-lib/aws-iam';
import { CfnTopicRule } from 'aws-cdk-lib/aws-iot';
import { CfnEventSourceMapping, Function } from 'aws-cdk-lib/aws-lambda';
import { Construct } from 'constructs';
import { deviceDynamoDbTable } from './dynamoDB/deviceDynamoDbTable';
import { distanceTrackerLambdaFunction } from './lambda/distanceTrackerLambdaFunction';
import { eventSourceMapping } from './dynamoDB/eventSourceMapping';
import { iotExecutionRole } from './iam/iotExecutionRole';
import { iotRule } from './iot/iotRule';
import { lambdaExecutionRole } from './iam/lambdaExecutionRole';


export class DistanceTrackerStack extends cdk.Stack {
  private devicesDynamoDbTable: Table;
  private iotExecutionRole: Role;
  private lambdaExecutionRole: Role;
  private iotRule: CfnTopicRule;
  private distanceTrackerLambdaFunction: Function;
  private dynamoDbEventSource: CfnEventSourceMapping;

  constructor(scope: Construct, id: string, props?: cdk.StackProps) {
    super(scope, id, props);

    const lambdaFunctionName = "DistanceTrackerFunction";

    this.devicesDynamoDbTable = deviceDynamoDbTable(this);

    this.iotExecutionRole = iotExecutionRole(this,this.devicesDynamoDbTable.tableName);

    this.lambdaExecutionRole = lambdaExecutionRole(this,this.devicesDynamoDbTable.tableName, this.devicesDynamoDbTable.tableStreamArn as string, lambdaFunctionName);

    this.iotRule = iotRule(this, this.devicesDynamoDbTable.tableName, this.iotExecutionRole.roleArn);

    this.distanceTrackerLambdaFunction = distanceTrackerLambdaFunction(this,lambdaFunctionName,this.lambdaExecutionRole, this.devicesDynamoDbTable.tableName);

    this.dynamoDbEventSource = eventSourceMapping(this,lambdaFunctionName,this.devicesDynamoDbTable.tableStreamArn as string);
  }
}
