#pragma once
#include "KinectFunctionLibrary.h"
#include "KinectDepthFrameListenerInterface.generated.h"



UINTERFACE(Blueprintable, MinimalApi)
class UKinectDepthFrameListenerInterface : public UInterface
{
	GENERATED_UINTERFACE_BODY()

};

class IKinectDepthFrameListenerInterface
{
	GENERATED_IINTERFACE_BODY()

public:

	UFUNCTION(BlueprintImplementableEvent, Category = NUI)
		void NewDepthFrameEvent(UTexture* DepthFrame);


};