import { AnyPrincipal, Effect, PolicyStatement } from 'aws-cdk-lib/aws-iam';
import { Queue, QueuePolicy, QueuePolicyProps } from 'aws-cdk-lib/aws-sqs';
import { DistanceTrackerSmoketestStack } from '../distanceTrackerSmoketestStack';

export const distanceTrackerSmoketestQueue = (scope: DistanceTrackerSmoketestStack, snsTopicArn:string): Queue => {
  const queue = new Queue(scope, "DistanceTrackerSmoketestQueue", {
    queueName: "DistanceTrackerSmoketestQueue",
    
  })

  const policyStatement = new PolicyStatement({
    effect: Effect.ALLOW,
    actions: ["SQS:SendMessage"],
    principals: [new AnyPrincipal()],
    resources: [
      queue.queueArn
    ],
    conditions: {
      "StringLike" : {"aws:SourceArn": snsTopicArn},
    }
  });

  queue.addToResourcePolicy(policyStatement);
  return queue;
}