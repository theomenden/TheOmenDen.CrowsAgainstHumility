namespace TheOmenDen.CrowsAgainstHumility.Shared
{
    public partial class GameCreationDisplay : ComponentBase
    {
        private string _playerName = String.Empty;

        private string _playerToAdd = String.Empty;

        private List<String> _playerNames = new (10);

        private Task AddPlayerToList()
        {
            _playerNames.Add(_playerToAdd);

            return Task.CompletedTask;
        }
    }
}
