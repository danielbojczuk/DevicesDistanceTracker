using Amazon.CDK;
using Amazon.CDK.AWS.DynamoDB;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.IoT;
using Constructs;
using static Amazon.CDK.AWS.IoT.CfnTopicRule;

namespace Infrastructure
{
  public class DevicesDistanceTrackerInfrastructure : Stack
  {
    private const string IoTRuleDynamoDBRoleAssumePolicy = @"{
       'Version': '2012-10-17',
       'Statement': [
           {
           'Effect':'Allow',
               'Principal': {
                 'Service': 'iot.amazonaws.com'
                },
           'Action': 'sts:AssumeRole'
           }
        ]
    }";
    internal DevicesDistanceTrackerInfrastructure(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
    {
      var DynamoDbTable = new Table(this, "DevicesLocationTable", new TableProps
      {
        TableName = "DevicesLocation",
        PartitionKey = new Attribute { Name = "macAddress", Type = AttributeType.STRING },
        BillingMode = BillingMode.PROVISIONED,
        ReadCapacity = 5,
        WriteCapacity = 5
      });

      foreach (var tag in this.GetDefaultTags())
      {
        Amazon.CDK.Tags.Of(DynamoDbTable).Add(tag.Key, tag.Value);
      }

      var IoTRuleDynamoDBRole = new Role(this, "InstanceRole", new RoleProps
      {
        AssumedBy = new ServicePrincipal("iot.amazonaws.com"),
        RoleName = "IoTRuleDynamoDBRole",
      });
      IoTRuleDynamoDBRole.AddToPolicy(new PolicyStatement(this.GetDynamoDBPutPolicyDocument(DynamoDbTable.TableName)));
      foreach (var tag in this.GetDefaultTags())
      {
        Amazon.CDK.Tags.Of(IoTRuleDynamoDBRole).Add(tag.Key, tag.Value);
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
                RoleArn = IoTRuleDynamoDBRole.RoleArn
            }} },
          Sql = "SELECT CASE isUndefined(vehicleId) WHEN true then handheldId else vehicleId end AS macAddress, CASE isUndefined(vehicleId) WHEN true then 'handheld' else 'vehicle' end AS type, * FROM 'v1/gps/+/#'",
          AwsIotSqlVersion = "2016-03-23",
          Description = "IoT Rule to receice Data from Devices and send them to DynamoDB table.",
        },
        RuleName = "DevicesLocationRule",
        Tags = this.GetDefaultTags()
      });

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
  }
}
