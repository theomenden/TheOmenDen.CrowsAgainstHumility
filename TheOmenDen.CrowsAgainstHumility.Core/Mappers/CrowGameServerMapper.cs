using Riok.Mapperly.Abstractions;
using TheOmenDen.CrowsAgainstHumility.Core.DTO.ViewModels;
using TheOmenDen.CrowsAgainstHumility.Core.Engine.Game;

namespace TheOmenDen.CrowsAgainstHumility.Core.Transformation.Mappers;
[Mapper]
public partial class CrowGameServerMapper
{
    public partial CrowGameServerViewModel ServerToServerViewModel(CrowGameServer server);
}
