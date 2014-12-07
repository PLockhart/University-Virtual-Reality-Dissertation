
#include "KinectV2EditorPCH.h"

//#include "Messaging.h"
//#include "AssetToolsModule.h"
//#include "Toolkits/AssetEditorManager.h"
//#include "AssetRegistryModule.h"
#include "VisualGestureAssetTypeActions.h"

#define LOCTEXT_NAMESPACE "AssetTypeActions"

//////////////////////////////////////////////////////////////////////////
// FTileSetAssetTypeActions

FText FVisualGestureAssetTypeActions::GetName() const
{
	return LOCTEXT("FVisualGestureAssetTypeActionsName", "Visual Gesture");
}

FColor FVisualGestureAssetTypeActions::GetTypeColor() const
{
	return FColor(0, 255, 255);
}

UClass* FVisualGestureAssetTypeActions::GetSupportedClass() const
{
	return UVisualGesture::StaticClass();
}


uint32 FVisualGestureAssetTypeActions::GetCategories()
{
	return EAssetTypeCategories::Misc;
}

//////////////////////////////////////////////////////////////////////////

#undef LOCTEXT_NAMESPACE