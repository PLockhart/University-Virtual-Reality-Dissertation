
#include "IKinectV2Plugin.h"
#include "KinectAnimInstance.h"
#include "AnimationRuntime.h"
#include "AnimationUtils.h"
#include "AnimTree.h"
#include "KinectEventManager.h"

UKinectAnimInstance::UKinectAnimInstance(const class FPostConstructInitializeProperties& PCIP) :Super(PCIP){



}



bool UKinectAnimInstance::NativeEvaluateAnimation(FPoseContext& Output)
{

	if (RootNode != NULL)
	{
		//SCOPE_CYCLE_COUNTER(STAT_AnimGraphEvaluate);

		RootNode->Evaluate(Output);
	}
	else
	{
		Output.ResetToRefPose();
	}


	USkeletalMeshComponent* OwningComponent = GetOwningComponent();

	//Proof of concept
	if (OwningComponent)
	{

		for (auto i = 0; i < NameQueue.Num(); ++i){

			auto BoneName = NameQueue[i];
			if (BoneName != NAME_None)
			{

				int32 BoneIndex = OwningComponent->GetBoneIndex(BoneName);

				if (BoneIndex >= 0)
				{
					FA2CSPose CSPose;

					CSPose.AllocateLocalPoses(RequiredBones, Output.Pose);



					auto BoneTransform = CSPose.GetComponentSpaceTransform(BoneIndex);

					BoneTransform.SetRotation(FQuat(RotatorQueue[i]));

					int32 ParentIndex = OwningComponent->GetBoneIndex(OwningComponent->GetParentBone(BoneName));
					if (ParentIndex >= 0)
					{
						Output.Pose.Bones[BoneIndex].SetFromMatrix(BoneTransform.ToMatrixWithScale());

						Output.Pose.Bones[BoneIndex].SetToRelativeTransform(CSPose.GetComponentSpaceTransform(ParentIndex));
					}
				}
			}
		}
	}

	NameQueue.Reset();
	RotatorQueue.Reset();

	return true;
}

void UKinectAnimInstance::OnKinectBodyEvent(EAutoReceiveInput::Type KinectPlayer, const FBody& Skeleton)
{

}

void UKinectAnimInstance::OverrideBoneRotationByName(FName BoneName, FRotator BoneRotation)
{
	NameQueue.Push(BoneName);
	RotatorQueue.Push(BoneRotation);
}
