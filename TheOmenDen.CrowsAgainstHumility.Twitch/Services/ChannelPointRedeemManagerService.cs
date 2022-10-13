using Microsoft.Extensions.Logging;
using TwitchLib.PubSub.Events;

namespace TheOmenDen.CrowsAgainstHumility.Twitch.Services;

internal sealed class ChannelPointRedeemManagerService
{
	private readonly ILogger<ChannelPointRedeemManagerService> _logger;

	private readonly Dictionary<String, Action<ChannelPointRedeemManagerService, OnRewardRedeemedArgs>> _channelPointsRedeemEvents;

	public ChannelPointRedeemManagerService(ILogger<ChannelPointRedeemManagerService> logger)
	{
		_logger = logger;
		_channelPointsRedeemEvents = new();
	}

	public Boolean UnregisterEvent(String eventName)
	{
		if (!_channelPointsRedeemEvents.ContainsKey(eventName))
		{
			return false;
		}

		_channelPointsRedeemEvents.Remove(eventName);

		return true;
	}

	public ValueTask<Boolean> UnregisterEventAsync(String eventName, CancellationToken cancellationToken = new()) => cancellationToken.IsCancellationRequested
			? ValueTask.FromResult(false)
			: ValueTask.FromResult(UnregisterEvent(eventName));

	public Boolean TriggerEvent(OnRewardRedeemedArgs e)
	{
		if (!_channelPointsRedeemEvents.TryGetValue(e.RewardTitle, out var action))
		{
			return false;
		}

		try
		{
			action(this, e);
			return true;
		}
		catch (Exception ex)
		{
			_logger.LogError("@{Exception}", ex);
			return false;
		}
	}

	public ValueTask<Boolean> TriggerEventAsync(OnRewardRedeemedArgs e, CancellationToken cancellationToken = new())
	{

		if (cancellationToken.IsCancellationRequested)
		{
			return ValueTask.FromResult(false);
		}

		if (!_channelPointsRedeemEvents.TryGetValue(e.RewardTitle, out var action))
		{
			return ValueTask.FromResult(false);
		}

		try
		{
			action(this, e);
			return ValueTask.FromResult(true);
		}
		catch (Exception ex)
		{
			_logger.LogError("@{Exception}", ex);
			return ValueTask.FromResult(false);
		}
	}

	public Boolean RegisterEvent(String eventName, Action<ChannelPointRedeemManagerService, OnRewardRedeemedArgs> e) =>
		!String.IsNullOrWhiteSpace(eventName)
		&& _channelPointsRedeemEvents.TryAdd(eventName, e);
}
