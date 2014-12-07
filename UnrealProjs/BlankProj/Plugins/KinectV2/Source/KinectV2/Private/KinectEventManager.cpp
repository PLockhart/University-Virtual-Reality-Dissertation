
#include "IKinectV2Plugin.h"

#include "Slate.h"
#include "KinectFunctionLibrary.h"
#include "KinectEventManager.h"

#include "KinectV2InputDevice.h"
#include "AllowWindowsPlatformTypes.h"
#include <windows.h>
#include <winuser.h>

#pragma comment(lib, "User32.lib" )

UKinectEventManager::UKinectEventManager(const class FPostConstructInitializeProperties& PCIP)
	: Super(PCIP), MouseInput(nullptr), IsLeftHandTracked(false), IsRightHandTracked(false)
{

}



void UKinectEventManager::ProcessNewBodyFrameEvent(const struct FBodyFrame & NewBodyFrame, TSharedRef<FGenericApplicationMessageHandler> MessageHandler)
{

	if (RawBodyFrameEvent.IsBound()){
		RawBodyFrameEvent.Broadcast(NewBodyFrame);
	}

	for (int32 i = 0; i < NewBodyFrame.Bodies.Num(); ++i)
	{

		if (NewBodyFrame.Bodies[i].bIsTracked)
		{

			TrackingIds[i] = NewBodyFrame.Bodies[i].TrackingId;

			if (AssignedKinectPlayerController.Contains(NewBodyFrame.Bodies[i].TrackingId))
			{

				if (KinectBodyEvent.IsBound()){
					KinectBodyEvent.Broadcast(AssignedKinectPlayerController[NewBodyFrame.Bodies[i].TrackingId], NewBodyFrame.Bodies[i],i);
				}


				FSlateApplication::Get().OnControllerAnalog(EKinectKeys::KinectBodyJoystickX, AssignedKinectPlayerController[NewBodyFrame.Bodies[i].TrackingId] - 1, NewBodyFrame.Bodies[i].Lean.X);
				FSlateApplication::Get().OnControllerAnalog(EKinectKeys::KinectBodyJoystickY, AssignedKinectPlayerController[NewBodyFrame.Bodies[i].TrackingId] - 1, NewBodyFrame.Bodies[i].Lean.Y);
				if (GEngine)
				{

					FVector2D ViewPortSize;

					if (GEngine->GameViewport){
						auto Veiwport = GEngine->GameViewport->Viewport;
						if (Veiwport){
							GEngine->GameViewport->GetViewportSize(ViewPortSize);

							LastLeftHandSrceenPos = CurrLeftHandScreenPos;
							CurrLeftHandScreenPos = UKinectFunctionLibrary::ConvertBodyPointToScreenPoint(NewBodyFrame.Bodies[i].KinectBones[EJoint::JointType_HandLeft].CameraSpacePoint, ViewPortSize.X, ViewPortSize.Y);
							//MessageHandler->OnTouchMoved(CursorPos, ETouchIndex::Touch1, AssignedKinectPlayerController[Body.Bodies[i].TrackingId]);
							//Viewport->SetMouse(CursorPos.X, CursorPos.Y);
							if (NewBodyFrame.Bodies[i].LeftHandState != EHandState::HandState_NotTracked){

								if (NewBodyFrame.Bodies[i].LeftHandState == EHandState::HandState_Closed)
								{
									IsLeftHandTracked = true;
									//FSlateApplication::Get().OnTouchStarted(NULL, CurrLeftHandScreenPos, ETouchIndex::Touch1, AssignedKinectPlayerController[NewBodyFrame.Bodies[i].TrackingId] - 1);
								}
								else
								{
									//FSlateApplication::Get().OnTouchEnded(LastLeftHandSrceenPos, ETouchIndex::Touch1, AssignedKinectPlayerController[NewBodyFrame.Bodies[i].TrackingId] - 1);
								}
								//if (IsLeftHandTracked && NewBodyFrame.Bodies[i].LeftHandState != EHandState::HandState_NotTracked){

								MessageHandler->OnTouchMoved(CurrLeftHandScreenPos, ETouchIndex::Touch1, AssignedKinectPlayerController[NewBodyFrame.Bodies[i].TrackingId]);

								//FSlateApplication::Get().OnTouchMoved(CurrLeftHandScreenPos, ETouchIndex::Touch1, AssignedKinectPlayerController[NewBodyFrame.Bodies[i].TrackingId] - 1);


							}
						}
					}
				}



				//if (AssignedKinectPlayerController[Body.Bodies[i].TrackingId] == MouseControllerPlayer && ControlMouse){

				//}
			}
			else
			{

				if (NewSkeletonDetectedEvent.IsBound())
					NewSkeletonDetectedEvent.Broadcast(NewBodyFrame.Bodies[i],i);
				this->NewSkeletonDetected(NewBodyFrame.Bodies[i]);
			}
		}
		else if (TrackingIds[i] != 0){

			if (AssignedKinectPlayerController.Contains(TrackingIds[i])){
				this->SkeletonLost(AssignedKinectPlayerController[TrackingIds[i]].GetValue());
				if (SkeletonLostEvent.IsBound())
				{
					SkeletonLostEvent.Broadcast(AssignedKinectPlayerController[TrackingIds[i]].GetValue());
				}
				AssignedKinectPlayerController.Remove(TrackingIds[i]);
				TrackingIds[i] = 0;
			}

		}

	}
}

void UKinectEventManager::AssigneSkeletonToPlayer(const FBody& Body, TEnumAsByte<EAutoReceiveInput::Type> Player, bool SetAsMouseController)
{

	if (Player.GetValue() != EAutoReceiveInput::Disabled && Player.GetValue() < EAutoReceiveInput::Player6)
	{

		AssignedKinectPlayerController.Add(Body.TrackingId, Player.GetValue());
		if (SetAsMouseController)
			MouseControllerPlayer = Player;
	}
}

void UKinectEventManager::BeginDestroy(){

	Super::BeginDestroy();

}

void UKinectEventManager::EnableMouseControl(bool MouseControl)
{
	ControlMouse = MouseControl;
}

#include "HideWindowsPlatformTypes.h"
