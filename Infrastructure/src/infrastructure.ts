#!/usr/bin/env node
import 'source-map-support/register';
import * as cdk from 'aws-cdk-lib';
import { DistanceTrackerStack } from './lib/distanceTrackerStack/distanceTrackerStack';
import { DistanceTrackerSmoketestStack } from './lib/distanceTrackerSmoketestTStack/distanceTrackerSmoketestStack';
const app = new cdk.App();
new DistanceTrackerStack(app, 'DevicesDistanceTrackerInfrastructure', {
  env:{
  account: process.env.AWS_ACCOUNT,
  region: process.env.AWS_REGION,
 }
});
new DistanceTrackerSmoketestStack(app, 'DevicesDistanceTrackerSmoketestInfrastructure', {
 env:{
  account: process.env.AWS_ACCOUNT,
  region: process.env.AWS_REGION,
 }
});

