#pragma once

#include "VisualGestureAssetImportData.generated.h"

USTRUCT()
struct FVisualGestureImportMapping
{
	GENERATED_USTRUCT_BODY()

		UPROPERTY()
		FString SourceName;

	//UPROPERTY()
	//	TWeakObjectPtr<class UPaperTileSet> ImportedTileSet;

	//UPROPERTY()
	//	TWeakObjectPtr<class UTexture> ImportedTexture;
};

/**
* Base class for import data and options used when importing a tile map
*/
UCLASS()
class KINECTV2EDITOR_API UVisualGestureAssetImportData : public UAssetImportData
{
	GENERATED_UCLASS_BODY()

		UPROPERTY()
		TArray<FVisualGestureImportMapping> TileSetMap;

	static UVisualGestureAssetImportData* GetImportDataForVisualGesture(class UVisualGesture* VisualGesture);
};
