import { DistanceTrackerSmoketestStack } from '../distanceTrackerSmoketestStack';
import { Architecture, Code, Function, Runtime, Tracing } from 'aws-cdk-lib/aws-lambda';
import { Duration } from 'aws-cdk-lib';
import { IRole } from 'aws-cdk-lib/aws-iam';
import * as ssm from 'aws-cdk-lib/aws-ssm';
import { Bucket } from 'aws-cdk-lib/aws-s3';

export const distanceTrackerSmoketestLambdaFunction = (scope: DistanceTrackerSmoketestStack, deviceTrackerSmoketestLambdaFunctionName: string, lambdaExecutionRole: IRole, queueUrl: string ): Function => {

   const notificationTopic = ssm.StringParameter.valueForStringParameter(
      scope, '/DevicesDistanceTracker/NotificationSNSTopic');
   const iotEndpoint = ssm.StringParameter.valueForStringParameter(
      scope, '/DevicesDistanceTracker/IotEndpoint');
   const expectedNotification = ssm.StringParameter.valueForStringParameter(
      scope, '/DevicesDistanceTracker/ExpectedNotification');
   const vehicleId = ssm.StringParameter.valueForStringParameter(
      scope, '/DevicesDistanceTracker/VehicleId');

   return new Function(scope, "DistanceTrackerSmoketestLambdaFunction", {
      runtime: Runtime.DOTNET_6,
      memorySize: 256,
      architecture: Architecture.ARM_64,
      handler: "DevicesDistanceTrackerSmoketest::DevicesDistanceTrackerSmoketest.Function::FunctionHandler",
      code: Code.fromAsset("deploy/DistanceTrackerFunctionSmoketest"),
      role: lambdaExecutionRole,
      functionName: deviceTrackerSmoketestLambdaFunctionName,
      timeout: Duration.seconds(180),
      tracing: Tracing.ACTIVE,
      environment: {
         "EXPECTED_NOTIFICATION": expectedNotification,
         "IOT_ENDPOINT": iotEndpoint,
         "QUEUE_URL": queueUrl,
         "VEHICLE_ID": vehicleId
      }
    })
   }