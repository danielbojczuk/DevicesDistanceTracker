using Amazon.CDK;
using Amazon.CDK.AWS.DynamoDB;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.IoT;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AWS.Lambda.EventSources;
using Constructs;
using static Amazon.CDK.AWS.IoT.CfnTopicRule;

namespace Infrastructure
{
  public class DevicesDistanceTrackerInfrastructure : Stack
  {
    internal DevicesDistanceTrackerInfrastructure(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
    {
      var LambdaFunctionName = "DistanceTrackerFunction";
      var DynamoDbTable = new Table(this, "DevicesLocationTable", new TableProps
      {
        TableName = "DevicesLocation",
        PartitionKey = new Attribute { Name = "macAddress", Type = AttributeType.STRING },
        BillingMode = BillingMode.PROVISIONED,
        ReadCapacity = 5,
        WriteCapacity = 5,
        RemovalPolicy = RemovalPolicy.RETAIN,
        Stream = StreamViewType.NEW_AND_OLD_IMAGES,
      });

      foreach (var tag in this.GetDefaultTags())
      {
        Amazon.CDK.Tags.Of(DynamoDbTable).Add(tag.Key, tag.Value);
      }

      var IoTRuleExecutionRole = new Role(this, "IotRuleExecutionRole", new RoleProps
      {
        AssumedBy = new ServicePrincipal("iot.amazonaws.com"),
        RoleName = "IotRuleExecutionRole",
      });
      IoTRuleExecutionRole.AddToPolicy(new PolicyStatement(this.GetDynamoDBPutPolicyDocument(DynamoDbTable.TableName)));
      foreach (var tag in this.GetDefaultTags())
      {
        Amazon.CDK.Tags.Of(IoTRuleExecutionRole).Add(tag.Key, tag.Value);
      }

      var LambdaFunctionExecutionRole = new Role(this, "LambdaFunctionExecutionRole", new RoleProps
      {
        AssumedBy = new ServicePrincipal("lambda.amazonaws.com"),
        RoleName = "LambdaFunctionExecutionRole",
      });
      foreach (var Policy in this.GetLambdaFunctionPolicyDocument(LambdaFunctionName, DynamoDbTable.TableName, DynamoDbTable.TableStreamArn))
      {
        LambdaFunctionExecutionRole.AddToPolicy(new PolicyStatement(Policy));
      }
      foreach (var tag in this.GetDefaultTags())
      {
        Amazon.CDK.Tags.Of(LambdaFunctionExecutionRole).Add(tag.Key, tag.Value);
      }

      var IotRule = new CfnTopicRule(this, "DevicesLocationRule", new CfnTopicRuleProps
      {
        TopicRulePayload = new TopicRulePayloadProperty
        {
          Actions = new[] { new ActionProperty {
            DynamoDBv2 = new DynamoDBv2ActionProperty {
                PutItem = new PutItemInputProperty {
                    TableName = DynamoDbTable.TableName
                },
                RoleArn = IoTRuleExecutionRole.RoleArn
            }} },
          Sql = "SELECT CASE isUndefined(vehicleId) WHEN true then handheldId else vehicleId end AS macAddress, CASE isUndefined(vehicleId) WHEN true then 'handheld' else 'vehicle' end AS type, * FROM 'v1/gps/+/#'",
          AwsIotSqlVersion = "2016-03-23",
          Description = "IoT Rule to receice Data from Devices and send them to DynamoDB table.",
        },
        RuleName = "DevicesLocationRule",
        Tags = this.GetDefaultTags()
      });

      var DistanceTrackerLambdaFunction = new Function(this, "DistanceTrackerFunction", new FunctionProps
      {
        Runtime = Runtime.DOTNET_6,
        MemorySize = 128,
        Architecture = Architecture.ARM_64,
        Handler = "DistanceTrackerFunction::DistanceTrackerFunction.Function::FunctionHandler",
        Code = Code.FromAsset("deploy/DistanceTrackerFunction"),
        Role = LambdaFunctionExecutionRole,
        FunctionName = LambdaFunctionName,
        Timeout = Duration.Seconds(10),
      });
      foreach (var tag in this.GetDefaultTags())
      {
        Amazon.CDK.Tags.Of(DistanceTrackerLambdaFunction).Add(tag.Key, tag.Value);
      }
      DistanceTrackerLambdaFunction.AddEventSource(new DynamoEventSource(DynamoDbTable, new DynamoEventSourceProps { StartingPosition = StartingPosition.LATEST, BatchSize = 100, MaxBatchingWindow = Duration.Seconds(15), RetryAttempts = 0 }));
    }
    private CfnTag[] GetDefaultTags()
    {
      return new CfnTag[] { new CfnTag { Key = "Project", Value = "DevicesDistanceTracker" }, new CfnTag { Key = "Version", Value = "1.0.0" } };
    }

    private PolicyStatementProps GetDynamoDBPutPolicyDocument(string TableName)
    {
      return new PolicyStatementProps
      {
        Effect = Effect.ALLOW,
        Actions = new[] { "dynamodb:PutItem" },
        Resources = new[]
                {
                    Arn.Format(new ArnComponents
                    {
                        Service = "dynamodb",
                        Resource = "table",
                        ResourceName = TableName
                    },this)
                }
      };
    }
    private PolicyStatementProps[] GetLambdaFunctionPolicyDocument(string lambdaName, string devicesTableName, string streamArn)
    {
      return new PolicyStatementProps[]
      {
       new PolicyStatementProps {
          Effect = Effect.ALLOW,
          Actions = new[] { "logs:CreateLogGroup" },
          Resources = new[]
                  {
                      Arn.Format(new ArnComponents
                      {
                          Service = "logs",
                          Resource = "*"
                      },this)
                  }
        },
        new PolicyStatementProps {
          Effect = Effect.ALLOW,
          Actions = new[] { "logs:CreateLogStream", "logs:PutLogEvents" },
          Resources = new[]
                  {
                      Arn.Format(new ArnComponents
                      {
                          Service = "logs",
                          Resource = "log-group",
                          ResourceName = $"/aws/lambda/{lambdaName}",
                          ArnFormat = ArnFormat.COLON_RESOURCE_NAME,
                      },this),
                      Arn.Format(new ArnComponents
                      {
                          Service = "logs",
                          Resource = "log-group",
                          ResourceName = $"/aws/lambda/{lambdaName}:*",
                          ArnFormat = ArnFormat.COLON_RESOURCE_NAME,
                      },this)
                  }
        },
        new PolicyStatementProps {
          Effect = Effect.ALLOW,
          Actions = new[] { "dynamodb:GetItem" },
          Resources = new[]
                  {
                      Arn.Format(new ArnComponents
                      {
                          Service = "dynamodb",
                          Resource = "table",
                          ResourceName = $"{devicesTableName}"
                      },this),
                      Arn.Format(new ArnComponents
                      {
                          Service = "dynamodb",
                          Resource = "table",
                          ResourceName = "DevicePairs"
                      },this)
                  },
        },
        new PolicyStatementProps {
          Effect = Effect.ALLOW,
          Actions = new[] { "sns:Publish" },
          Resources = new[]
                  {
                      Arn.Format(new ArnComponents
                      {
                          Service = "sns",
                          Resource = "PostNL_Topic",
                      },this)
                  }
        },
        new PolicyStatementProps {
          Effect = Effect.ALLOW,
          Actions = new[] { "dynamodb:DescribeStream", "dynamodb:GetRecords", "dynamodb:GetShardIterator" },
          Resources = new[]
                  {
                      streamArn
                  }
        },
        new PolicyStatementProps {
          Effect = Effect.ALLOW,
          Actions = new[] { "dynamodb:ListStreams" },
          Resources = new[]
                  {
                      "*"
                  }
        },
      };
    }
  }
}
