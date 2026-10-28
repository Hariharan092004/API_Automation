
using Microsoft.Playwright;
using NUnit.Framework;

namespace PetstoreApiAutomation;

public class PetApiTests
{
    [Test]
    public async Task CreatePet_ThenGetPet_ShouldReturnPetDetails()
    {
        using var playwright = await Playwright.CreateAsync();

        var api = await playwright.APIRequest.NewContextAsync(
            new APIRequestNewContextOptions
            {
                BaseURL = "https://petstore.swagger.io/v2/"
            });

        try
        {
            // Generate a unique pet ID
            long petId = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            // Prepare the pet data
            var pet = new
            {
                id = petId,
                name = "AutomationTestPet",
                photoUrls = new[] { "https://example.com/pet.jpg" },
                status = "available"
            };

            // Step 1: Create a pet using POST
            var createResponse = await api.PostAsync(
                "./pet",
                new APIRequestContextOptions
                {
                    DataObject = pet
                });

            // Print response details for debugging
            Console.WriteLine($"Create URL: {createResponse.Url}");
            Console.WriteLine($"Create Status: {createResponse.Status}");
            Console.WriteLine($"Create Body: {await createResponse.TextAsync()}");

            Assert.That(
                createResponse.Status,
                Is.EqualTo(200),
                "Pet creation failed.");

            // Step 2: Retrieve the created pet using GET
            var getResponse = await api.GetAsync($"./pet/{petId}");

            Console.WriteLine($"Get URL: {getResponse.Url}");
            Console.WriteLine($"Get Status: {getResponse.Status}");

            Assert.That(
                getResponse.Status,
                Is.EqualTo(200),
                "Retrieving the created pet failed.");

            // Step 3: Parse the JSON response
            var json = await getResponse.JsonAsync();

            // Step 4: Validate the pet ID
            Assert.That(
                json?.GetProperty("id").GetInt64(),
                Is.EqualTo(petId));

            // Step 5: Validate the pet name
            Assert.That(
                json?.GetProperty("name").GetString(),
                Is.EqualTo("AutomationTestPet"));

            Console.WriteLine("Pet creation and retrieval test passed.");
        }
        finally
        {
            // Release API resources
            await api.DisposeAsync();
        }
    }
}
