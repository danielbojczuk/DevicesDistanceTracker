import { CfnSubscription } from 'aws-cdk-lib/aws-sns';
import { DistanceTrackerSmoketestStack } from '../distanceTrackerSmoketestStack';


export const distanceTrackerSmoketestSubscription = (scope: DistanceTrackerSmoketestStack, snsTopicArn: string, queueArn:string): CfnSubscription => 
  new CfnSubscription(scope, "SmoketestSNSTopicSubscription", {
    protocol: "sqs",
    topicArn: snsTopicArn,
    endpoint: queueArn,
  })
