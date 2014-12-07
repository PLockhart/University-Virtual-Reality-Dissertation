
#include "KinectV2EditorPCH.h"
#include "KinectV2EditorModule.h"
#include "VisualGestureAssetTypeActions.h"
#include "AssetToolsModule.h"
#include "PropertyEditorModule.h"

//#include "KinectV2Editor.generated.inl"


class FKinectV2EditorModule : public IKinectV2EditorModule
{


public:

	virtual void StartupModule() override {

		IAssetTools& AssetTools = FModuleManager::LoadModuleChecked<FAssetToolsModule>("AssetTools").Get();
		RegisterAssetTypeAction(AssetTools, MakeShareable(new FVisualGestureAssetTypeActions));

	}

	virtual void ShutdownModule() override {

		// Unregister all the asset types that we registered
		if (FModuleManager::Get().IsModuleLoaded("AssetTools"))
		{
			IAssetTools& AssetTools = FModuleManager::GetModuleChecked<FAssetToolsModule>("AssetTools").Get();
			for (int32 Index = 0; Index < CreatedAssetTypeActions.Num(); ++Index)
			{
				AssetTools.UnregisterAssetTypeActions(CreatedAssetTypeActions[Index].ToSharedRef());
			}
		}
		CreatedAssetTypeActions.Empty();


	}

private:

	void RegisterAssetTypeAction(IAssetTools& AssetTools, TSharedRef<IAssetTypeActions> Action)
	{
		AssetTools.RegisterAssetTypeActions(Action);
		CreatedAssetTypeActions.Add(Action);
	}

private:

	TArray< TSharedPtr<IAssetTypeActions> > CreatedAssetTypeActions;



};

DEFINE_LOG_CATEGORY(LogKinectV2Editor);
IMPLEMENT_MODULE(FKinectV2EditorModule, KinectV2Editor);