using System;
using System.Text;
using Steamworks;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.Diamond.AccessProvider.Steam;

public class SteamLoginAccessProvider : ILoginAccessProvider
{
	private string _steamUserName;

	private ulong _steamId;

	private PlatformInitParams _initParams;

	private uint _appId;

	private string _appTicket;

	private int AppId => Convert.ToInt32(_appId);

	void ILoginAccessProvider.Initialize(string preferredUserName, PlatformInitParams initParams)
	{
		if (SteamAPI.Init() && Packsize.Test())
		{
			_appId = SteamUtils.GetAppID().m_AppId;
			_steamId = SteamUser.GetSteamID().m_SteamID;
			_steamUserName = SteamFriends.GetPersonaName();
			_initParams = initParams;
			byte[] array = new byte[1042];
			SteamAPICall_t hAPICall = SteamUser.RequestEncryptedAppTicket(Encoding.UTF8.GetBytes(""), array.Length);
			CallResult<EncryptedAppTicketResponse_t>.Create(EncryptedAppTicketResponseReceived).Set(hAPICall);
			while (_appTicket == null)
			{
				SteamAPI.RunCallbacks();
			}
		}
	}

	string ILoginAccessProvider.GetUserName()
	{
		return _steamUserName;
	}

	PlayerId ILoginAccessProvider.GetPlayerId()
	{
		return new PlayerId(2, 0uL, _steamId);
	}

	AccessObjectResult ILoginAccessProvider.CreateAccessObject()
	{
		if (!SteamAPI.IsSteamRunning())
		{
			return AccessObjectResult.CreateFailed(new TextObject("{=uunRVBPN}Steam is not running."));
		}
		byte[] array = new byte[1024];
		if (SteamUser.GetAuthSessionTicket(array, 1024, out var _) == HAuthTicket.Invalid)
		{
			return AccessObjectResult.CreateFailed(new TextObject("{=XSU8Bbbb}Invalid Steam session."));
		}
		StringBuilder stringBuilder = new StringBuilder(array.Length * 2);
		byte[] array2 = array;
		foreach (byte b in array2)
		{
			stringBuilder.AppendFormat("{0:x2}", b);
		}
		string externalAccessToken = stringBuilder.ToString();
		return AccessObjectResult.CreateSuccess(new SteamAccessObject(_steamUserName, externalAccessToken, AppId, _appTicket));
	}

	private void EncryptedAppTicketResponseReceived(EncryptedAppTicketResponse_t response, bool bIOFailure)
	{
		byte[] array = new byte[2048];
		SteamUser.GetEncryptedAppTicket(array, 2048, out var pcbTicket);
		byte[] array2 = new byte[pcbTicket];
		Array.Copy(array, array2, pcbTicket);
		_appTicket = BitConverter.ToString(array2).Replace("-", "");
	}
}
