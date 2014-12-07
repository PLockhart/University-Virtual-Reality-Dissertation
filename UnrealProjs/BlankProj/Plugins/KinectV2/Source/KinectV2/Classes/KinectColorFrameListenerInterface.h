
#pragma once
#include "KinectFunctionLibrary.h"
#include "KinectColorFrameListenerInterface.generated.h"



UINTERFACE(Blueprintable, MinimalApi)
class UKinectColorFrameListenerInterface : public UInterface
{
	GENERATED_UINTERFACE_BODY()

};
DEPRECATED(4.4, "IKinectColorFrameListenerInterface is now deprecated. Please use Kinect Event Manager Delegates instead.")
class IKinectColorFrameListenerInterface
{
	GENERATED_IINTERFACE_BODY()

public:

	UFUNCTION(BlueprintImplementableEvent, Category = NUI)
		void NewColorFrameEvent(UTexture* ColorFrame);


};