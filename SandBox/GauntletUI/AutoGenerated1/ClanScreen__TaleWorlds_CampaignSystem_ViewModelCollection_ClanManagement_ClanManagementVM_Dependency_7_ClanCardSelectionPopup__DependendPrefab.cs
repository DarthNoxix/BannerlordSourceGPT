using System.ComponentModel;
using System.Numerics;
using TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.GauntletUI.GamepadNavigation;
using TaleWorlds.GauntletUI.Layout;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.GauntletUI.Widgets;

namespace SandBox.GauntletUI.AutoGenerated1;

public class ClanScreen__TaleWorlds_CampaignSystem_ViewModelCollection_ClanManagement_ClanManagementVM_Dependency_7_ClanCardSelectionPopup__DependendPrefab : Widget
{
	private Widget _widget;

	private NavigationForcedScopeCollectionTargeter _widget_0;

	private Widget _widget_1;

	private Widget _widget_2;

	private ClanScreen__TaleWorlds_CampaignSystem_ViewModelCollection_ClanManagement_ClanManagementVM_Dependency_39_Standard_PopupCloseButton__DependendPrefab _widget_2_0;

	private Widget _widget_2_1;

	private ImageWidget _widget_2_1_0;

	private ImageWidget _widget_2_1_1;

	private ImageWidget _widget_2_1_2;

	private TextWidget _widget_2_1_3;

	private RichTextWidget _widget_2_1_4;

	private ScrollablePanel _widget_2_1_5;

	private NavigationScopeTargeter _widget_2_1_5_0;

	private Widget _widget_2_1_5_1;

	private ListPanel _widget_2_1_5_1_0;

	private NavigatableListPanel _widget_2_1_5_1_0_0;

	private ScrollbarWidget _widget_2_1_5_2;

	private Widget _widget_2_1_5_2_0;

	private ImageWidget _widget_2_1_5_2_1;

	private ButtonWidget _widget_2_2;

	private ClanCardSelectionPopupVM _datasource_Root;

	private MBBindingList<ClanCardSelectionPopupItemVM> _datasource_Root_Items;

	public ClanScreen__TaleWorlds_CampaignSystem_ViewModelCollection_ClanManagement_ClanManagementVM_Dependency_7_ClanCardSelectionPopup__DependendPrefab(UIContext context)
		: base(context)
	{
	}

	public void CreateWidgets()
	{
		_widget = this;
		_widget_0 = new NavigationForcedScopeCollectionTargeter(base.Context);
		_widget.AddChild(_widget_0);
		_widget_1 = new Widget(base.Context);
		_widget.AddChild(_widget_1);
		_widget_2 = new Widget(base.Context);
		_widget.AddChild(_widget_2);
		_widget_2_0 = new ClanScreen__TaleWorlds_CampaignSystem_ViewModelCollection_ClanManagement_ClanManagementVM_Dependency_39_Standard_PopupCloseButton__DependendPrefab(base.Context);
		_widget_2.AddChild(_widget_2_0);
		_widget_2_0.CreateWidgets();
		_widget_2_1 = new Widget(base.Context);
		_widget_2.AddChild(_widget_2_1);
		_widget_2_1_0 = new ImageWidget(base.Context);
		_widget_2_1.AddChild(_widget_2_1_0);
		_widget_2_1_1 = new ImageWidget(base.Context);
		_widget_2_1.AddChild(_widget_2_1_1);
		_widget_2_1_2 = new ImageWidget(base.Context);
		_widget_2_1.AddChild(_widget_2_1_2);
		_widget_2_1_3 = new TextWidget(base.Context);
		_widget_2_1.AddChild(_widget_2_1_3);
		_widget_2_1_4 = new RichTextWidget(base.Context);
		_widget_2_1.AddChild(_widget_2_1_4);
		_widget_2_1_5 = new ScrollablePanel(base.Context);
		_widget_2_1.AddChild(_widget_2_1_5);
		_widget_2_1_5_0 = new NavigationScopeTargeter(base.Context);
		_widget_2_1_5.AddChild(_widget_2_1_5_0);
		_widget_2_1_5_1 = new Widget(base.Context);
		_widget_2_1_5.AddChild(_widget_2_1_5_1);
		_widget_2_1_5_1_0 = new ListPanel(base.Context);
		_widget_2_1_5_1.AddChild(_widget_2_1_5_1_0);
		_widget_2_1_5_1_0_0 = new NavigatableListPanel(base.Context);
		_widget_2_1_5_1_0.AddChild(_widget_2_1_5_1_0_0);
		_widget_2_1_5_2 = new ScrollbarWidget(base.Context);
		_widget_2_1_5.AddChild(_widget_2_1_5_2);
		_widget_2_1_5_2_0 = new Widget(base.Context);
		_widget_2_1_5_2.AddChild(_widget_2_1_5_2_0);
		_widget_2_1_5_2_1 = new ImageWidget(base.Context);
		_widget_2_1_5_2.AddChild(_widget_2_1_5_2_1);
		_widget_2_2 = new ButtonWidget(base.Context);
		_widget_2.AddChild(_widget_2_2);
	}

	public void SetIds()
	{
		_widget_2_0.SetIds();
		_widget_2_1_5_1.Id = "ClipRect";
		_widget_2_1_5_1_0.Id = "ItemList";
		_widget_2_1_5_2.Id = "HorizontalScrollbar";
		_widget_2_1_5_2_1.Id = "HorizontalScrollbarHandle";
	}

	public void SetAttributes()
	{
		base.WidthSizePolicy = SizePolicy.StretchToParent;
		base.HeightSizePolicy = SizePolicy.StretchToParent;
		_widget_0.CollectionID = "ClanCardSelectionPopupCollection";
		_widget_0.CollectionParent = _widget;
		_widget_1.WidthSizePolicy = SizePolicy.StretchToParent;
		_widget_1.HeightSizePolicy = SizePolicy.StretchToParent;
		_widget_1.Sprite = base.Context.SpriteData.GetSprite("BlankWhiteSquare_9");
		_widget_1.Color = new Color(0f, 0f, 0f);
		_widget_1.AlphaFactor = 0.6f;
		_widget_2.WidthSizePolicy = SizePolicy.Fixed;
		_widget_2.HeightSizePolicy = SizePolicy.Fixed;
		_widget_2.SuggestedWidth = 1370f;
		_widget_2.SuggestedHeight = 720f;
		_widget_2.VerticalAlignment = VerticalAlignment.Center;
		_widget_2.HorizontalAlignment = HorizontalAlignment.Center;
		_widget_2_0.SetAttributes();
		_widget_2_0.VerticalAlignment = VerticalAlignment.Bottom;
		_widget_2_0.PositionYOffset = 26f;
		_widget_2_1.WidthSizePolicy = SizePolicy.StretchToParent;
		_widget_2_1.HeightSizePolicy = SizePolicy.StretchToParent;
		_widget_2_1.MarginTop = 35f;
		_widget_2_1.MarginBottom = 40f;
		_widget_2_1.MarginRight = 35f;
		_widget_2_1_0.WidthSizePolicy = SizePolicy.StretchToParent;
		_widget_2_1_0.HeightSizePolicy = SizePolicy.StretchToParent;
		_widget_2_1_0.Sprite = base.Context.SpriteData.GetSprite("StdAssets\\Popup\\order_popup_canvas");
		_widget_2_1_1.WidthSizePolicy = SizePolicy.StretchToParent;
		_widget_2_1_1.HeightSizePolicy = SizePolicy.StretchToParent;
		_widget_2_1_1.Sprite = base.Context.SpriteData.GetSprite("StdAssets\\Popup\\canvas_gradient");
		_widget_2_1_2.WidthSizePolicy = SizePolicy.StretchToParent;
		_widget_2_1_2.HeightSizePolicy = SizePolicy.StretchToParent;
		_widget_2_1_2.MarginLeft = -20f;
		_widget_2_1_2.MarginTop = -20f;
		_widget_2_1_2.MarginRight = -20f;
		_widget_2_1_2.MarginBottom = -20f;
		_widget_2_1_2.Sprite = base.Context.SpriteData.GetSprite("frame_9");
		_widget_2_1_3.WidthSizePolicy = SizePolicy.CoverChildren;
		_widget_2_1_3.HeightSizePolicy = SizePolicy.Fixed;
		_widget_2_1_3.SuggestedHeight = 45f;
		_widget_2_1_3.HorizontalAlignment = HorizontalAlignment.Center;
		_widget_2_1_3.MarginTop = 10f;
		_widget_2_1_3.Brush = base.Context.GetBrush("InventoryHeaderFontBrush");
		_widget_2_1_3.Brush.FontSize = 34;
		_widget_2_1_4.WidthSizePolicy = SizePolicy.StretchToParent;
		_widget_2_1_4.HeightSizePolicy = SizePolicy.Fixed;
		_widget_2_1_4.SuggestedHeight = 40f;
		_widget_2_1_4.VerticalAlignment = VerticalAlignment.Bottom;
		_widget_2_1_4.MarginBottom = 12f;
		_widget_2_1_4.Brush = base.Context.GetBrush("ClanCardSelectionPopup.ActionResult.Text");
		_widget_2_1_5.WidthSizePolicy = SizePolicy.StretchToParent;
		_widget_2_1_5.HeightSizePolicy = SizePolicy.StretchToParent;
		_widget_2_1_5.MarginTop = 60f;
		_widget_2_1_5.MarginBottom = 60f;
		_widget_2_1_5.MarginLeft = 40f;
		_widget_2_1_5.MarginRight = 40f;
		_widget_2_1_5.HorizontalAlignment = HorizontalAlignment.Center;
		_widget_2_1_5.ClipRect = _widget_2_1_5_1;
		_widget_2_1_5.InnerPanel = _widget_2_1_5_1_0;
		_widget_2_1_5.HorizontalScrollbar = _widget_2_1_5_2;
		_widget_2_1_5.AutoHideScrollBars = true;
		_widget_2_1_5.Sprite = base.Context.SpriteData.GetSprite("scrollable_field_9");
		_widget_2_1_5.Color = new Color(1f, 1f, 1f);
		_widget_2_1_5_0.ScopeID = "ClanCardSelectionPopupScope";
		_widget_2_1_5_0.ScopeParent = _widget_2_1_5_1;
		_widget_2_1_5_0.ScopeMovements = GamepadNavigationTypes.Horizontal;
		_widget_2_1_5_1.WidthSizePolicy = SizePolicy.StretchToParent;
		_widget_2_1_5_1.HeightSizePolicy = SizePolicy.StretchToParent;
		_widget_2_1_5_1.MarginTop = 7f;
		_widget_2_1_5_1.MarginBottom = 7f;
		_widget_2_1_5_1.MarginLeft = 9f;
		_widget_2_1_5_1.MarginRight = 9f;
		_widget_2_1_5_1.ClipContents = true;
		_widget_2_1_5_1_0.WidthSizePolicy = SizePolicy.CoverChildren;
		_widget_2_1_5_1_0.HeightSizePolicy = SizePolicy.StretchToParent;
		_widget_2_1_5_1_0.StackLayout.LayoutMethod = LayoutMethod.VerticalBottomToTop;
		_widget_2_1_5_1_0_0.WidthSizePolicy = SizePolicy.CoverChildren;
		_widget_2_1_5_1_0_0.HeightSizePolicy = SizePolicy.StretchToParent;
		_widget_2_1_5_1_0_0.StackLayout.LayoutMethod = LayoutMethod.HorizontalLeftToRight;
		_widget_2_1_5_2.WidthSizePolicy = SizePolicy.StretchToParent;
		_widget_2_1_5_2.HeightSizePolicy = SizePolicy.Fixed;
		_widget_2_1_5_2.SuggestedHeight = 6f;
		_widget_2_1_5_2.HorizontalAlignment = HorizontalAlignment.Center;
		_widget_2_1_5_2.VerticalAlignment = VerticalAlignment.Bottom;
		_widget_2_1_5_2.MarginLeft = 15f;
		_widget_2_1_5_2.MarginRight = 15f;
		_widget_2_1_5_2.MarginBottom = 5f;
		_widget_2_1_5_2.AlignmentAxis = AlignmentAxis.Horizontal;
		_widget_2_1_5_2.Handle = _widget_2_1_5_2_1;
		_widget_2_1_5_2.MaxValue = 100f;
		_widget_2_1_5_2.MinValue = 0f;
		_widget_2_1_5_2_0.WidthSizePolicy = SizePolicy.Fixed;
		_widget_2_1_5_2_0.HeightSizePolicy = SizePolicy.StretchToParent;
		_widget_2_1_5_2_0.SuggestedWidth = 6f;
		_widget_2_1_5_2_0.HorizontalAlignment = HorizontalAlignment.Center;
		_widget_2_1_5_2_0.Sprite = base.Context.SpriteData.GetSprite("BlankWhiteSquare_9");
		_widget_2_1_5_2_0.AlphaFactor = 0.2f;
		_widget_2_1_5_2_0.Color = new Color(0.3529412f, 0.2509804f, 0.2f);
		_widget_2_1_5_2_1.WidthSizePolicy = SizePolicy.Fixed;
		_widget_2_1_5_2_1.HeightSizePolicy = SizePolicy.StretchToParent;
		_widget_2_1_5_2_1.SuggestedWidth = 10f;
		_widget_2_1_5_2_1.MarginTop = 1f;
		_widget_2_1_5_2_1.MarginBottom = 1f;
		_widget_2_1_5_2_1.HorizontalAlignment = HorizontalAlignment.Center;
		_widget_2_1_5_2_1.Brush = base.Context.GetBrush("FaceGen.Scrollbar.Handle");
		_widget_2_2.WidthSizePolicy = SizePolicy.Fixed;
		_widget_2_2.HeightSizePolicy = SizePolicy.Fixed;
		_widget_2_2.SuggestedWidth = 70f;
		_widget_2_2.SuggestedHeight = 70f;
		_widget_2_2.HorizontalAlignment = HorizontalAlignment.Right;
		_widget_2_2.VerticalAlignment = VerticalAlignment.Top;
		_widget_2_2.Brush = base.Context.GetBrush("Popup.CloseButton");
	}

	public void DestroyDataSource()
	{
		if (_datasource_Root == null)
		{
			return;
		}
		_widget_2_0.DestroyDataSource();
		_datasource_Root.PropertyChanged -= ViewModelPropertyChangedListenerOf_datasource_Root;
		_datasource_Root.PropertyChangedWithValue -= ViewModelPropertyChangedWithValueListenerOf_datasource_Root;
		_datasource_Root.PropertyChangedWithBoolValue -= ViewModelPropertyChangedWithBoolValueListenerOf_datasource_Root;
		_datasource_Root.PropertyChangedWithIntValue -= ViewModelPropertyChangedWithIntValueListenerOf_datasource_Root;
		_datasource_Root.PropertyChangedWithFloatValue -= ViewModelPropertyChangedWithFloatValueListenerOf_datasource_Root;
		_datasource_Root.PropertyChangedWithUIntValue -= ViewModelPropertyChangedWithUIntValueListenerOf_datasource_Root;
		_datasource_Root.PropertyChangedWithColorValue -= ViewModelPropertyChangedWithColorValueListenerOf_datasource_Root;
		_datasource_Root.PropertyChangedWithDoubleValue -= ViewModelPropertyChangedWithDoubleValueListenerOf_datasource_Root;
		_datasource_Root.PropertyChangedWithVec2Value -= ViewModelPropertyChangedWithVec2ValueListenerOf_datasource_Root;
		_widget.PropertyChanged -= PropertyChangedListenerOf_widget;
		_widget.boolPropertyChanged -= boolPropertyChangedListenerOf_widget;
		_widget.floatPropertyChanged -= floatPropertyChangedListenerOf_widget;
		_widget.Vec2PropertyChanged -= Vec2PropertyChangedListenerOf_widget;
		_widget.Vector2PropertyChanged -= Vector2PropertyChangedListenerOf_widget;
		_widget.doublePropertyChanged -= doublePropertyChangedListenerOf_widget;
		_widget.intPropertyChanged -= intPropertyChangedListenerOf_widget;
		_widget.uintPropertyChanged -= uintPropertyChangedListenerOf_widget;
		_widget.ColorPropertyChanged -= ColorPropertyChangedListenerOf_widget;
		_widget_2_1_3.PropertyChanged -= PropertyChangedListenerOf_widget_2_1_3;
		_widget_2_1_3.boolPropertyChanged -= boolPropertyChangedListenerOf_widget_2_1_3;
		_widget_2_1_3.floatPropertyChanged -= floatPropertyChangedListenerOf_widget_2_1_3;
		_widget_2_1_3.Vec2PropertyChanged -= Vec2PropertyChangedListenerOf_widget_2_1_3;
		_widget_2_1_3.Vector2PropertyChanged -= Vector2PropertyChangedListenerOf_widget_2_1_3;
		_widget_2_1_3.doublePropertyChanged -= doublePropertyChangedListenerOf_widget_2_1_3;
		_widget_2_1_3.intPropertyChanged -= intPropertyChangedListenerOf_widget_2_1_3;
		_widget_2_1_3.uintPropertyChanged -= uintPropertyChangedListenerOf_widget_2_1_3;
		_widget_2_1_3.ColorPropertyChanged -= ColorPropertyChangedListenerOf_widget_2_1_3;
		_widget_2_1_4.PropertyChanged -= PropertyChangedListenerOf_widget_2_1_4;
		_widget_2_1_4.boolPropertyChanged -= boolPropertyChangedListenerOf_widget_2_1_4;
		_widget_2_1_4.floatPropertyChanged -= floatPropertyChangedListenerOf_widget_2_1_4;
		_widget_2_1_4.Vec2PropertyChanged -= Vec2PropertyChangedListenerOf_widget_2_1_4;
		_widget_2_1_4.Vector2PropertyChanged -= Vector2PropertyChangedListenerOf_widget_2_1_4;
		_widget_2_1_4.doublePropertyChanged -= doublePropertyChangedListenerOf_widget_2_1_4;
		_widget_2_1_4.intPropertyChanged -= intPropertyChangedListenerOf_widget_2_1_4;
		_widget_2_1_4.uintPropertyChanged -= uintPropertyChangedListenerOf_widget_2_1_4;
		_widget_2_1_4.ColorPropertyChanged -= ColorPropertyChangedListenerOf_widget_2_1_4;
		_widget_2_2.EventFire -= EventListenerOf_widget_2_2;
		if (_datasource_Root_Items != null)
		{
			_datasource_Root_Items.ListChanged -= OnList_datasource_Root_ItemsChanged;
			for (int num = _widget_2_1_5_1_0_0.ChildCount - 1; num >= 0; num--)
			{
				Widget child = _widget_2_1_5_1_0_0.GetChild(num);
				((ClanScreen__TaleWorlds_CampaignSystem_ViewModelCollection_ClanManagement_ClanManagementVM_Dependency_38_ItemTemplate)child).OnBeforeRemovedChild(child);
				((ClanScreen__TaleWorlds_CampaignSystem_ViewModelCollection_ClanManagement_ClanManagementVM_Dependency_38_ItemTemplate)_widget_2_1_5_1_0_0.GetChild(num)).DestroyDataSource();
			}
			_datasource_Root_Items = null;
		}
		_datasource_Root = null;
	}

	public void SetDataSource(ClanCardSelectionPopupVM dataSource)
	{
		RefreshDataSource_datasource_Root(dataSource);
	}

	private void EventListenerOf_widget_2_2(Widget widget, string commandName, object[] args)
	{
		if (commandName == "Click")
		{
			_datasource_Root.ExecuteCancel();
		}
	}

	private void PropertyChangedListenerOf_widget(PropertyOwnerObject propertyOwnerObject, string propertyName, object e)
	{
		HandleWidgetPropertyChangeOf_widget(propertyName);
	}

	private void boolPropertyChangedListenerOf_widget(PropertyOwnerObject propertyOwnerObject, string propertyName, bool e)
	{
		HandleWidgetPropertyChangeOf_widget(propertyName);
	}

	private void floatPropertyChangedListenerOf_widget(PropertyOwnerObject propertyOwnerObject, string propertyName, float e)
	{
		HandleWidgetPropertyChangeOf_widget(propertyName);
	}

	private void Vec2PropertyChangedListenerOf_widget(PropertyOwnerObject propertyOwnerObject, string propertyName, Vec2 e)
	{
		HandleWidgetPropertyChangeOf_widget(propertyName);
	}

	private void Vector2PropertyChangedListenerOf_widget(PropertyOwnerObject propertyOwnerObject, string propertyName, Vector2 e)
	{
		HandleWidgetPropertyChangeOf_widget(propertyName);
	}

	private void doublePropertyChangedListenerOf_widget(PropertyOwnerObject propertyOwnerObject, string propertyName, double e)
	{
		HandleWidgetPropertyChangeOf_widget(propertyName);
	}

	private void intPropertyChangedListenerOf_widget(PropertyOwnerObject propertyOwnerObject, string propertyName, int e)
	{
		HandleWidgetPropertyChangeOf_widget(propertyName);
	}

	private void uintPropertyChangedListenerOf_widget(PropertyOwnerObject propertyOwnerObject, string propertyName, uint e)
	{
		HandleWidgetPropertyChangeOf_widget(propertyName);
	}

	private void ColorPropertyChangedListenerOf_widget(PropertyOwnerObject propertyOwnerObject, string propertyName, Color e)
	{
		HandleWidgetPropertyChangeOf_widget(propertyName);
	}

	private void HandleWidgetPropertyChangeOf_widget(string propertyName)
	{
		if (propertyName == "IsVisible")
		{
			_datasource_Root.IsVisible = _widget.IsVisible;
		}
	}

	private void PropertyChangedListenerOf_widget_2_1_3(PropertyOwnerObject propertyOwnerObject, string propertyName, object e)
	{
		HandleWidgetPropertyChangeOf_widget_2_1_3(propertyName);
	}

	private void boolPropertyChangedListenerOf_widget_2_1_3(PropertyOwnerObject propertyOwnerObject, string propertyName, bool e)
	{
		HandleWidgetPropertyChangeOf_widget_2_1_3(propertyName);
	}

	private void floatPropertyChangedListenerOf_widget_2_1_3(PropertyOwnerObject propertyOwnerObject, string propertyName, float e)
	{
		HandleWidgetPropertyChangeOf_widget_2_1_3(propertyName);
	}

	private void Vec2PropertyChangedListenerOf_widget_2_1_3(PropertyOwnerObject propertyOwnerObject, string propertyName, Vec2 e)
	{
		HandleWidgetPropertyChangeOf_widget_2_1_3(propertyName);
	}

	private void Vector2PropertyChangedListenerOf_widget_2_1_3(PropertyOwnerObject propertyOwnerObject, string propertyName, Vector2 e)
	{
		HandleWidgetPropertyChangeOf_widget_2_1_3(propertyName);
	}

	private void doublePropertyChangedListenerOf_widget_2_1_3(PropertyOwnerObject propertyOwnerObject, string propertyName, double e)
	{
		HandleWidgetPropertyChangeOf_widget_2_1_3(propertyName);
	}

	private void intPropertyChangedListenerOf_widget_2_1_3(PropertyOwnerObject propertyOwnerObject, string propertyName, int e)
	{
		HandleWidgetPropertyChangeOf_widget_2_1_3(propertyName);
	}

	private void uintPropertyChangedListenerOf_widget_2_1_3(PropertyOwnerObject propertyOwnerObject, string propertyName, uint e)
	{
		HandleWidgetPropertyChangeOf_widget_2_1_3(propertyName);
	}

	private void ColorPropertyChangedListenerOf_widget_2_1_3(PropertyOwnerObject propertyOwnerObject, string propertyName, Color e)
	{
		HandleWidgetPropertyChangeOf_widget_2_1_3(propertyName);
	}

	private void HandleWidgetPropertyChangeOf_widget_2_1_3(string propertyName)
	{
		if (propertyName == "Text")
		{
			_datasource_Root.Title = _widget_2_1_3.Text;
		}
	}

	private void PropertyChangedListenerOf_widget_2_1_4(PropertyOwnerObject propertyOwnerObject, string propertyName, object e)
	{
		HandleWidgetPropertyChangeOf_widget_2_1_4(propertyName);
	}

	private void boolPropertyChangedListenerOf_widget_2_1_4(PropertyOwnerObject propertyOwnerObject, string propertyName, bool e)
	{
		HandleWidgetPropertyChangeOf_widget_2_1_4(propertyName);
	}

	private void floatPropertyChangedListenerOf_widget_2_1_4(PropertyOwnerObject propertyOwnerObject, string propertyName, float e)
	{
		HandleWidgetPropertyChangeOf_widget_2_1_4(propertyName);
	}

	private void Vec2PropertyChangedListenerOf_widget_2_1_4(PropertyOwnerObject propertyOwnerObject, string propertyName, Vec2 e)
	{
		HandleWidgetPropertyChangeOf_widget_2_1_4(propertyName);
	}

	private void Vector2PropertyChangedListenerOf_widget_2_1_4(PropertyOwnerObject propertyOwnerObject, string propertyName, Vector2 e)
	{
		HandleWidgetPropertyChangeOf_widget_2_1_4(propertyName);
	}

	private void doublePropertyChangedListenerOf_widget_2_1_4(PropertyOwnerObject propertyOwnerObject, string propertyName, double e)
	{
		HandleWidgetPropertyChangeOf_widget_2_1_4(propertyName);
	}

	private void intPropertyChangedListenerOf_widget_2_1_4(PropertyOwnerObject propertyOwnerObject, string propertyName, int e)
	{
		HandleWidgetPropertyChangeOf_widget_2_1_4(propertyName);
	}

	private void uintPropertyChangedListenerOf_widget_2_1_4(PropertyOwnerObject propertyOwnerObject, string propertyName, uint e)
	{
		HandleWidgetPropertyChangeOf_widget_2_1_4(propertyName);
	}

	private void ColorPropertyChangedListenerOf_widget_2_1_4(PropertyOwnerObject propertyOwnerObject, string propertyName, Color e)
	{
		HandleWidgetPropertyChangeOf_widget_2_1_4(propertyName);
	}

	private void HandleWidgetPropertyChangeOf_widget_2_1_4(string propertyName)
	{
		if (propertyName == "Text")
		{
			_datasource_Root.ActionResult = _widget_2_1_4.Text;
		}
	}

	private void ViewModelPropertyChangedListenerOf_datasource_Root(object sender, PropertyChangedEventArgs e)
	{
		HandleViewModelPropertyChangeOf_datasource_Root(e.PropertyName);
	}

	private void ViewModelPropertyChangedWithValueListenerOf_datasource_Root(object sender, PropertyChangedWithValueEventArgs e)
	{
		HandleViewModelPropertyChangeOf_datasource_Root(e.PropertyName);
	}

	private void ViewModelPropertyChangedWithBoolValueListenerOf_datasource_Root(object sender, PropertyChangedWithBoolValueEventArgs e)
	{
		HandleViewModelPropertyChangeOf_datasource_Root(e.PropertyName);
	}

	private void ViewModelPropertyChangedWithIntValueListenerOf_datasource_Root(object sender, PropertyChangedWithIntValueEventArgs e)
	{
		HandleViewModelPropertyChangeOf_datasource_Root(e.PropertyName);
	}

	private void ViewModelPropertyChangedWithFloatValueListenerOf_datasource_Root(object sender, PropertyChangedWithFloatValueEventArgs e)
	{
		HandleViewModelPropertyChangeOf_datasource_Root(e.PropertyName);
	}

	private void ViewModelPropertyChangedWithUIntValueListenerOf_datasource_Root(object sender, PropertyChangedWithUIntValueEventArgs e)
	{
		HandleViewModelPropertyChangeOf_datasource_Root(e.PropertyName);
	}

	private void ViewModelPropertyChangedWithColorValueListenerOf_datasource_Root(object sender, PropertyChangedWithColorValueEventArgs e)
	{
		HandleViewModelPropertyChangeOf_datasource_Root(e.PropertyName);
	}

	private void ViewModelPropertyChangedWithDoubleValueListenerOf_datasource_Root(object sender, PropertyChangedWithDoubleValueEventArgs e)
	{
		HandleViewModelPropertyChangeOf_datasource_Root(e.PropertyName);
	}

	private void ViewModelPropertyChangedWithVec2ValueListenerOf_datasource_Root(object sender, PropertyChangedWithVec2ValueEventArgs e)
	{
		HandleViewModelPropertyChangeOf_datasource_Root(e.PropertyName);
	}

	private void HandleViewModelPropertyChangeOf_datasource_Root(string propertyName)
	{
		switch (propertyName)
		{
		case "Items":
			RefreshDataSource_datasource_Root_Items(_datasource_Root.Items);
			break;
		case "IsVisible":
			_widget.IsVisible = _datasource_Root.IsVisible;
			break;
		case "Title":
			_widget_2_1_3.Text = _datasource_Root.Title;
			break;
		case "ActionResult":
			_widget_2_1_4.Text = _datasource_Root.ActionResult;
			break;
		}
	}

	public void OnList_datasource_Root_ItemsChanged(object sender, TaleWorlds.Library.ListChangedEventArgs e)
	{
		switch (e.ListChangedType)
		{
		case TaleWorlds.Library.ListChangedType.Reset:
		{
			for (int num = _widget_2_1_5_1_0_0.ChildCount - 1; num >= 0; num--)
			{
				Widget child3 = _widget_2_1_5_1_0_0.GetChild(num);
				((ClanScreen__TaleWorlds_CampaignSystem_ViewModelCollection_ClanManagement_ClanManagementVM_Dependency_38_ItemTemplate)child3).OnBeforeRemovedChild(child3);
				Widget child4 = _widget_2_1_5_1_0_0.GetChild(num);
				((ClanScreen__TaleWorlds_CampaignSystem_ViewModelCollection_ClanManagement_ClanManagementVM_Dependency_38_ItemTemplate)child4).SetDataSource(null);
				_widget_2_1_5_1_0_0.RemoveChild(child4);
			}
			break;
		}
		case TaleWorlds.Library.ListChangedType.Sorted:
		{
			for (int i = 0; i < _datasource_Root_Items.Count; i++)
			{
				ClanCardSelectionPopupItemVM bindingObject = _datasource_Root_Items[i];
				_widget_2_1_5_1_0_0.FindChild((Widget widget) => widget.GetComponent<GeneratedWidgetData>().Data == bindingObject).SetSiblingIndex(i);
			}
			break;
		}
		case TaleWorlds.Library.ListChangedType.ItemAdded:
		{
			ClanScreen__TaleWorlds_CampaignSystem_ViewModelCollection_ClanManagement_ClanManagementVM_Dependency_38_ItemTemplate clanScreen__TaleWorlds_CampaignSystem_ViewModelCollection_ClanManagement_ClanManagementVM_Dependency_38_ItemTemplate = new ClanScreen__TaleWorlds_CampaignSystem_ViewModelCollection_ClanManagement_ClanManagementVM_Dependency_38_ItemTemplate(base.Context);
			GeneratedWidgetData generatedWidgetData = new GeneratedWidgetData(clanScreen__TaleWorlds_CampaignSystem_ViewModelCollection_ClanManagement_ClanManagementVM_Dependency_38_ItemTemplate);
			ClanCardSelectionPopupItemVM dataSource = (ClanCardSelectionPopupItemVM)(generatedWidgetData.Data = _datasource_Root_Items[e.NewIndex]);
			clanScreen__TaleWorlds_CampaignSystem_ViewModelCollection_ClanManagement_ClanManagementVM_Dependency_38_ItemTemplate.AddComponent(generatedWidgetData);
			_widget_2_1_5_1_0_0.AddChildAtIndex(clanScreen__TaleWorlds_CampaignSystem_ViewModelCollection_ClanManagement_ClanManagementVM_Dependency_38_ItemTemplate, e.NewIndex);
			clanScreen__TaleWorlds_CampaignSystem_ViewModelCollection_ClanManagement_ClanManagementVM_Dependency_38_ItemTemplate.CreateWidgets();
			clanScreen__TaleWorlds_CampaignSystem_ViewModelCollection_ClanManagement_ClanManagementVM_Dependency_38_ItemTemplate.SetIds();
			clanScreen__TaleWorlds_CampaignSystem_ViewModelCollection_ClanManagement_ClanManagementVM_Dependency_38_ItemTemplate.SetAttributes();
			clanScreen__TaleWorlds_CampaignSystem_ViewModelCollection_ClanManagement_ClanManagementVM_Dependency_38_ItemTemplate.SetDataSource(dataSource);
			break;
		}
		case TaleWorlds.Library.ListChangedType.ItemBeforeDeleted:
		{
			Widget child2 = _widget_2_1_5_1_0_0.GetChild(e.NewIndex);
			((ClanScreen__TaleWorlds_CampaignSystem_ViewModelCollection_ClanManagement_ClanManagementVM_Dependency_38_ItemTemplate)child2).OnBeforeRemovedChild(child2);
			break;
		}
		case TaleWorlds.Library.ListChangedType.ItemDeleted:
		{
			Widget child = _widget_2_1_5_1_0_0.GetChild(e.NewIndex);
			((ClanScreen__TaleWorlds_CampaignSystem_ViewModelCollection_ClanManagement_ClanManagementVM_Dependency_38_ItemTemplate)child).SetDataSource(null);
			_widget_2_1_5_1_0_0.RemoveChild(child);
			break;
		}
		case TaleWorlds.Library.ListChangedType.ItemChanged:
			break;
		}
	}

	private void RefreshDataSource_datasource_Root(ClanCardSelectionPopupVM newDataSource)
	{
		if (_datasource_Root != null)
		{
			_widget_2_0.SetDataSource(null);
			_datasource_Root.PropertyChanged -= ViewModelPropertyChangedListenerOf_datasource_Root;
			_datasource_Root.PropertyChangedWithValue -= ViewModelPropertyChangedWithValueListenerOf_datasource_Root;
			_datasource_Root.PropertyChangedWithBoolValue -= ViewModelPropertyChangedWithBoolValueListenerOf_datasource_Root;
			_datasource_Root.PropertyChangedWithIntValue -= ViewModelPropertyChangedWithIntValueListenerOf_datasource_Root;
			_datasource_Root.PropertyChangedWithFloatValue -= ViewModelPropertyChangedWithFloatValueListenerOf_datasource_Root;
			_datasource_Root.PropertyChangedWithUIntValue -= ViewModelPropertyChangedWithUIntValueListenerOf_datasource_Root;
			_datasource_Root.PropertyChangedWithColorValue -= ViewModelPropertyChangedWithColorValueListenerOf_datasource_Root;
			_datasource_Root.PropertyChangedWithDoubleValue -= ViewModelPropertyChangedWithDoubleValueListenerOf_datasource_Root;
			_datasource_Root.PropertyChangedWithVec2Value -= ViewModelPropertyChangedWithVec2ValueListenerOf_datasource_Root;
			_widget.PropertyChanged -= PropertyChangedListenerOf_widget;
			_widget.boolPropertyChanged -= boolPropertyChangedListenerOf_widget;
			_widget.floatPropertyChanged -= floatPropertyChangedListenerOf_widget;
			_widget.Vec2PropertyChanged -= Vec2PropertyChangedListenerOf_widget;
			_widget.Vector2PropertyChanged -= Vector2PropertyChangedListenerOf_widget;
			_widget.doublePropertyChanged -= doublePropertyChangedListenerOf_widget;
			_widget.intPropertyChanged -= intPropertyChangedListenerOf_widget;
			_widget.uintPropertyChanged -= uintPropertyChangedListenerOf_widget;
			_widget.ColorPropertyChanged -= ColorPropertyChangedListenerOf_widget;
			_widget_2_1_3.PropertyChanged -= PropertyChangedListenerOf_widget_2_1_3;
			_widget_2_1_3.boolPropertyChanged -= boolPropertyChangedListenerOf_widget_2_1_3;
			_widget_2_1_3.floatPropertyChanged -= floatPropertyChangedListenerOf_widget_2_1_3;
			_widget_2_1_3.Vec2PropertyChanged -= Vec2PropertyChangedListenerOf_widget_2_1_3;
			_widget_2_1_3.Vector2PropertyChanged -= Vector2PropertyChangedListenerOf_widget_2_1_3;
			_widget_2_1_3.doublePropertyChanged -= doublePropertyChangedListenerOf_widget_2_1_3;
			_widget_2_1_3.intPropertyChanged -= intPropertyChangedListenerOf_widget_2_1_3;
			_widget_2_1_3.uintPropertyChanged -= uintPropertyChangedListenerOf_widget_2_1_3;
			_widget_2_1_3.ColorPropertyChanged -= ColorPropertyChangedListenerOf_widget_2_1_3;
			_widget_2_1_4.PropertyChanged -= PropertyChangedListenerOf_widget_2_1_4;
			_widget_2_1_4.boolPropertyChanged -= boolPropertyChangedListenerOf_widget_2_1_4;
			_widget_2_1_4.floatPropertyChanged -= floatPropertyChangedListenerOf_widget_2_1_4;
			_widget_2_1_4.Vec2PropertyChanged -= Vec2PropertyChangedListenerOf_widget_2_1_4;
			_widget_2_1_4.Vector2PropertyChanged -= Vector2PropertyChangedListenerOf_widget_2_1_4;
			_widget_2_1_4.doublePropertyChanged -= doublePropertyChangedListenerOf_widget_2_1_4;
			_widget_2_1_4.intPropertyChanged -= intPropertyChangedListenerOf_widget_2_1_4;
			_widget_2_1_4.uintPropertyChanged -= uintPropertyChangedListenerOf_widget_2_1_4;
			_widget_2_1_4.ColorPropertyChanged -= ColorPropertyChangedListenerOf_widget_2_1_4;
			_widget_2_2.EventFire -= EventListenerOf_widget_2_2;
			if (_datasource_Root_Items != null)
			{
				_datasource_Root_Items.ListChanged -= OnList_datasource_Root_ItemsChanged;
				for (int num = _widget_2_1_5_1_0_0.ChildCount - 1; num >= 0; num--)
				{
					Widget child = _widget_2_1_5_1_0_0.GetChild(num);
					((ClanScreen__TaleWorlds_CampaignSystem_ViewModelCollection_ClanManagement_ClanManagementVM_Dependency_38_ItemTemplate)child).OnBeforeRemovedChild(child);
					Widget child2 = _widget_2_1_5_1_0_0.GetChild(num);
					((ClanScreen__TaleWorlds_CampaignSystem_ViewModelCollection_ClanManagement_ClanManagementVM_Dependency_38_ItemTemplate)child2).SetDataSource(null);
					_widget_2_1_5_1_0_0.RemoveChild(child2);
				}
				_datasource_Root_Items = null;
			}
			_datasource_Root = null;
		}
		_datasource_Root = newDataSource;
		if (_datasource_Root == null)
		{
			return;
		}
		_datasource_Root.PropertyChanged += ViewModelPropertyChangedListenerOf_datasource_Root;
		_datasource_Root.PropertyChangedWithValue += ViewModelPropertyChangedWithValueListenerOf_datasource_Root;
		_datasource_Root.PropertyChangedWithBoolValue += ViewModelPropertyChangedWithBoolValueListenerOf_datasource_Root;
		_datasource_Root.PropertyChangedWithIntValue += ViewModelPropertyChangedWithIntValueListenerOf_datasource_Root;
		_datasource_Root.PropertyChangedWithFloatValue += ViewModelPropertyChangedWithFloatValueListenerOf_datasource_Root;
		_datasource_Root.PropertyChangedWithUIntValue += ViewModelPropertyChangedWithUIntValueListenerOf_datasource_Root;
		_datasource_Root.PropertyChangedWithColorValue += ViewModelPropertyChangedWithColorValueListenerOf_datasource_Root;
		_datasource_Root.PropertyChangedWithDoubleValue += ViewModelPropertyChangedWithDoubleValueListenerOf_datasource_Root;
		_datasource_Root.PropertyChangedWithVec2Value += ViewModelPropertyChangedWithVec2ValueListenerOf_datasource_Root;
		_widget.IsVisible = _datasource_Root.IsVisible;
		_widget.PropertyChanged += PropertyChangedListenerOf_widget;
		_widget.boolPropertyChanged += boolPropertyChangedListenerOf_widget;
		_widget.floatPropertyChanged += floatPropertyChangedListenerOf_widget;
		_widget.Vec2PropertyChanged += Vec2PropertyChangedListenerOf_widget;
		_widget.Vector2PropertyChanged += Vector2PropertyChangedListenerOf_widget;
		_widget.doublePropertyChanged += doublePropertyChangedListenerOf_widget;
		_widget.intPropertyChanged += intPropertyChangedListenerOf_widget;
		_widget.uintPropertyChanged += uintPropertyChangedListenerOf_widget;
		_widget.ColorPropertyChanged += ColorPropertyChangedListenerOf_widget;
		_widget_2_1_3.Text = _datasource_Root.Title;
		_widget_2_1_3.PropertyChanged += PropertyChangedListenerOf_widget_2_1_3;
		_widget_2_1_3.boolPropertyChanged += boolPropertyChangedListenerOf_widget_2_1_3;
		_widget_2_1_3.floatPropertyChanged += floatPropertyChangedListenerOf_widget_2_1_3;
		_widget_2_1_3.Vec2PropertyChanged += Vec2PropertyChangedListenerOf_widget_2_1_3;
		_widget_2_1_3.Vector2PropertyChanged += Vector2PropertyChangedListenerOf_widget_2_1_3;
		_widget_2_1_3.doublePropertyChanged += doublePropertyChangedListenerOf_widget_2_1_3;
		_widget_2_1_3.intPropertyChanged += intPropertyChangedListenerOf_widget_2_1_3;
		_widget_2_1_3.uintPropertyChanged += uintPropertyChangedListenerOf_widget_2_1_3;
		_widget_2_1_3.ColorPropertyChanged += ColorPropertyChangedListenerOf_widget_2_1_3;
		_widget_2_1_4.Text = _datasource_Root.ActionResult;
		_widget_2_1_4.PropertyChanged += PropertyChangedListenerOf_widget_2_1_4;
		_widget_2_1_4.boolPropertyChanged += boolPropertyChangedListenerOf_widget_2_1_4;
		_widget_2_1_4.floatPropertyChanged += floatPropertyChangedListenerOf_widget_2_1_4;
		_widget_2_1_4.Vec2PropertyChanged += Vec2PropertyChangedListenerOf_widget_2_1_4;
		_widget_2_1_4.Vector2PropertyChanged += Vector2PropertyChangedListenerOf_widget_2_1_4;
		_widget_2_1_4.doublePropertyChanged += doublePropertyChangedListenerOf_widget_2_1_4;
		_widget_2_1_4.intPropertyChanged += intPropertyChangedListenerOf_widget_2_1_4;
		_widget_2_1_4.uintPropertyChanged += uintPropertyChangedListenerOf_widget_2_1_4;
		_widget_2_1_4.ColorPropertyChanged += ColorPropertyChangedListenerOf_widget_2_1_4;
		_widget_2_2.EventFire += EventListenerOf_widget_2_2;
		_datasource_Root_Items = _datasource_Root.Items;
		if (_datasource_Root_Items != null)
		{
			_datasource_Root_Items.ListChanged += OnList_datasource_Root_ItemsChanged;
			for (int i = 0; i < _datasource_Root_Items.Count; i++)
			{
				ClanScreen__TaleWorlds_CampaignSystem_ViewModelCollection_ClanManagement_ClanManagementVM_Dependency_38_ItemTemplate clanScreen__TaleWorlds_CampaignSystem_ViewModelCollection_ClanManagement_ClanManagementVM_Dependency_38_ItemTemplate = new ClanScreen__TaleWorlds_CampaignSystem_ViewModelCollection_ClanManagement_ClanManagementVM_Dependency_38_ItemTemplate(base.Context);
				GeneratedWidgetData generatedWidgetData = new GeneratedWidgetData(clanScreen__TaleWorlds_CampaignSystem_ViewModelCollection_ClanManagement_ClanManagementVM_Dependency_38_ItemTemplate);
				ClanCardSelectionPopupItemVM dataSource = (ClanCardSelectionPopupItemVM)(generatedWidgetData.Data = _datasource_Root_Items[i]);
				clanScreen__TaleWorlds_CampaignSystem_ViewModelCollection_ClanManagement_ClanManagementVM_Dependency_38_ItemTemplate.AddComponent(generatedWidgetData);
				_widget_2_1_5_1_0_0.AddChildAtIndex(clanScreen__TaleWorlds_CampaignSystem_ViewModelCollection_ClanManagement_ClanManagementVM_Dependency_38_ItemTemplate, i);
				clanScreen__TaleWorlds_CampaignSystem_ViewModelCollection_ClanManagement_ClanManagementVM_Dependency_38_ItemTemplate.CreateWidgets();
				clanScreen__TaleWorlds_CampaignSystem_ViewModelCollection_ClanManagement_ClanManagementVM_Dependency_38_ItemTemplate.SetIds();
				clanScreen__TaleWorlds_CampaignSystem_ViewModelCollection_ClanManagement_ClanManagementVM_Dependency_38_ItemTemplate.SetAttributes();
				clanScreen__TaleWorlds_CampaignSystem_ViewModelCollection_ClanManagement_ClanManagementVM_Dependency_38_ItemTemplate.SetDataSource(dataSource);
			}
		}
		_widget_2_0.SetDataSource(_datasource_Root);
	}

	private void RefreshDataSource_datasource_Root_Items(MBBindingList<ClanCardSelectionPopupItemVM> newDataSource)
	{
		if (_datasource_Root_Items != null)
		{
			_datasource_Root_Items.ListChanged -= OnList_datasource_Root_ItemsChanged;
			for (int num = _widget_2_1_5_1_0_0.ChildCount - 1; num >= 0; num--)
			{
				Widget child = _widget_2_1_5_1_0_0.GetChild(num);
				((ClanScreen__TaleWorlds_CampaignSystem_ViewModelCollection_ClanManagement_ClanManagementVM_Dependency_38_ItemTemplate)child).OnBeforeRemovedChild(child);
				Widget child2 = _widget_2_1_5_1_0_0.GetChild(num);
				((ClanScreen__TaleWorlds_CampaignSystem_ViewModelCollection_ClanManagement_ClanManagementVM_Dependency_38_ItemTemplate)child2).SetDataSource(null);
				_widget_2_1_5_1_0_0.RemoveChild(child2);
			}
			_datasource_Root_Items = null;
		}
		_datasource_Root_Items = newDataSource;
		_datasource_Root_Items = _datasource_Root.Items;
		if (_datasource_Root_Items != null)
		{
			_datasource_Root_Items.ListChanged += OnList_datasource_Root_ItemsChanged;
			for (int i = 0; i < _datasource_Root_Items.Count; i++)
			{
				ClanScreen__TaleWorlds_CampaignSystem_ViewModelCollection_ClanManagement_ClanManagementVM_Dependency_38_ItemTemplate clanScreen__TaleWorlds_CampaignSystem_ViewModelCollection_ClanManagement_ClanManagementVM_Dependency_38_ItemTemplate = new ClanScreen__TaleWorlds_CampaignSystem_ViewModelCollection_ClanManagement_ClanManagementVM_Dependency_38_ItemTemplate(base.Context);
				GeneratedWidgetData generatedWidgetData = new GeneratedWidgetData(clanScreen__TaleWorlds_CampaignSystem_ViewModelCollection_ClanManagement_ClanManagementVM_Dependency_38_ItemTemplate);
				ClanCardSelectionPopupItemVM dataSource = (ClanCardSelectionPopupItemVM)(generatedWidgetData.Data = _datasource_Root_Items[i]);
				clanScreen__TaleWorlds_CampaignSystem_ViewModelCollection_ClanManagement_ClanManagementVM_Dependency_38_ItemTemplate.AddComponent(generatedWidgetData);
				_widget_2_1_5_1_0_0.AddChildAtIndex(clanScreen__TaleWorlds_CampaignSystem_ViewModelCollection_ClanManagement_ClanManagementVM_Dependency_38_ItemTemplate, i);
				clanScreen__TaleWorlds_CampaignSystem_ViewModelCollection_ClanManagement_ClanManagementVM_Dependency_38_ItemTemplate.CreateWidgets();
				clanScreen__TaleWorlds_CampaignSystem_ViewModelCollection_ClanManagement_ClanManagementVM_Dependency_38_ItemTemplate.SetIds();
				clanScreen__TaleWorlds_CampaignSystem_ViewModelCollection_ClanManagement_ClanManagementVM_Dependency_38_ItemTemplate.SetAttributes();
				clanScreen__TaleWorlds_CampaignSystem_ViewModelCollection_ClanManagement_ClanManagementVM_Dependency_38_ItemTemplate.SetDataSource(dataSource);
			}
		}
	}
}