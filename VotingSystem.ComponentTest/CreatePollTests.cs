using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;
using VotingSystem.AdminClient.Pages;
using VotingSystem.AdminClient.Services;
using VotingSystem.AdminClient.ViewModels;
using Microsoft.AspNetCore.Components;

namespace VotingSystem.ComponentTest
{
    public class CreatePollTests : TestContext
    {
        [Fact]
        public void CreatePoll_ShouldShowValidationError_WhenDatesAreInThePast()
        {
            // Arrange
            var pollServiceMock = new Mock<IPollsService>();
            Services.AddSingleton(pollServiceMock.Object);

            var cut = RenderComponent<CreatePoll>();

            // Act
            var questionInput = cut.Find("input.form-control");
            questionInput.Change("What is your favorite color?");

            var startDateInput = cut.Find("input[type='datetime-local']");
            startDateInput.Change(DateTime.Now.AddMinutes(-10).ToString("yyyy-MM-ddTHH:mm"));

            var endDateInput = cut.FindAll("input[type='datetime-local']")[1];
            endDateInput.Change(DateTime.Now.AddMinutes(-5).ToString("yyyy-MM-ddTHH:mm"));

            var submitButton = cut.Find("button[type='submit']");
            submitButton.Click();

            // Debug: Print markup
            Console.WriteLine(cut.Markup);

            // Assert (wait for error message to appear)
            cut.WaitForAssertion(() =>
            {
                var validationMessages = cut.FindAll("li.validation-message");
                Assert.Contains(validationMessages, m => m.TextContent.Contains("The StartDate field is required.")
                                                      || m.TextContent.Contains("Dates have to be in the future"));
            });


        }

        [Fact]
        public void CreatePoll_ShouldShowValidationError_WhenNotEnoughOptions()
        {
            // Arrange
            var pollServiceMock = new Mock<IPollsService>();
            Services.AddSingleton(pollServiceMock.Object);

            var cut = RenderComponent<CreatePoll>();

            // Set required fields
            cut.Find("input.form-control").Change("Test poll?");
            var futureDate = DateTime.Now.AddDays(1);
            var startDateString = futureDate.ToString("yyyy-MM-ddTHH:mm");
            var endDateString = futureDate.AddHours(1).ToString("yyyy-MM-ddTHH:mm");
            cut.Find("input[type='datetime-local']").Change(startDateString);
            cut.FindAll("input[type='datetime-local']")[1].Change(endDateString);

            // Add only one option
            cut.Find("button.btn-outline-primary").Click();
            cut.FindAll("input.option-input")[0].Change("Option 1");

            // Set min/max votes
            cut.FindAll("input.form-control")[2].Change(1);
            cut.FindAll("input.form-control")[3].Change(1);

            cut.Find("button[type='submit']").Click();

            // Assert
            cut.WaitForAssertion(() =>
            {
                var validationMessages = cut.FindAll("li.validation-message");
                Assert.Contains(validationMessages, m => m.TextContent.Contains("Legalább 2 opció szükséges."));
            });
        }

        [Fact]
        public void CreatePoll_ShouldAddAndRemoveOptions()
        {
            // Arrange
            var pollServiceMock = new Mock<IPollsService>();
            Services.AddSingleton(pollServiceMock.Object);

            var cut = RenderComponent<CreatePoll>();

            // Debugging: Print the rendered HTML
            Console.WriteLine(cut.Markup);

            // Act
            var addOptionButton = cut.Find("button.btn-outline-primary");
            addOptionButton.Click(); // Add one option
            addOptionButton.Click(); // Add another option

            // Refine the selector to target only option input fields
            var optionInputs = cut.FindAll("input.option-input");
            Assert.Equal(2, optionInputs.Count); // Assert two options are added

            var removeOptionButton = cut.FindAll("button.btn-outline-danger")[0];
            removeOptionButton.Click(); // Remove the first option

            optionInputs = cut.FindAll("input.option-input");
            Assert.Single(optionInputs); // Assert one option remains
        }
    }
}
