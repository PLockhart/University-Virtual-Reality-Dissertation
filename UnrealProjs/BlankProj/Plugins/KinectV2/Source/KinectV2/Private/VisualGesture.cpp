

#include "IKinectV2Plugin.h"
#include "VisualGesture.h"
#include "KinectV2Classes.h"
#include "KinectV2InputDevice.h"
#include "KinectSensor.h"
#include "CString.h"
#include "UnrealString.h"
#include "NameTypes.h"
#include "StringConv.h"
#include "AllowWindowsPlatformTypes.h"

#include <Kinect.Face.h>
#include <Kinect.VisualGestureBuilder.h>
#pragma comment(lib,"kinect20.lib")
#pragma comment(lib,"Kinect20.Face.lib")
#pragma comment(lib,"Kinect20.VisualGestureBuilder.lib")


#ifdef _UNICODE
#if defined _M_IX86
#pragma comment(linker,"/manifestdependency:\"type='win32' name='Microsoft.Windows.Common-Controls' version='6.0.0.0' processorArchitecture='x86' publicKeyToken='6595b64144ccf1df' language='*'\"")
#elif defined _M_X64
#pragma comment(linker,"/manifestdependency:\"type='win32' name='Microsoft.Windows.Common-Controls' version='6.0.0.0' processorArchitecture='amd64' publicKeyToken='6595b64144ccf1df' language='*'\"")
#else
#pragma comment(linker,"/manifestdependency:\"type='win32' name='Microsoft.Windows.Common-Controls' version='6.0.0.0' processorArchitecture='*' publicKeyToken='6595b64144ccf1df' language='*'\"")
#endif
#endif

FKinectVisualGestureWrapper::~FKinectVisualGestureWrapper()
{
	for (size_t i = 0; i < BODY_COUNT; i++)
	{

		//GestureSources[i].Reset();
		//GestureReaders[i].Reset();
		//GestureDatabases[i].Reset();
	}
}

UVisualGesture::UVisualGesture(const class FPostConstructInitializeProperties& PCIP) :Super(PCIP){

	KinectVisualGesture = TSharedPtr<FKinectVisualGestureWrapper>(new FKinectVisualGestureWrapper);




}

TArray<FKinectGesture> UVisualGesture::GetGestures(const FBodyFrame& BodyFrame)
{
	TArray<FKinectGesture> Result;

	if (!KinectVisualGesture->Loaded)
		PostLoad();
	for (int bodyId = 0; bodyId < BODY_COUNT; ++bodyId)
	{
		
		if (BodyFrame.Bodies[bodyId].bIsTracked)
		{
			KinectVisualGesture->GestureSources[bodyId]->put_TrackingId(BodyFrame.Bodies[bodyId].TrackingId);

			// get gesture frame
			IVisualGestureBuilderFrame*	gestureFrame = nullptr;
			if (FAILED(KinectVisualGesture->GestureReaders[bodyId]->CalculateAndAcquireLatestFrame(&gestureFrame)))
				continue;

			UINT		numGestures = 0;
			IGesture*	gestures = NULL;
			GestureType	gestureType = GestureType_None;
			KinectVisualGesture->GestureSources[bodyId]->get_GestureCount(&numGestures);
			KinectVisualGesture->GestureSources[bodyId]->get_Gestures(numGestures, &gestures);
			for (UINT gestureId = 0; gestureId < numGestures; ++gestureId)
			{
				gestures[gestureId].get_GestureType(&gestureType);
				WCHAR GestureName[200];
				gestures[gestureId].get_Name(200, GestureName);

				if (gestureType == GestureType_Discrete)
				{

					IDiscreteGestureResult*	result = NULL;
					HRESULT hr = gestureFrame->get_DiscreteGestureResult(&gestures[gestureId], &result);
					if (FAILED(hr))
						continue;
					if (result)
					{
						BOOLEAN	detected = false;
						float	confidence = 0.f;
						result->get_Detected(&detected);
						result->get_Confidence(&confidence);
						printf("BodyId: %d\tDiscrete GestureId: %d\tDetected: %d\tConfidence %.2f\n", bodyId, gestureId, detected, confidence);
					}
				}
			}
		}
	}


	return Result;
}

void UVisualGesture::PostLoad()
{

	

	auto KinectInputDevice = UKinectFunctionLibrary::GetKinectInputDevice();



	if (KinectInputDevice && Buff.Num() > 0)
	{


		// initialize gesture
		TComPtr<IVisualGestureBuilderFrameSource>	gestureSources[BODY_COUNT] = { 0 };
		TComPtr<IVisualGestureBuilderFrameReader>	gestureReaders[BODY_COUNT] = { 0 };
		TComPtr<IVisualGestureBuilderDatabase>		gestureDatabases[BODY_COUNT] = { 0 };

		for (int bodyId = 0; bodyId < BODY_COUNT; ++bodyId)
		{
			// create source
			HRESULT hr = CreateVisualGestureBuilderFrameSource(KinectInputDevice->KinectSensor->m_pKinectSensor, 0, &KinectVisualGesture->GestureSources[bodyId]);
			if (FAILED(hr))	{

				break;
			}

			// open reader
			hr = KinectVisualGesture->GestureSources[bodyId]->OpenReader(&KinectVisualGesture->GestureReaders[bodyId]);
			if (FAILED(hr))	{
				//UE_LOG(LogKinectV2Editor, Warning, TEXT("ERROR opening gesture reader."), *NameForErrors);
				//bLoadedSuccessfully = false;
				break;
			}

			// create databases
			hr = CreateVisualGestureBuilderDatabaseInstanceFromMemory(Buff.Num(), &Buff[0], &KinectVisualGesture->GestureDatabases[bodyId]);
			if (FAILED(hr))	{
				//UE_LOG(LogKinectV2Editor, Warning, TEXT("ERROR loading gesture database."), *NameForErrors);
				//bLoadedSuccessfully = false;
				break;
			}

			// add gestures to source
			uint32		numGestures = 0;
			IGesture*	gestures = NULL;
			KinectVisualGesture->GestureDatabases[bodyId]->get_AvailableGesturesCount(&numGestures);
			KinectVisualGesture->GestureDatabases[bodyId]->get_AvailableGestures(numGestures, &gestures);
			KinectVisualGesture->GestureSources[bodyId]->AddGestures(numGestures, &gestures);
		}

		KinectVisualGesture->Loaded = true;
	}
	Super::PostLoad();
}

#include "HideWindowsPlatformTypes.h"