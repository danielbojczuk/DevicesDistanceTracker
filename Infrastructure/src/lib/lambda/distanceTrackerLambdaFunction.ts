import { ServiceStack } from '../serviceStack';
import { Architecture, Code, Function, Runtime, Tracing } from 'aws-cdk-lib/aws-lambda';
import { Duration } from 'aws-cdk-lib';
import { IRole } from 'aws-cdk-lib/aws-iam';
export const distanceTrackerLambdaFunction = (scope: ServiceStack, deviceTrackerLambdaFunctionName: string, lambdaExecutionRole: IRole ): Function => 
   new Function(scope, "DistanceTrackerFunction", {
      runtime: Runtime.DOTNET_6,
      memorySize: 512,
      architecture: Architecture.ARM_64,
      handler: "DistanceTrackerFunction::DistanceTrackerFunction.Function::FunctionHandler",
      code: Code.fromAsset("deploy/DistanceTrackerFunction"),
      role: lambdaExecutionRole,
      functionName: deviceTrackerLambdaFunctionName,
      timeout: Duration.seconds(15),
      tracing: Tracing.ACTIVE
    })