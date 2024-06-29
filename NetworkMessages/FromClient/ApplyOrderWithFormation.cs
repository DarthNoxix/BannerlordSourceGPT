using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient;

[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
public sealed class ApplyOrderWithFormation : GameNetworkMessage
{
	public OrderType OrderType { get; private set; }

	public int FormationIndex { get; private set; }

	public ApplyOrderWithFormation(OrderType orderType, int formationIndex)
	{
		OrderType = orderType;
		FormationIndex = formationIndex;
	}

	public ApplyOrderWithFormation()
	{
	}

	protected override bool OnRead()
	{
		bool bufferReadValid = true;
		OrderType = (OrderType)GameNetworkMessage.ReadIntFromPacket(CompressionMission.OrderTypeCompressionInfo, ref bufferReadValid);
		FormationIndex = GameNetworkMessage.ReadIntFromPacket(CompressionMission.FormationClassCompressionInfo, ref bufferReadValid);
		return bufferReadValid;
	}

	protected override void OnWrite()
	{
		GameNetworkMessage.WriteIntToPacket((int)OrderType, CompressionMission.OrderTypeCompressionInfo);
		GameNetworkMessage.WriteIntToPacket(FormationIndex, CompressionMission.FormationClassCompressionInfo);
	}

	protected override MultiplayerMessageFilter OnGetLogFilter()
	{
		return MultiplayerMessageFilter.Formations | MultiplayerMessageFilter.Orders;
	}

	protected override string OnGetLogFormat()
	{
		return string.Concat("Apply order: ", OrderType, ", to formation with index: ", FormationIndex);
	}
}
