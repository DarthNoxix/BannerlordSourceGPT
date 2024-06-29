using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ModuleManager;
using TaleWorlds.MountAndBlade;
using TaleWorlds.SaveSystem;
using TaleWorlds.SaveSystem.Load;

namespace SandBox;

public static class SandBoxSaveHelper
{
	public enum SaveHelperState
	{
		Start,
		Inquiry,
		LoadGame
	}

	private readonly struct ModuleCheckResult
	{
		public readonly string ModuleName;

		public readonly ModuleCheckResultType Type;

		public ModuleCheckResult(string moduleName, ModuleCheckResultType type)
		{
			ModuleName = moduleName;
			Type = type;
		}
	}

	private static readonly ApplicationVersion SaveResetVersion = new ApplicationVersion(ApplicationVersionType.EarlyAccess, 1, 7, 0, 0);

	private static readonly TextObject _stringSpaceStringTextObject = new TextObject("{=7AFlpaem}{STR1} {STR2}");

	private static readonly TextObject _newlineTextObject = new TextObject("{=ol0rBSrb}{STR1}{newline}{STR2}");

	private static readonly TextObject _moduleMissmatchInquiryTitle = new TextObject("{=r7xdYj4q}Module Mismatch");

	private static readonly TextObject _errorTitle = new TextObject("{=oZrVNUOk}Error");

	private static readonly TextObject _saveLoadingProblemText = new TextObject("{=onLDP7mP}A problem occured while trying to load the saved game.");

	private static readonly TextObject _saveResetVersionProblemText = new TextObject("{=5hbSkbQg}This save file is from a game version that is older than e1.7.0. Please switch your game version to e1.7.0, load the save file and save the game. This will allow it to work on newer versions beyond e1.7.0.");

	private static bool _isInquiryActive;

	public static event Action<SaveHelperState> OnStateChange;

	public static void TryLoadSave(SaveGameFileInfo saveInfo, Action<LoadResult> onStartGame, Action onCancel = null)
	{
		_newlineTextObject.SetTextVariable("newline", "\n");
		SandBoxSaveHelper.OnStateChange?.Invoke(SaveHelperState.Start);
		bool flag = true;
		ApplicationVersion applicationVersion = saveInfo.MetaData.GetApplicationVersion();
		if (applicationVersion < SaveResetVersion)
		{
			InquiryData data = new InquiryData(_moduleMissmatchInquiryTitle.ToString(), _saveResetVersionProblemText.ToString(), isAffirmativeOptionShown: true, isNegativeOptionShown: false, new TextObject("{=yS7PvrTD}OK").ToString(), null, delegate
			{
				_isInquiryActive = false;
				onCancel?.Invoke();
			}, null);
			_isInquiryActive = true;
			InformationManager.ShowInquiry(data);
			SandBoxSaveHelper.OnStateChange?.Invoke(SaveHelperState.Inquiry);
			return;
		}
		List<ModuleCheckResult> list = CheckModules(saveInfo.MetaData);
		if (list.Count > 0)
		{
			IEnumerable<IGrouping<ModuleCheckResultType, ModuleCheckResult>> enumerable = from m in list
				group m by m.Type;
			string text = string.Empty;
			GameTextManager globalTextManager = Module.CurrentModule.GlobalTextManager;
			foreach (IGrouping<ModuleCheckResultType, ModuleCheckResult> item in enumerable)
			{
				_stringSpaceStringTextObject.SetTextVariable("STR1", globalTextManager.FindText("str_load_module_error", Enum.GetName(typeof(ModuleCheckResultType), item.Key)));
				_stringSpaceStringTextObject.SetTextVariable("STR2", item.ElementAt(0).ModuleName);
				text += _stringSpaceStringTextObject.ToString();
				for (int i = 1; i < item.Count(); i++)
				{
					_stringSpaceStringTextObject.SetTextVariable("STR1", text);
					_stringSpaceStringTextObject.SetTextVariable("STR2", item.ElementAt(i).ModuleName);
					text = _stringSpaceStringTextObject.ToString();
				}
				_newlineTextObject.SetTextVariable("STR1", text);
				_newlineTextObject.SetTextVariable("STR2", "");
				text = _newlineTextObject.ToString();
			}
			_newlineTextObject.SetTextVariable("STR1", text);
			_newlineTextObject.SetTextVariable("STR2", " ");
			text = _newlineTextObject.ToString();
			bool flag2 = MBSaveLoad.CurrentVersion >= applicationVersion || flag;
			if (flag2)
			{
				_newlineTextObject.SetTextVariable("STR1", text);
				_newlineTextObject.SetTextVariable("STR2", new TextObject("{=lh0so0uX}Do you want to load the saved game with different modules?"));
				text = _newlineTextObject.ToString();
			}
			InquiryData data2 = new InquiryData(_moduleMissmatchInquiryTitle.ToString(), text, flag2, isNegativeOptionShown: true, new TextObject("{=aeouhelq}Yes").ToString(), new TextObject("{=3CpNUnVl}Cancel").ToString(), delegate
			{
				_isInquiryActive = false;
				LoadGameAction(saveInfo, onStartGame, onCancel);
			}, delegate
			{
				_isInquiryActive = false;
				onCancel?.Invoke();
			});
			_isInquiryActive = true;
			InformationManager.ShowInquiry(data2);
			SandBoxSaveHelper.OnStateChange?.Invoke(SaveHelperState.Inquiry);
		}
		else
		{
			LoadGameAction(saveInfo, onStartGame, onCancel);
		}
	}

	private static List<ModuleCheckResult> CheckModules(MetaData fileMetaData)
	{
		List<ModuleInfo> moduleInfos = ModuleHelper.GetModuleInfos(Utilities.GetModulesNames());
		string[] modulesInSaveFile = fileMetaData.GetModules();
		List<ModuleCheckResult> list = new List<ModuleCheckResult>();
		string[] array = modulesInSaveFile;
		foreach (string moduleName2 in array)
		{
			if (moduleInfos.All((ModuleInfo loadedModule) => loadedModule.Name != moduleName2))
			{
				list.Add(new ModuleCheckResult(moduleName2, ModuleCheckResultType.ModuleRemoved));
			}
			else if (!fileMetaData.GetModuleVersion(moduleName2).IsSame(moduleInfos.Single((ModuleInfo loadedModule) => loadedModule.Name == moduleName2).Version, checkChangeSet: false))
			{
				list.Add(new ModuleCheckResult(moduleName2, ModuleCheckResultType.VersionMismatch));
			}
		}
		foreach (ModuleInfo item in moduleInfos.Where((ModuleInfo loadedModule) => modulesInSaveFile.All((string moduleName) => loadedModule.Name != moduleName)))
		{
			list.Add(new ModuleCheckResult(item.Name, ModuleCheckResultType.ModuleAdded));
		}
		return list;
	}

	private static void LoadGameAction(SaveGameFileInfo saveInfo, Action<LoadResult> onStartGame, Action onCancel)
	{
		SandBoxSaveHelper.OnStateChange?.Invoke(SaveHelperState.LoadGame);
		LoadResult loadResult = MBSaveLoad.LoadSaveGameData(saveInfo.Name);
		if (loadResult != null)
		{
			onStartGame?.Invoke(loadResult);
			return;
		}
		InquiryData data = new InquiryData(_errorTitle.ToString(), _saveLoadingProblemText.ToString(), isAffirmativeOptionShown: true, isNegativeOptionShown: false, new TextObject("{=WiNRdfsm}Done").ToString(), string.Empty, delegate
		{
			_isInquiryActive = false;
			onCancel?.Invoke();
		}, null);
		_isInquiryActive = true;
		InformationManager.ShowInquiry(data);
		SandBoxSaveHelper.OnStateChange?.Invoke(SaveHelperState.Inquiry);
	}
}
