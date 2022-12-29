using DistanceTrackerFunction.Domain.Devices;
using DistanceTrackerFunction.Domain.Tracker;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using System.Text.Json;

namespace DistanceTrackerFunction.Infrastructure.Repositories;

public class NotificationRepository : INotificationRepository
{
  private IAmazonSimpleNotificationService snsClient { get; init; }
  private string snsTopic { get; init; }
  public NotificationRepository(IAmazonSimpleNotificationService snsClient, string topic)
  {
    this.snsClient = snsClient;
    this.snsTopic = topic;
  }

  public async Task SendNotification(Notification notification)
  {
    await this.snsClient.PublishAsync(this.snsTopic, JsonSerializer.Serialize(notification));
  }
}