namespace TheOmenDen.CrowsAgainstHumility.Services;

internal sealed class TokenStateService
{
    private readonly TokenProvider _tokenProvider;

    public TokenStateService(TokenProvider tokenProvider)
    {
        _tokenProvider = tokenProvider;
    }

    public String GetUserName() => _tokenProvider.Username;

    public Boolean UserHasAccess() => _tokenProvider.IsAuthenticated;
}
