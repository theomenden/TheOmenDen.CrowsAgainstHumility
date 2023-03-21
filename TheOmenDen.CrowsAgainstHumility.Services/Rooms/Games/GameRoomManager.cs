using System.Collections.Concurrent;
using TheOmenDen.CrowsAgainstHumility.Core.Enumerations;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Repositories;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Services;
using TheOmenDen.CrowsAgainstHumility.Core.Models;
using TheOmenDen.Shared.Extensions;

namespace TheOmenDen.CrowsAgainstHumility.Services.Rooms.Games;
internal sealed class GameRoomManager : IGameRoomManager
{
    #region Private Members
    private readonly IRoomStateRepository _games;
    private readonly ConcurrentDictionary<Guid, SemaphoreSlim> _roomLocks = new();
    private readonly ConcurrentDictionary<Guid, RoomStateDto> _rooms = new();
    private readonly ConcurrentDictionary<string, Guid> _connectionsToRoom = new();
    private readonly ConcurrentDictionary<string, Player> _connectionsToUser = new();
    #endregion
    #region Stateful Non-Gameplay Methods
    public Task<RoomStateDto> AddPlayerToRoomAsync(Guid roomId, Player player, string connectionId)
    {
        _connectionsToRoom.AddOrUpdate(connectionId, roomId, (_, _) => roomId);
        _connectionsToUser.AddOrUpdate(connectionId, player, (_, _) => player);

        return WithRoomLocking(roomId, () =>
        {
            var roomValue = _rooms.AddOrUpdate(roomId,
                new RoomStateDto(roomId, new[] { player }), (_, roomState) =>
                {
                    return roomState with
                    {
                        Players = roomState.Players
                            .Where(u => u.Id != player.Id)
                            .Append(player)
                            .ToArray()
                    };
                });
            return Task.FromResult(roomValue);
        });
    }

    public async Task<RoomStateDto> AddPlayerToRoomByCodeAsync(String roomCode, Player player, string connectionId)
    {
        var room = (await _games.GetRoomByCodeAsync(roomCode));

        _connectionsToRoom.AddOrUpdate(connectionId, room.Id, (_, _) => room.Id);
        _connectionsToUser.AddOrUpdate(connectionId, player, (_, _) => player);

        return await WithRoomLocking(room.Id, () =>
        {
            var roomValue = _rooms.AddOrUpdate(room.Id,
                new RoomStateDto(room.Id, new[] { player }), (_, roomState) =>
                {
                    return roomState with
                    {
                        Players = roomState.Players
                            .Where(u => u.Id != player.Id)
                            .Append(player)
                            .ToArray()
                    };
                });
            return Task.FromResult(roomValue);
        });
    }

    public Task<RoomStateDto?> DisconnectAsync(string connectionId) =>
    WithConnection(connectionId, (room, playerId) =>
    {
        var players = room.Players.Where(x => x.Id != playerId).ToArray();

        return players.Any()
            ? room with { Players = players }
            : null;
    });

    public Task<RoomStateDto?> UpdateRoomAsync(RoomOptions roomOptions, string connectionId)
        => WithConnection(connectionId, (room, userId) =>
        {
            if (!(room.Players.FirstOrDefault(p => p.Id == userId)?.GameRole == GameRoles.Facilitator))
            {
                return room;
            }

            if (roomOptions.AutoShowWhiteCards is { } autoShowWhiteCards)
            {
                room = room with { ShouldRevealCardsAutomatically = autoShowWhiteCards };
            }

            if (roomOptions.WhiteCardsShow is { } whiteCardsShow)
            {
                room = room with { ShouldShowCards = true };

                if (!whiteCardsShow && room.ShouldRevealCardsAutomatically)
                {
                    room = room with { ShouldRevealCardsAutomatically = false };
                }
            }

            if (ShouldShowWhiteCards(room))
            {
                room = room with { ShouldShowCards = true };
            }

            return room;
        });

    public Task<RoomStateDto?> UpdatePlayerAsync(PlayerOptions playerOptions, string connectionId)
        => WithConnection(connectionId, (room, playerId) =>
        {
            var players = room.Players.Select(player =>
            {
                if (!(player.Id == playerId))
                {
                    return player;
                }

                if (playerOptions.Username is { } username)
                {
                    player = player with { Username = username };
                }

                if (playerOptions.Role is { } role)
                {
                    player = player with { GameRole = role };
                }

                return player;
            })
                .ToArray();
            return room with { Players = players };
        });
    #endregion
    #region Game Play Methods
    public Task<RoomStateDto?> ResetGameBoardAsync(string connectionId)
    => WithConnection(connectionId, (room, userId) =>
        {
            var currentUser = room.Players.FirstOrDefault(x => x.Id == userId);

            if (currentUser is null)
            {
                return null;
            }

            if (!currentUser.IsCardCzar)
            {
                return room;
            }

            var players = room.Players.Select(u => u with { PlayedWhiteCard = null }).ToArray();

            return room with
            {
                Players = players,
                ShouldShowCards = false,
            };

        });

    public Task<RoomStateDto?> NextBlackCardAsync(Guid roomId, BlackCard blackCard)
        => WithinExistingRoom(roomId, room =>
         room with
         {
             Players = room.Players.Select(p =>
                     p with
                     {
                         PlayedWhiteCard = null
                     }
                     ).ToArray(),
             BlackCards = room.BlackCards.Where(b => b.Id != blackCard.Id).ToArray(),
             PlayedBlackCards = room.PlayedBlackCards.Append(blackCard),
             CurrentBlackCard = room.BlackCards.GetRandomElement(),
             ShouldShowCards = false
         });

    public Task<RoomStateDto?> DiscardedWhiteCards(Guid roomId, IEnumerable<WhiteCard> cards)
        => WithinExistingRoom(roomId, room =>
        {
            var playedWhiteCards = cards as WhiteCard[] ?? cards.ToArray();
            return room with
            {
                PlayedWhiteCards = playedWhiteCards,
                Players = room.Players.Select(p => p with { PlayedWhiteCard = null })
                    .ToArray(),
                WhiteCards = room.WhiteCards
                    .Where(wc => !playedWhiteCards.Contains(wc))
                    .ToArray(),
                ShouldShowCards = false
            };
        });
    public Task<RoomStateDto?> PlayWhiteCardAsync(WhiteCard card, string connectionId)
    => WithConnection(connectionId, (room, playerId) =>
        {
            var roomState = room with
            {
                Players = room.Players.Select(p =>
                    p.Id == playerId
                    ? p with
                    {
                        PlayedWhiteCard = room.ShouldShowCards
                        ? p.PlayedWhiteCard
                        : card
                    }
                    : p).ToArray(),
            };

            if (ShouldShowWhiteCards(roomState))
            {
                roomState = roomState with { ShouldShowCards = true };
            }

            return roomState;
        });
    public Task<GameRoles> GetNewCardTsarAsync(Guid roomId) =>
        WithRoomLocking(roomId, () =>
            _rooms.TryGetValue(roomId, out var roomState)
                 && roomState.Players.Any(p => p.IsCardCzar)
            ? Task.FromResult(GameRoles.Player)
            : Task.FromResult(GameRoles.CardTsar));
    #endregion
    #region Private Methods
    private Task<RoomStateDto?> WithConnection(string connectionId, Func<RoomStateDto, Guid, RoomStateDto?> updateRoom)
    {
        if (_connectionsToRoom.TryGetValue(connectionId, out var roomId)
            && _connectionsToUser.TryGetValue(connectionId, out var user))
        {
            return WithinExistingRoom(roomId, room => updateRoom(room, user.Id));
        }

        return Task.FromResult<RoomStateDto?>(null);
    }

    private Task<RoomStateDto?> WithinExistingRoom(Guid roomId, Func<RoomStateDto, RoomStateDto?> updateRoomState)
    {
        return WithRoomLocking(roomId, () =>
        {
            if (!_rooms.TryGetValue(roomId, out var existingRoom))
            {
                return Task.FromResult<RoomStateDto?>(null);
            }

            var updatedRoom = updateRoomState(existingRoom);

            if (updatedRoom is not null)
            {
                return !_rooms.TryUpdate(roomId, updatedRoom, existingRoom)
                    ? Task.FromResult<RoomStateDto?>(existingRoom)
                    : Task.FromResult<RoomStateDto?>(updatedRoom);
            }

            _rooms.TryRemove(roomId, out _);
            return Task.FromResult<RoomStateDto?>(null);

        });
    }

    private async Task<T> WithRoomLocking<T>(Guid roomId, Func<Task<T>> action)
    {
        var semaphore = _roomLocks.AddOrUpdate(roomId, new SemaphoreSlim(1, 1), (_, existing) => existing);

        await semaphore.WaitAsync();

        try
        {
            return await action();
        }
        finally
        {
            semaphore.Release();
        }
    }

    private static bool ShouldShowWhiteCards(RoomStateDto roomState)
    {
        if (!roomState.ShouldRevealCardsAutomatically)
        {
            return false;
        }

        var currentPlayers = roomState.CurrentPlayers;

        return currentPlayers.Any()
               && currentPlayers.All(p => p.PlayedWhiteCard is not null);
    }
    #endregion
}
