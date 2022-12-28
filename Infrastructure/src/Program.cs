using Amazon.CDK;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure
{
  sealed class Program
  {
    public static void Main(string[] args)
    {
      var app = new App();
      new DevicesDistanceTrackerInfrastructure(app, "DevicesDistanceTrackerInfrastructure", new StackProps());
      app.Synth();
    }
  }
}
