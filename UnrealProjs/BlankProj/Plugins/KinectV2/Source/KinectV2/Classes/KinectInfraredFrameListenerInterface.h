#pragma once
#include "KinectFunctionLibrary.h"
#include "KinectInfraredFrameListenerInterface.generated.h"



UINTERFACE(Blueprintable, MinimalApi)
class UKinectInfraredFrameListenerInterface : public UInterface
{
	GENERATED_UINTERFACE_BODY()

};

class IKinectInfraredFrameListenerInterface
{
	GENERATED_IINTERFACE_BODY()

public:

	UFUNCTION(BlueprintImplementableEvent, Category = NUI)
		void NewInfraredFrameEvent(UTexture* InfraredFrame);


};