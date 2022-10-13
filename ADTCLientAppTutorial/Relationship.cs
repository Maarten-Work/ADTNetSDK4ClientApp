using Azure;
using Azure.DigitalTwins.Core;

namespace Helpers
{
    public class Relationship
    {
        public async static Task CreateRelationshipAsync(DigitalTwinsClient client, string srcId, string targetId)
        {
            var relationShip = new BasicRelationship
            {
                TargetId = targetId,
                Name = "contains"
            };

            try
            {
                string relId = $"{srcId}-contains->{targetId}";
                await client.CreateOrReplaceRelationshipAsync(srcId, relId, relationShip);
                Console.WriteLine("RelationShip created successfully!");
            }
            catch (RequestFailedException e)
            {
                Console.WriteLine($"Create relationship error: {e.Status}: {e.Message}");
            }
        }

        public async static Task ListRelationshipsAsync(DigitalTwinsClient client, string srcId)
        {
            try
            {
                var relationShips = client.GetRelationshipsAsync<BasicRelationship>(srcId);
                await foreach (var r in relationShips)
                {
                    Console.WriteLine($"{r.SourceId}:{r.Name}->{r.TargetId}");
                }
            }
            catch (RequestFailedException e)
            {
                Console.WriteLine($"List relationship error: {e.Status}: {e.Message}");
            }
        }
    }
}