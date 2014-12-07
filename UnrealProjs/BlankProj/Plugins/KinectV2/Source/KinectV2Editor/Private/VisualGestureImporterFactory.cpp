
#include "KinectV2EditorPCH.h"
#include "KinectV2InputDevice.h"
#include "KinectV2/Private/KinectSensor.h"
#include "AllowWindowsPlatformTypes.h"

#include "AssetToolsModule.h"
#include "AssetRegistryModule.h"
#include "PackageTools.h"

#include "Kinect.h"

#include <Kinect.Face.h>
#include <Kinect.VisualGestureBuilder.h>
#pragma comment(lib,"kinect20.lib")
#pragma comment(lib,"Kinect20.Face.lib")
#pragma comment(lib,"Kinect20.VisualGestureBuilder.lib")


#ifdef _UNICODE
#if defined _M_IX86
#pragma comment(linker,"/manifestdependency:\"type='win32' name='Microsoft.Windows.Common-Controls' version='6.0.0.0' processorArchitecture='x86' publicKeyToken='6595b64144ccf1df' language='*'\"")
#elif defined _M_X64
#pragma comment(linker,"/manifestdependency:\"type='win32' name='Microsoft.Windows.Common-Controls' version='6.0.0.0' processorArchitecture='amd64' publicKeyToken='6595b64144ccf1df' language='*'\"")
#else
#pragma comment(linker,"/manifestdependency:\"type='win32' name='Microsoft.Windows.Common-Controls' version='6.0.0.0' processorArchitecture='*' publicKeyToken='6595b64144ccf1df' language='*'\"")
#endif
#endif
#include "CString.h"
#include "UnrealString.h"
#include "NameTypes.h"
#include "StringConv.h"
#include "COMPointer.h"

#define LOCTEXT_NAMESPACE "KinectV2"
#define TILED_IMPORT_ERROR(FormatString, ...) \
	if (!bSilent) { UE_LOG(LogKinectV2Editor, Warning, FormatString, __VA_ARGS__); }
#define TILED_IMPORT_WARNING(FormatString, ...) \
	if (!bSilent) { UE_LOG(LogKinectV2Editor, Warning, FormatString, __VA_ARGS__); }


UVisualGestureImporterFactory::UVisualGestureImporterFactory(const FPostConstructInitializeProperties& PCIP)
	: Super(PCIP)
{
	bCreateNew = false;
	//bEditAfterNew = true;
	SupportedClass = UVisualGesture::StaticClass();

	bEditorImport = true;
	bText = false;

	Formats.Add(TEXT("gbd;Visual Gesture Database file"));
	Formats.Add(TEXT("gba;Visual Gesture Database file"));
}

FText UVisualGestureImporterFactory::GetToolTip() const
{
	return LOCTEXT("VisualGestureImporterFactoryDescription", "Tile maps exported from Tiled");
}

bool UVisualGestureImporterFactory::FactoryCanImport(const FString& Filename)
{


	FString FileContent;
	if (FFileHelper::LoadFileToString(FileContent, *Filename))
	{
		return true;
	}
	/*
	if (GetDefault<UPaperRuntimeSettings>()->bEnableTileMapEditing)
	{
	FString FileContent;
	if (FFileHelper::LoadFileToString( FileContent, *Filename))
	{
	TSharedPtr<FJsonObject> DescriptorObject = ParseJSON(FileContent, FString(),  true);
	if (DescriptorObject.IsValid())
	{
	FTileMapFromTiled GlobalInfo;
	ParseGlobalInfoFromJSON(DescriptorObject, GlobalInfo, FString(),  true);

	return GlobalInfo.IsValid();
	}
	}
	}
	*/
	return true;
}

UObject* UVisualGestureImporterFactory::FactoryCreateText(UClass* InClass, UObject* InParent, FName InName, EObjectFlags Flags, UObject* Context, const TCHAR* Type, const TCHAR*& Buffer, const TCHAR* BufferEnd, FFeedbackContext* Warn)
{

	return nullptr;
	Flags |= RF_Transactional | RF_NeedPostLoad;

	FEditorDelegates::OnAssetPreImport.Broadcast(this, InClass, InParent, InName, Type);

	FAssetToolsModule& AssetToolsModule = FModuleManager::GetModuleChecked<FAssetToolsModule>("AssetTools");

	bool bLoadedSuccessfully = true;

	const FString CurrentFilename = UFactory::GetCurrentFilename();

	TSharedPtr<WCHAR> filePath = TSharedPtr<WCHAR>(new WCHAR[CurrentFilename.Len() + 1]);

	TStringConvert<TCHAR, WCHAR>::Convert(filePath.Get(), CurrentFilename.Len(), &CurrentFilename[0], CurrentFilename.Len());
	filePath.Get()[CurrentFilename.Len()] = L'\0';
	FString CurrentSourcePath;
	FString FilenameNoExtension;
	FString UnusedExtension;
	FPaths::Split(CurrentFilename, CurrentSourcePath, FilenameNoExtension, UnusedExtension);

	const FString LongPackagePath = FPackageName::GetLongPackagePath(InParent->GetOutermost()->GetPathName());

	const FString NameForErrors(InName.ToString());
	const FString FileContent(BufferEnd - Buffer, Buffer);

	UVisualGesture* Result = nullptr;

	Result = NewNamedObject<UVisualGesture>(InParent, InName, Flags);

	Result->GestureBinary = FileContent;


		/*
		if (bLoadedSuccessfully){
			for (uint32 i = 0; i < BODY_COUNT; ++i)
			{
			
				Result->KinectVisualGesture->GestureDatabases[i] = gestureDatabases[i];
				Result->KinectVisualGesture->GestureReaders[i] = gestureReaders[i];
				Result->KinectVisualGesture->GestureSources[i] = gestureSources[i];
			}
		}
		*/
		if (Result != nullptr){
			Result->PostEditChange();
			UVisualGestureAssetImportData* ImportData = UVisualGestureAssetImportData::GetImportDataForVisualGesture(Result);
			//ImportData->SourceFilePath = FReimportManager::SanitizeImportFilename(CurrentFilename, Result);
			//ImportData->SourceFileTimestamp = IFileManager::Get().GetTimeStamp(*CurrentFilename).ToString();
		}
	//}

	
	FEditorDelegates::OnAssetPostImport.Broadcast(this, Result);

	return Result;
}

bool UVisualGestureImporterFactory::CanReimport(UObject* Obj, TArray<FString>& OutFilenames)
{
	if (UVisualGesture* VisualGesture = Cast<UVisualGesture>(Obj))
	{
		if (VisualGesture->AssetImportData != nullptr)
		{
			OutFilenames.Add(FReimportManager::ResolveImportFilename(VisualGesture->AssetImportData->SourceFilePath, VisualGesture));
		}
		else
		{
			OutFilenames.Add(TEXT(""));
		}
		return true;
	}
	return false;
}

void UVisualGestureImporterFactory::SetReimportPaths(UObject* Obj, const TArray<FString>& NewReimportPaths)
{
	/*
	if (UVisualGesture* TileMap = Cast<UVisualGesture>(Obj))
	{
	if (ensure(NewReimportPaths.Num() == 1))
	{
	UTileMapAssetImportData* ImportData = UTileMapAssetImportData::GetImportDataForTileMap(TileMap);

	ImportData->SourceFilePath = FReimportManager::SanitizeImportFilename(NewReimportPaths[0], TileMap);
	}
	}
	*/
}

EReimportResult::Type UVisualGestureImporterFactory::Reimport(UObject* Obj)
{
	if (UVisualGesture* TileMap = Cast<UVisualGesture>(Obj))
	{
		//@TODO: Not implemented yet
		ensureMsg(false, TEXT("Tile map reimport is not implemented yet"));
	}
	return EReimportResult::Failed;
}


UObject* UVisualGestureImporterFactory::CreateNewAsset(UClass* AssetClass, const FString& TargetPath, const FString& DesiredName, EObjectFlags Flags)
{
	FAssetToolsModule& AssetToolsModule = FModuleManager::GetModuleChecked<FAssetToolsModule>("AssetTools");

	// Create a unique package name and asset name for the frame
	const FString TentativePackagePath = PackageTools::SanitizePackageName(TargetPath + TEXT("/") + DesiredName);
	FString DefaultSuffix;
	FString AssetName;
	FString PackageName;
	AssetToolsModule.Get().CreateUniqueAssetName(TentativePackagePath, /*out*/ DefaultSuffix, /*out*/ PackageName, /*out*/ AssetName);

	// Create a package for the asset
	UObject* OuterForAsset = CreatePackage(nullptr, *PackageName);

	// Create a frame in the package
	UObject* NewAsset = ConstructObject<UObject>(AssetClass, OuterForAsset, *AssetName, Flags);
	FAssetRegistryModule::AssetCreated(NewAsset);

	return NewAsset;
}

UObject* UVisualGestureImporterFactory::FactoryCreateBinary(UClass* InClass, UObject* InParent, FName InName, EObjectFlags Flags, UObject* Context, const TCHAR* Type, const uint8*& Buffer, const uint8* BufferEnd, FFeedbackContext* Warn)
{

	Flags |= RF_Transactional;

	FEditorDelegates::OnAssetPreImport.Broadcast(this, InClass, InParent, InName, Type);

	UVisualGesture* Result = nullptr;

	Result = NewNamedObject<UVisualGesture>(InParent, InName, Flags);

	if (Result)
	{

		Result->Buff.Empty(BufferEnd - Buffer);
		Result->Buff.AddUninitialized(BufferEnd - Buffer);
		FMemory::Memcpy(Result->Buff.GetData(), Buffer, Result->Buff.Num());



	}

	FEditorDelegates::OnAssetPostImport.Broadcast(this, Result);

	return Result;
}

#include "HideWindowsPlatformTypes.h"