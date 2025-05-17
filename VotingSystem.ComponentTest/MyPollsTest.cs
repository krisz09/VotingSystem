using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;
using VotingSystem.AdminClient.Pages;
using VotingSystem.AdminClient.Services;
using VotingSystem.AdminClient.ViewModels;

namespace VotingSystem.ComponentTest
{
    public class MyPollsTests : TestContext
    {
        

        [Fact]
        public void MyPolls_ShouldShowErrorMessage_WhenErrorOccurs()
        {
            // Arrange
            var pollsServiceMock = new Mock<IPollsService>();
            pollsServiceMock.Setup(service => service.GetPollsCreatedByUserAsync())
                .ThrowsAsync(new Exception("Failed to load polls"));

            Services.AddSingleton(pollsServiceMock.Object);

            // Act
            var cut = RenderComponent<MyPolls>();

            // Assert
            var errorMessage = cut.WaitForElement("div.alert.alert-danger");
            Assert.NotNull(errorMessage);
            Assert.Equal("Failed to load your polls.", errorMessage.TextContent);
        }

        [Fact]
        public void MyPolls_ShouldShowNoPollsMessage_WhenNoPollsExist()
        {
            // Arrange
            var pollsServiceMock = new Mock<IPollsService>();
            pollsServiceMock.Setup(service => service.GetPollsCreatedByUserAsync())
                .ReturnsAsync(new List<PollViewModel>()); // Simulate no polls

            Services.AddSingleton(pollsServiceMock.Object);

            // Act
            var cut = RenderComponent<MyPolls>();

            // Assert
            var noPollsMessage = cut.WaitForElement("div.alert.alert-info.text-center");
            Assert.NotNull(noPollsMessage);
            Assert.Equal("You have not created any polls yet.", noPollsMessage.TextContent);
        }

        [Fact]
        public void MyPolls_ShouldRenderPolls_WhenPollsExist()
        {
            // Arrange
            var pollsServiceMock = new Mock<IPollsService>();
            var mockPolls = new List<PollViewModel>
    {
        new PollViewModel
        {
            Id = 1,
            Question = "What is your favorite color?",
            StartDate = new DateTime(2025, 5, 1, 0, 0, 0),
            EndDate = new DateTime(2025, 5, 10, 0, 0, 0),
            Options = new List<PollOptionViewModel>
            {
                new PollOptionViewModel { OptionText = "Red" },
                new PollOptionViewModel { OptionText = "Blue" }
            },
            Voters = new List<VoterViewModel>
            {
                new VoterViewModel { Email = "voter1@example.com" }
            }
        }
    };

            pollsServiceMock.Setup(service => service.GetPollsCreatedByUserAsync())
                .ReturnsAsync(mockPolls);

            Services.AddSingleton(pollsServiceMock.Object);

            // Act
            var cut = RenderComponent<MyPolls>();

            // Debugging: Print the rendered HTML
            Console.WriteLine(cut.Markup);

            // Wait for the component to render the polls
            var pollCards = cut.WaitForElements("div.card");
            Assert.Single(pollCards);

            // Use QuerySelector on the first card to locate child elements
            var pollTitle = pollCards[0].QuerySelector("h5.card-title");
            Assert.NotNull(pollTitle);
            Assert.Equal("What is your favorite color?", pollTitle.TextContent);

            var pollDetails = pollCards[0].QuerySelector("p.card-text");
            Assert.NotNull(pollDetails);

            // Assert the content of poll details
            Assert.Contains("Start: 2025.05.01 02:00", pollDetails.TextContent);
            Assert.Contains("End: 2025.05.10 02:00", pollDetails.TextContent);
        }

        [Fact]
        public void MyPolls_ShouldToggleDetails_WhenShowDetailsButtonClicked()
        {
            // Arrange
            var pollsServiceMock = new Mock<IPollsService>();
            var mockPolls = new List<PollViewModel>
            {
                new PollViewModel
                {
                    Id = 1,
                    Question = "What is your favorite color?",
                    Options = new List<PollOptionViewModel>
                    {
                        new PollOptionViewModel { OptionText = "Red" },
                        new PollOptionViewModel { OptionText = "Blue" }
                    }
                }
            };

            pollsServiceMock.Setup(service => service.GetPollsCreatedByUserAsync())
                .ReturnsAsync(mockPolls);

            Services.AddSingleton(pollsServiceMock.Object);

            // Act
            var cut = RenderComponent<MyPolls>();

            // Wait for the "Show Details" button to appear
            cut.WaitForElement("button.btn-outline-primary");

            var showDetailsButton = cut.Find("button.btn-outline-primary");
            showDetailsButton.Click(); // Simulate clicking "Show Details"

            // Assert
            var details = cut.Find("ul.list-group");
            Assert.NotNull(details);
            Assert.Contains("Red", details.TextContent);
            Assert.Contains("Blue", details.TextContent);
        }
    }
}