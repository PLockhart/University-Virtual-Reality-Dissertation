#pragma once

#include "VisualGesture.h"
#include "VisualGestureDetector.generated.h"


UCLASS(MinimalApi, BlueprintType)
class UVisualGestureDetector : public UObject
{

	GENERATED_UCLASS_BODY()

public:

	UPROPERTY(Category = VisualGestureAsset, AssetRegistrySearchable, EditAnywhere)
		UVisualGesture* VisualGesture;

};