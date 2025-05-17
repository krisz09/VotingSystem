using Microsoft.AspNetCore.Components;

public class FakeNavigationManager : NavigationManager
{
    public FakeNavigationManager()
    {
        Initialize("http://localhost/", "http://localhost/");
    }

    protected override void NavigateToCore(string uri, bool forceLoad)
    {
        Uri = ToAbsoluteUri(uri).ToString();
    }
}
