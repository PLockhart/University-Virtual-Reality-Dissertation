
#pragma once
#include "KinectFunctionLibrary.h"
#include "KinectBodyFrameListenerInterface.generated.h"



DECLARE_DELEGATE_FiveParams(FVirtualBlueprintEventsWorkaroundEvent, const TArray<FTransform>& , bool , EHandState::Type , EHandState::Type , const FPlane& );


UINTERFACE(MinimalApi)
class UKinectBodyFrameListenerInterface : public UInterface
{
	GENERATED_UINTERFACE_BODY()
};

class KINECTV2_API IKinectBodyFrameListenerInterface
{
	GENERATED_IINTERFACE_BODY()

public:

	UFUNCTION(BlueprintNativeEvent, Category = NUI, meta = (DeprecatedFunction, DeprecationMessage = "Please use NewBodyFrameEventArray instead"))
		void NewBodyFrameEvent(int32 SkeletonIndex, const TArray<FTransform>& BoneTransforms, bool IsTracked, EHandState::Type RightHandState, EHandState::Type LeftHandState,const FPlane& FloorPlane);

	UFUNCTION(BlueprintNativeEvent, Category = NUI)
		void NewBodyFrameEventArray(const FBodyFrame& BodyFrame);

protected:

	FVirtualBlueprintEventsWorkaroundEvent VirtualNewBodyFrameEvent;

};