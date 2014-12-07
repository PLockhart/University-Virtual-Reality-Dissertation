
#pragma once

#include "COMPointer.h"
#include "AllowWindowsPlatformTypes.h"
#include "Kinect.h"
#include "Kinect.VisualGestureBuilder.h"
#include "HideWindowsPlatformTypes.h"
#include "Engine/EngineTypes.h"
#include "VisualGesture.generated.h"

//USTRUCT()
struct KINECTV2_API FKinectVisualGestureWrapper {

	//GENERATED_USTRUCT_BODY()

public:

	~FKinectVisualGestureWrapper();

	IVisualGestureBuilderFrameSource*	GestureSources[BODY_COUNT];
	IVisualGestureBuilderFrameReader*	GestureReaders[BODY_COUNT];
	IVisualGestureBuilderDatabase*		GestureDatabases[BODY_COUNT];

	bool Loaded = false;
private:



};

USTRUCT(BlueprintType)
struct KINECTV2_API FKinectGesture
{
	GENERATED_USTRUCT_BODY()
public:

	UPROPERTY(VisibleAnywhere, BlueprintReadOnly, Category = "Kinect")
		float Confidence;
	UPROPERTY(VisibleAnywhere, BlueprintReadOnly, Category = "Kinect")
		FString GestureName;
	UPROPERTY(VisibleAnywhere, BlueprintReadOnly, Category = "Kinect")
		TEnumAsByte<EAutoReceiveInput::Type> DetectedPlayer;

};

UCLASS(BlueprintType)
class KINECTV2_API UVisualGesture : public UDataAsset
{

	GENERATED_UCLASS_BODY()

public:

	friend class UVisualGestureImporterFactory;

	UFUNCTION(BlueprintCallable, Category = "Kinect")
		TArray<FKinectGesture> GetGestures(const struct FBodyFrame& Body);

	TSharedPtr<FKinectVisualGestureWrapper> KinectVisualGesture;


	virtual void PostLoad() override;
private:
	UPROPERTY()
		FString GestureBinary;

	UPROPERTY()
		TArray<uint8> Buff;

#if WITH_EDITORONLY_DATA
	/** Importing data and options used for this tile map */
	UPROPERTY(Category = ImportSettings, VisibleAnywhere)
	class UAssetImportData* AssetImportData;
#endif
};