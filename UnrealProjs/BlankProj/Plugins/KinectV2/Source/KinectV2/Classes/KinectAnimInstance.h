#pragma once

#include "Engine/EngineTypes.h"
#include "Animation/AnimInstance.h"
#include "KinectFunctionLibrary.h"
#include "Animation/AnimNodeSpaceConversions.h"
#include "KinectAnimInstance.generated.h"


UCLASS(MinimalApi, BlueprintType)
class UKinectAnimInstance : public UAnimInstance
{

	GENERATED_UCLASS_BODY()

public:

	virtual bool NativeEvaluateAnimation(FPoseContext& Output) override;

public:

	UPROPERTY(EditAnywhere, Category = "Kinect")
		TEnumAsByte<EAutoReceiveInput::Type> ReceiveInputFromPlayer;

	//UPROPERTY(EditAnywhere, Category = "Kinect")
	//	FName BoneName;

	//UPROPERTY(EditAnywhere, Category = "Kinect")
	//	FRotator BoneRotation;

	UFUNCTION(BlueprintCallable, Category = "Kinect")
		void OnKinectBodyEvent(EAutoReceiveInput::Type KinectPlayer, const FBody& Skeleton);

	UFUNCTION(BlueprintCallable, Category = "Animation")
		void OverrideBoneRotationByName(FName BoneName, FRotator BoneBoneRotation);
	
protected:

private:

	TArray<FRotator> RotatorQueue;

	TArray<FName> NameQueue;

};


