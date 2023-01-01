import * as cdk from 'aws-cdk-lib';
import { Queue } from 'aws-cdk-lib/aws-sqs';
import { Construct } from 'constructs';
import {distanceTrackerSmoketestQueue} from "./sqs/distanceTrackerSmoketestQueue";
import {distanceTrackerSmoketestSubscription} from "./snsSubscription/distanceTrackerSmoketestSubscription"
import { CfnSubscription } from 'aws-cdk-lib/aws-sns';
import * as ssm from 'aws-cdk-lib/aws-ssm';
import { lambdaExecutionRole } from './iam/lambdaExecutionRole';
import { Role } from 'aws-cdk-lib/aws-iam';
import { distanceTrackerSmoketestLambdaFunction } from './lambda/distanceTrackerSmoketestLambdaFunction';
import { Function } from 'aws-cdk-lib/aws-lambda';

export class DistanceTrackerSmoketestStack extends cdk.Stack {

  private distanceTrackerSmoketestQueue: Queue;
  private distanceTrackerSmoketestSubscription: CfnSubscription;
  private distanceTrackerSmoketestExecRole: Role;
  private distanceTrackerSmoketestFunction: Function;
 
  constructor(scope: Construct, id: string, props?: cdk.StackProps) {
    super(scope, id, props);
    
    const notificationTopic = ssm.StringParameter.valueFromLookup(
      this, '/DevicesDistanceTracker/NotificationSNSTopic');

    const vehicleId = ssm.StringParameter.valueFromLookup(
      this, '/DevicesDistanceTrackerSmoketest/VehicleID');
      
    const lambdaFunctionName = "DevicesDistanceTrackerSmoketest"

    this.distanceTrackerSmoketestQueue =  distanceTrackerSmoketestQueue(this, notificationTopic);

    this.distanceTrackerSmoketestSubscription =  distanceTrackerSmoketestSubscription(this,notificationTopic,this.distanceTrackerSmoketestQueue.queueArn);    
    
    this.distanceTrackerSmoketestExecRole = lambdaExecutionRole(this,lambdaFunctionName, vehicleId, this.distanceTrackerSmoketestQueue.queueArn);

    this.distanceTrackerSmoketestFunction = distanceTrackerSmoketestLambdaFunction(this,lambdaFunctionName, this.distanceTrackerSmoketestExecRole, this.distanceTrackerSmoketestQueue.queueUrl)
  }
}
