
#pragma once
#include "KinectFunctionLibrary.h"
#include "KinectListenerInterface.generated.h"



UINTERFACE(Blueprintable,MinimalApi)
class UKinectListenerInterface : public UInterface
{
	GENERATED_UINTERFACE_BODY()
};

class IKinectListenerInterface
{
	GENERATED_IINTERFACE_BODY()

public:

	UFUNCTION(BlueprintImplementableEvent, Category = NUI)
		void ReciveBody(int32 SkeletonIndex, const TArray<FTransform>& BoneTransforms, bool IsTracked);
};