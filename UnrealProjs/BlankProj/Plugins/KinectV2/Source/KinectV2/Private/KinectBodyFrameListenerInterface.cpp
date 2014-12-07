

#include "IKinectV2Plugin.h"
#include "KinectBodyFrameListenerInterface.h"

UKinectBodyFrameListenerInterface::UKinectBodyFrameListenerInterface(const class FPostConstructInitializeProperties& PCIP)
: Super(PCIP)
{
	
}

void IKinectBodyFrameListenerInterface::NewBodyFrameEvent_Implementation(int32 SkeletonIndex, const TArray<FTransform>& BoneTransforms, bool IsTracked, EHandState::Type RightHandState, EHandState::Type LeftHandState, const FPlane& FloorPlane)
{

}

void IKinectBodyFrameListenerInterface::NewBodyFrameEventArray_Implementation(const FBodyFrame& BodyFrame)
{

}
