using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epic.OnlineServices;
using Epic.OnlineServices.Achievements;
using Epic.OnlineServices.Auth;
using Epic.OnlineServices.Connect;
using Epic.OnlineServices.Friends;
using Epic.OnlineServices.Platform;
using Epic.OnlineServices.Presence;
using Epic.OnlineServices.Stats;
using Epic.OnlineServices.UserInfo;
using Newtonsoft.Json;
using TaleWorlds.AchievementSystem;
using TaleWorlds.ActivitySystem;
using TaleWorlds.Avatar.PlayerServices;
using TaleWorlds.Diamond;
using TaleWorlds.Diamond.AccessProvider.Epic;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.PlayerServices;
using TaleWorlds.PlayerServices.Avatar;

namespace TaleWorlds.PlatformService.Epic;

public class EpicPlatformServices : IPlatformServices
{
	private class IngestStatsQueueItem
	{
		public string Name { get; set; }

		public int Value { get; set; }
	}

	private class EpicAuthErrorResponse
	{
		[JsonProperty("errorCode")]
		public string ErrorCode { get; set; }

		[JsonProperty("errorMessage")]
		public string ErrorMessage { get; set; }

		[JsonProperty("numericErrorCode")]
		public int NumericErrorCode { get; set; }

		[JsonProperty("error_description")]
		public string ErrorDescription { get; set; }

		[JsonProperty("error")]
		public string Error { get; set; }
	}

	private class EpicAuthResponse
	{
		[JsonProperty("access_token")]
		public string AccessToken { get; set; }

		[JsonProperty("expires_in")]
		public int ExpiresIn { get; set; }

		[JsonProperty("expires_at")]
		public DateTime ExpiresAt { get; set; }

		[JsonProperty("token_type")]
		public string TokenType { get; set; }

		[JsonProperty("refresh_token")]
		public string RefreshToken { get; set; }

		[JsonProperty("refresh_expires")]
		public int RefreshExpires { get; set; }

		[JsonProperty("refresh_expires_at")]
		public DateTime RefreshExpiresAt { get; set; }

		[JsonProperty("account_id")]
		public string AccountId { get; set; }

		[JsonProperty("client_id")]
		public string ClientId { get; set; }

		[JsonProperty("internal_client")]
		public bool InternalClient { get; set; }

		[JsonProperty("client_service")]
		public string ClientService { get; set; }

		[JsonProperty("displayName")]
		public string DisplayName { get; set; }

		[JsonProperty("app")]
		public string App { get; set; }

		[JsonProperty("in_app_id")]
		public string InAppId { get; set; }

		[JsonProperty("device_id")]
		public string DeviceId { get; set; }

		[JsonProperty("product_id")]
		public string ProductId { get; set; }
	}

	private EpicAccountId _epicAccountId;

	private ProductUserId _localUserId;

	private string _accessToken;

	private string _epicUserName;

	private PlatformInterface _platform;

	private PlatformInitParams _initParams;

	private EpicFriendListService _epicFriendListService;

	private IFriendListService[] _friendListServices;

	private TextObject _initFailReason;

	private ulong _refreshConnectionCallbackId;

	private ConcurrentBag<IngestStatsQueueItem> _ingestStatsQueue = new ConcurrentBag<IngestStatsQueueItem>();

	private bool _writingStats;

	private DateTime _statsLastWrittenOn = DateTime.MinValue;

	private const int MinStatsWriteInterval = 5;

	private static EpicPlatformServices Instance => PlatformServices.Instance as EpicPlatformServices;

	public string UserId
	{
		get
		{
			if (_epicAccountId == null)
			{
				return "";
			}
			return _epicAccountId.ToString();
		}
	}

	string IPlatformServices.UserDisplayName => _epicUserName;

	IReadOnlyCollection<PlayerId> IPlatformServices.BlockedUsers => new List<PlayerId>();

	bool IPlatformServices.IsPermanentMuteAvailable => true;

	private string ExchangeCode => (string)_initParams["ExchangeCode"];

	string IPlatformServices.ProviderName => "Epic";

	PlayerId IPlatformServices.PlayerId => EpicAccountIdToPlayerId(_epicAccountId);

	bool IPlatformServices.UserLoggedIn
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	public event Action<AvatarData> OnAvatarUpdated;

	public event Action<string> OnNameUpdated;

	public event Action<bool, TextObject> OnSignInStateUpdated;

	public event Action OnBlockedUserListUpdated;

	public event Action<string> OnTextEnteredFromPlatform;

	public EpicPlatformServices(PlatformInitParams initParams)
	{
		_initParams = initParams;
		AvatarServices.AddAvatarService(PlayerIdProvidedTypes.Epic, new EpicPlatformAvatarService());
		_epicFriendListService = new EpicFriendListService(this);
	}

	public bool Initialize(IFriendListService[] additionalFriendListServices)
	{
		_friendListServices = new IFriendListService[additionalFriendListServices.Length + 1];
		_friendListServices[0] = _epicFriendListService;
		for (int i = 0; i < additionalFriendListServices.Length; i++)
		{
			_friendListServices[i + 1] = additionalFriendListServices[i];
		}
		InitializeOptions options = default(InitializeOptions);
		options.ProductName = "Bannerlord";
		options.ProductVersion = "1.0";
		Result result = PlatformInterface.Initialize(ref options);
		if (result != 0)
		{
			_initFailReason = new TextObject("{=BJ1626h7}Epic platform initialization failed: {FAILREASON}.");
			_initFailReason.SetTextVariable("FAILREASON", result.ToString());
			Debug.Print("Epic PlatformInterface.Initialize Failed:" + result);
			return false;
		}
		ClientCredentials clientCredentials = default(ClientCredentials);
		clientCredentials.ClientId = "e2cf3228b2914793b9a5e5570bad92b3";
		clientCredentials.ClientSecret = "Fk5W1E6t1zExNqEUfjyNZinYrkDcDTA63sf5MfyDbQG4";
		ClientCredentials clientCredentials2 = clientCredentials;
		Options options2 = default(Options);
		options2.ProductId = "6372ed7350f34ffc9ace219dff4b9f40";
		options2.SandboxId = "aeac94c7a11048758064b82f8f8d20ed";
		options2.ClientCredentials = clientCredentials2;
		options2.IsServer = false;
		options2.DeploymentId = "e77799aa8a5143f199b2cda9937a133f";
		Options options3 = options2;
		_platform = PlatformInterface.Create(ref options3);
		AddNotifyFriendsUpdateOptions options4 = default(AddNotifyFriendsUpdateOptions);
		_platform.GetFriendsInterface().AddNotifyFriendsUpdate(ref options4, null, delegate(ref OnFriendsUpdateInfo callbackInfo)
		{
			_epicFriendListService.UserStatusChanged(EpicAccountIdToPlayerId(callbackInfo.TargetUserId));
		});
		global::Epic.OnlineServices.Auth.Credentials credentials = default(global::Epic.OnlineServices.Auth.Credentials);
		credentials.Type = LoginCredentialType.ExchangeCode;
		credentials.Token = ExchangeCode;
		global::Epic.OnlineServices.Auth.Credentials value = credentials;
		bool failed = false;
		global::Epic.OnlineServices.Auth.LoginOptions loginOptions = default(global::Epic.OnlineServices.Auth.LoginOptions);
		loginOptions.Credentials = value;
		global::Epic.OnlineServices.Auth.LoginOptions options5 = loginOptions;
		_platform.GetAuthInterface().Login(ref options5, null, delegate(ref global::Epic.OnlineServices.Auth.LoginCallbackInfo callbackInfo)
		{
			if (callbackInfo.ResultCode != 0)
			{
				failed = true;
				Debug.Print("Epic AuthInterface.Login Failed:" + callbackInfo.ResultCode);
			}
			else
			{
				EpicAccountId epicAccountId = callbackInfo.LocalUserId;
				QueryUserInfoOptions queryUserInfoOptions = default(QueryUserInfoOptions);
				queryUserInfoOptions.LocalUserId = epicAccountId;
				queryUserInfoOptions.TargetUserId = epicAccountId;
				QueryUserInfoOptions options6 = queryUserInfoOptions;
				_platform.GetUserInfoInterface().QueryUserInfo(ref options6, null, delegate(ref QueryUserInfoCallbackInfo queryCallbackInfo)
				{
					if (queryCallbackInfo.ResultCode != 0)
					{
						failed = true;
						Debug.Print("Epic UserInfoInterface.QueryUserInfo Failed:" + queryCallbackInfo.ResultCode);
					}
					else
					{
						CopyUserInfoOptions copyUserInfoOptions = default(CopyUserInfoOptions);
						copyUserInfoOptions.LocalUserId = epicAccountId;
						copyUserInfoOptions.TargetUserId = epicAccountId;
						CopyUserInfoOptions options7 = copyUserInfoOptions;
						_platform.GetUserInfoInterface().CopyUserInfo(ref options7, out var outUserInfo);
						_epicUserName = outUserInfo?.DisplayName ?? ((Utf8String)"");
						_epicAccountId = epicAccountId;
					}
				});
			}
		});
		while (_epicAccountId == null && !failed)
		{
			_platform.Tick();
		}
		if (failed)
		{
			_initFailReason = new TextObject("{=KoKdRd1u}Could not login to Epic");
			return false;
		}
		return Connect();
	}

	private void Dummy()
	{
		if (this.OnAvatarUpdated != null)
		{
			this.OnAvatarUpdated(null);
		}
		if (this.OnNameUpdated != null)
		{
			this.OnNameUpdated(null);
		}
		if (this.OnSignInStateUpdated != null)
		{
			this.OnSignInStateUpdated(arg1: false, null);
		}
		if (this.OnBlockedUserListUpdated != null)
		{
			this.OnBlockedUserListUpdated();
		}
		if (this.OnTextEnteredFromPlatform != null)
		{
			this.OnTextEnteredFromPlatform(null);
		}
	}

	private void RefreshConnection(ref AuthExpirationCallbackInfo clientData)
	{
		try
		{
			Connect();
		}
		catch (Exception ex)
		{
			Debug.Print("RefreshConnection:" + ex.Message + " " + Environment.StackTrace, 5);
		}
	}

	private bool Connect()
	{
		bool failed = false;
		CopyUserAuthTokenOptions options = default(CopyUserAuthTokenOptions);
		_platform.GetAuthInterface().CopyUserAuthToken(ref options, _epicAccountId, out var outUserAuthToken);
		if (!outUserAuthToken.HasValue)
		{
			_initFailReason = new TextObject("{=oGIdsL8h}Could not retrieve token");
			return false;
		}
		_accessToken = outUserAuthToken.Value.AccessToken;
		_platform.GetConnectInterface().RemoveNotifyAuthExpiration(_refreshConnectionCallbackId);
		global::Epic.OnlineServices.Connect.LoginOptions loginOptions = default(global::Epic.OnlineServices.Connect.LoginOptions);
		loginOptions.Credentials = new global::Epic.OnlineServices.Connect.Credentials
		{
			Token = _accessToken,
			Type = ExternalCredentialType.Epic
		};
		global::Epic.OnlineServices.Connect.LoginOptions options2 = loginOptions;
		_platform.GetConnectInterface().Login(ref options2, null, delegate(ref global::Epic.OnlineServices.Connect.LoginCallbackInfo data)
		{
			if (data.ResultCode == Result.InvalidUser)
			{
				CreateUserOptions createUserOptions = default(CreateUserOptions);
				createUserOptions.ContinuanceToken = data.ContinuanceToken;
				CreateUserOptions options4 = createUserOptions;
				_platform.GetConnectInterface().CreateUser(ref options4, null, delegate(ref CreateUserCallbackInfo res)
				{
					if (res.ResultCode != 0)
					{
						failed = true;
					}
					else
					{
						_localUserId = res.LocalUserId;
					}
				});
			}
			else if (data.ResultCode != 0)
			{
				failed = true;
			}
			else
			{
				_localUserId = data.LocalUserId;
			}
		});
		while (_localUserId == null && !failed)
		{
			_platform.Tick();
		}
		if (failed)
		{
			_initFailReason = new TextObject("{=KoKdRd1u}Could not login to Epic");
			return false;
		}
		AddNotifyAuthExpirationOptions options3 = default(AddNotifyAuthExpirationOptions);
		_refreshConnectionCallbackId = _platform.GetConnectInterface().AddNotifyAuthExpiration(ref options3, outUserAuthToken, RefreshConnection);
		QueryStats();
		QueryDefinitions();
		return true;
	}

	public void Terminate()
	{
		if (_platform != null)
		{
			_platform.Release();
			_platform = null;
			PlatformInterface.Shutdown();
		}
	}

	public void Tick(float dt)
	{
		if (_platform != null)
		{
			_platform.Tick();
			ProcessIngestStatsQueue();
		}
	}

	bool IPlatformServices.IsPlayerProfileCardAvailable(PlayerId providedId)
	{
		return false;
	}

	void IPlatformServices.ShowPlayerProfileCard(PlayerId providedId)
	{
	}

	void IPlatformServices.LoginUser()
	{
		throw new NotImplementedException();
	}

	Task<AvatarData> IPlatformServices.GetUserAvatar(PlayerId providedId)
	{
		return Task.FromResult<AvatarData>(null);
	}

	Task<bool> IPlatformServices.ShowOverlayForWebPage(string url)
	{
		return Task.FromResult(result: false);
	}

	Task<ILoginAccessProvider> IPlatformServices.CreateLobbyClientLoginProvider()
	{
		return Task.FromResult((ILoginAccessProvider)new EpicLoginAccessProvider(_platform, _epicAccountId, _epicUserName, _accessToken, _initFailReason));
	}

	PlatformInitParams IPlatformServices.GetInitParams()
	{
		return _initParams;
	}

	IAchievementService IPlatformServices.GetAchievementService()
	{
		return new EpicAchievementService(this);
	}

	IActivityService IPlatformServices.GetActivityService()
	{
		return new TestActivityService();
	}

	void IPlatformServices.CheckPrivilege(Privilege privilege, bool displayResolveUI, PrivilegeResult callback)
	{
		callback(result: true);
	}

	void IPlatformServices.CheckPermissionWithUser(Permission privilege, PlayerId targetPlayerId, PermissionResult callback)
	{
		callback(result: true);
	}

	bool IPlatformServices.RegisterPermissionChangeEvent(PlayerId targetPlayerId, Permission permission, PermissionChanged callback)
	{
		return false;
	}

	bool IPlatformServices.UnregisterPermissionChangeEvent(PlayerId targetPlayerId, Permission permission, PermissionChanged callback)
	{
		return false;
	}

	void IPlatformServices.ShowRestrictedInformation()
	{
	}

	Task<bool> IPlatformServices.VerifyString(string content)
	{
		return Task.FromResult(result: true);
	}

	void IPlatformServices.OnFocusGained()
	{
	}

	void IPlatformServices.GetPlatformId(PlayerId playerId, Action<object> callback)
	{
		callback(PlayerIdToEpicAccountId(playerId));
	}

	internal async Task<string> GetUserName(PlayerId providedId)
	{
		if (!providedId.IsValid || providedId.ProvidedType != PlayerIdProvidedTypes.Epic)
		{
			return null;
		}
		EpicAccountId targetUserId = PlayerIdToEpicAccountId(providedId);
		UserInfoData? userInfoData = await GetUserInfo(targetUserId);
		if (!userInfoData.HasValue)
		{
			return "";
		}
		return userInfoData.Value.DisplayName;
	}

	internal async Task<bool> GetUserOnlineStatus(PlayerId providedId)
	{
		EpicAccountId targetUserId = PlayerIdToEpicAccountId(providedId);
		await GetUserInfo(targetUserId);
		Info? info = await GetUserPresence(targetUserId);
		if (!info.HasValue)
		{
			return false;
		}
		return info.Value.Status == Status.Online;
	}

	internal async Task<bool> IsPlayingThisGame(PlayerId providedId)
	{
		Info? info = await GetUserPresence(PlayerIdToEpicAccountId(providedId));
		if (!info.HasValue)
		{
			return false;
		}
		return info.Value.ProductId == (Utf8String)"6372ed7350f34ffc9ace219dff4b9f40";
	}

	internal Task<PlayerId> GetUserWithName(string name)
	{
		TaskCompletionSource<PlayerId> tsc = new TaskCompletionSource<PlayerId>();
		QueryUserInfoByDisplayNameOptions queryUserInfoByDisplayNameOptions = default(QueryUserInfoByDisplayNameOptions);
		queryUserInfoByDisplayNameOptions.LocalUserId = _epicAccountId;
		queryUserInfoByDisplayNameOptions.DisplayName = name;
		QueryUserInfoByDisplayNameOptions options = queryUserInfoByDisplayNameOptions;
		_platform.GetUserInfoInterface().QueryUserInfoByDisplayName(ref options, null, delegate(ref QueryUserInfoByDisplayNameCallbackInfo callbackInfo)
		{
			if (callbackInfo.ResultCode == Result.Success)
			{
				PlayerId result = EpicAccountIdToPlayerId(callbackInfo.TargetUserId);
				tsc.SetResult(result);
				return;
			}
			throw new Exception("Could not retrieve player from EOS");
		});
		return tsc.Task;
	}

	internal IEnumerable<PlayerId> GetAllFriends()
	{
		List<PlayerId> friends = new List<PlayerId>();
		bool? success = null;
		QueryFriendsOptions queryFriendsOptions = default(QueryFriendsOptions);
		queryFriendsOptions.LocalUserId = _epicAccountId;
		QueryFriendsOptions options = queryFriendsOptions;
		_platform.GetFriendsInterface().QueryFriends(ref options, null, delegate(ref QueryFriendsCallbackInfo callbackInfo)
		{
			if (callbackInfo.ResultCode == Result.Success)
			{
				GetFriendsCountOptions getFriendsCountOptions = default(GetFriendsCountOptions);
				getFriendsCountOptions.LocalUserId = _epicAccountId;
				GetFriendsCountOptions options2 = getFriendsCountOptions;
				int friendsCount = _platform.GetFriendsInterface().GetFriendsCount(ref options2);
				for (int i = 0; i < friendsCount; i++)
				{
					GetFriendAtIndexOptions getFriendAtIndexOptions = default(GetFriendAtIndexOptions);
					getFriendAtIndexOptions.LocalUserId = _epicAccountId;
					getFriendAtIndexOptions.Index = i;
					GetFriendAtIndexOptions options3 = getFriendAtIndexOptions;
					EpicAccountId friendAtIndex = _platform.GetFriendsInterface().GetFriendAtIndex(ref options3);
					friends.Add(EpicAccountIdToPlayerId(friendAtIndex));
				}
				success = true;
			}
			else
			{
				success = false;
			}
		});
		while (!success.HasValue)
		{
			_platform.Tick();
			Task.Delay(5);
		}
		return friends;
	}

	public void QueryDefinitions()
	{
		AchievementsInterface achievementsInterface = _platform.GetAchievementsInterface();
		QueryDefinitionsOptions options = new QueryDefinitionsOptions
		{
			LocalUserId = _localUserId
		};
		achievementsInterface.QueryDefinitions(ref options, null, delegate
		{
		});
	}

	internal bool SetStat(string name, int value)
	{
		_ingestStatsQueue.Add(new IngestStatsQueueItem
		{
			Name = name,
			Value = value
		});
		return true;
	}

	internal Task<int> GetStat(string name)
	{
		StatsInterface statsInterface = _platform.GetStatsInterface();
		CopyStatByNameOptions options = new CopyStatByNameOptions
		{
			Name = name,
			TargetUserId = _localUserId
		};
		if (statsInterface.CopyStatByName(ref options, out var outStat) == Result.Success)
		{
			return Task.FromResult(outStat.Value.Value);
		}
		return Task.FromResult(-1);
	}

	internal Task<int[]> GetStats(string[] names)
	{
		List<int> list = new List<int>();
		foreach (string name in names)
		{
			list.Add(GetStat(name).Result);
		}
		return Task.FromResult(list.ToArray());
	}

	private void ProcessIngestStatsQueue()
	{
		if (_writingStats || !(DateTime.Now.Subtract(_statsLastWrittenOn).TotalSeconds > 5.0) || _ingestStatsQueue.Count <= 0)
		{
			return;
		}
		_statsLastWrittenOn = DateTime.Now;
		_writingStats = true;
		StatsInterface statsInterface = _platform.GetStatsInterface();
		List<IngestData> stats = new List<IngestData>();
		while (_ingestStatsQueue.Count > 0)
		{
			if (_ingestStatsQueue.TryTake(out var result))
			{
				stats.Add(new IngestData
				{
					StatName = result.Name,
					IngestAmount = result.Value
				});
			}
		}
		IngestStatOptions ingestStatOptions = default(IngestStatOptions);
		ingestStatOptions.Stats = stats.ToArray();
		ingestStatOptions.LocalUserId = _localUserId;
		ingestStatOptions.TargetUserId = _localUserId;
		IngestStatOptions options = ingestStatOptions;
		statsInterface.IngestStat(ref options, null, delegate(ref IngestStatCompleteCallbackInfo data)
		{
			if (data.ResultCode != 0)
			{
				foreach (IngestData item in stats)
				{
					_ingestStatsQueue.Add(new IngestStatsQueueItem
					{
						Name = item.StatName,
						Value = item.IngestAmount
					});
				}
			}
			QueryStats();
			_writingStats = false;
		});
	}

	private static PlayerId EpicAccountIdToPlayerId(EpicAccountId epicAccountId)
	{
		return new PlayerId(3, epicAccountId.ToString());
	}

	private static EpicAccountId PlayerIdToEpicAccountId(PlayerId playerId)
	{
		byte[] b = Enumerable.ToArray(new ArraySegment<byte>(playerId.ToByteArray(), 16, 16));
		return EpicAccountId.FromString(new Guid(b).ToString("N"));
	}

	private Task<UserInfoData?> GetUserInfo(EpicAccountId targetUserId)
	{
		TaskCompletionSource<UserInfoData?> tsc = new TaskCompletionSource<UserInfoData?>();
		QueryUserInfoOptions queryUserInfoOptions = default(QueryUserInfoOptions);
		queryUserInfoOptions.LocalUserId = _epicAccountId;
		queryUserInfoOptions.TargetUserId = targetUserId;
		QueryUserInfoOptions options = queryUserInfoOptions;
		_platform.GetUserInfoInterface().QueryUserInfo(ref options, null, delegate(ref QueryUserInfoCallbackInfo callbackInfo)
		{
			if (callbackInfo.ResultCode == Result.Success)
			{
				CopyUserInfoOptions copyUserInfoOptions = default(CopyUserInfoOptions);
				copyUserInfoOptions.LocalUserId = _epicAccountId;
				copyUserInfoOptions.TargetUserId = targetUserId;
				CopyUserInfoOptions options2 = copyUserInfoOptions;
				_platform.GetUserInfoInterface().CopyUserInfo(ref options2, out var outUserInfo);
				tsc.SetResult(outUserInfo);
			}
			else
			{
				tsc.SetResult(null);
			}
		});
		return tsc.Task;
	}

	private Task<Info?> GetUserPresence(EpicAccountId targetUserId)
	{
		TaskCompletionSource<Info?> tsc = new TaskCompletionSource<Info?>();
		QueryPresenceOptions queryPresenceOptions = default(QueryPresenceOptions);
		queryPresenceOptions.LocalUserId = _epicAccountId;
		queryPresenceOptions.TargetUserId = targetUserId;
		QueryPresenceOptions options = queryPresenceOptions;
		_platform.GetPresenceInterface().QueryPresence(ref options, null, delegate(ref QueryPresenceCallbackInfo callbackInfo)
		{
			if (callbackInfo.ResultCode == Result.Success)
			{
				HasPresenceOptions hasPresenceOptions = default(HasPresenceOptions);
				hasPresenceOptions.LocalUserId = _epicAccountId;
				hasPresenceOptions.TargetUserId = targetUserId;
				HasPresenceOptions options2 = hasPresenceOptions;
				if (_platform.GetPresenceInterface().HasPresence(ref options2))
				{
					CopyPresenceOptions copyPresenceOptions = default(CopyPresenceOptions);
					copyPresenceOptions.LocalUserId = _epicAccountId;
					copyPresenceOptions.TargetUserId = targetUserId;
					CopyPresenceOptions options3 = copyPresenceOptions;
					_platform.GetPresenceInterface().CopyPresence(ref options3, out var outPresence);
					tsc.SetResult(outPresence);
				}
				else
				{
					tsc.SetResult(null);
				}
			}
			else
			{
				tsc.SetResult(null);
			}
		});
		return tsc.Task;
	}

	private void QueryStats()
	{
		QueryStatsOptions queryStatsOptions = default(QueryStatsOptions);
		queryStatsOptions.LocalUserId = _localUserId;
		queryStatsOptions.TargetUserId = _localUserId;
		QueryStatsOptions options = queryStatsOptions;
		_platform.GetStatsInterface().QueryStats(ref options, null, delegate
		{
		});
	}

	IFriendListService[] IPlatformServices.GetFriendListServices()
	{
		return _friendListServices;
	}

	public void ShowGamepadTextInput(string descriptionText, string existingText, uint maxChars, bool isObfuscated)
	{
	}
}
