import { DistanceTrackerStack } from '../distanceTrackerStack';
import { Architecture, Code, Function, Runtime, Tracing } from 'aws-cdk-lib/aws-lambda';
import { Duration } from 'aws-cdk-lib';
import { IRole } from 'aws-cdk-lib/aws-iam';
import * as ssm from 'aws-cdk-lib/aws-ssm';
import { Bucket } from 'aws-cdk-lib/aws-s3';

export const distanceTrackerLambdaFunction = (scope: DistanceTrackerStack, deviceTrackerLambdaFunctionName: string, lambdaExecutionRole: IRole, deviceDynamoDbTableName: string ): Function => {

   const devicePairsTable = ssm.StringParameter.valueForStringParameter(
      scope, '/DevicesDistanceTracker/DevicePairs');

   const notificationTopic = ssm.StringParameter.valueForStringParameter(
      scope, '/DevicesDistanceTracker/NotificationSNSTopic');

   return new Function(scope, "DistanceTrackerFunction", {
      runtime: Runtime.DOTNET_6,
      memorySize: 512,
      architecture: Architecture.ARM_64,
      handler: "DistanceTrackerFunction::DistanceTrackerFunction.Function::FunctionHandler",
      code: Code.fromBucket( Bucket.fromBucketArn(scope,"DeploymentBucket","arn:aws:s3:::devices-distance-tracker-deployment") ,`DistanceTrackerFunction_${process.env.SERVICE_VERSION}.zip`),
      role: lambdaExecutionRole,
      functionName: deviceTrackerLambdaFunctionName,
      timeout: Duration.seconds(15),
      tracing: Tracing.ACTIVE,
      environment: {
         "Vehicle2Handheld": devicePairsTable,
         "DevicePositionTable": deviceDynamoDbTableName,
         "NotificationSNSTopic": notificationTopic,
      }
    })
   }