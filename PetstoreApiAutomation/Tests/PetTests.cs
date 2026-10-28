
using NUnit.Framework;
using PetstoreApiAutomation.Helpers;

namespace PetstoreApiAutomation.Tests;

[TestFixture]
public class PetTests : ApiTestBase
{
    [Test]
    public async Task CreatePet_ThenGetPet_ShouldReturnPetDetails()
    {
        long petId = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        var pet = new
        {
            id = petId,
            name = "AutomationTestPet",
            photoUrls = new[] { "https://example.com/pet.jpg" },
            status = "available"
        };

        var createResponse = await Api.PostAsync(
            "./pet",
            new Microsoft.Playwright.APIRequestContextOptions
            {
                DataObject = pet
            });

        Assert.That(
            createResponse.Status,
            Is.EqualTo(200),
            "Pet creation failed.");

        var getResponse = await Api.GetAsync($"./pet/{petId}");

        Assert.That(
            getResponse.Status,
            Is.EqualTo(200),
            "Retrieving the created pet failed.");

        var json = await getResponse.JsonAsync();

        Assert.That(
            json?.GetProperty("id").GetInt64(),
            Is.EqualTo(petId));

        Assert.That(
            json?.GetProperty("name").GetString(),
            Is.EqualTo("AutomationTestPet"));
    }
}
