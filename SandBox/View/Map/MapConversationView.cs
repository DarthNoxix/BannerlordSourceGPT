namespace SandBox.View.Map;

public class MapConversationView : MapView
{
	public MapConversationMission ConversationMission;

	protected internal override void OnFinalize()
	{
		base.OnFinalize();
		ConversationMission.OnFinalize();
		ConversationMission = null;
	}

	protected void CreateConversationMission()
	{
		ConversationMission = new MapConversationMission();
	}
}
