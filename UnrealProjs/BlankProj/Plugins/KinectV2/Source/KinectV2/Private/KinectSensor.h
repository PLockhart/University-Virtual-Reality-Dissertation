//------------------------------------------------------------------------------
// 
//     The Kinect for Windows APIs used here are preliminary and subject to change
// 
//------------------------------------------------------------------------------
#pragma once
#include "Core.h"
#include "KinectFunctionLibrary.h"
#include "AllowWindowsPlatformTypes.h"
#include "Kinect.h"
#include "HideWindowsPlatformTypes.h"
#include "COMPointer.h"
/**********************************************************************************************//**
 * A kinect sensor.
 *
 * @author	Leon Rosengarten
 **************************************************************************************************/

class KINECTV2_API FKinectSensor : public FRunnable
{

	static const int        cColorWidth = 1920;
	static const int        cColorHeight = 1080;
	static const int        cInfraredWidth = 512;
	static const int        cInfraredHeight = 424;

public:

	friend class UVisualGestureImporterFactory;

	friend class UVisualGesture;
	/**********************************************************************************************//**
	 * Gets the get.
	 *
	 * @author	Leon Rosengarten
	 * @return	A KinectSensor&amp;
	 **************************************************************************************************/

	//static FKinectSensor& Get();

	/**********************************************************************************************//**
	 * Gets the run.
	 *
	 * @author	Leon Rosengarten
	 * @return	An uint32.
	 **************************************************************************************************/

	virtual uint32 Run() override;

	/**********************************************************************************************//**
	 * Destructor.
	 *
	 * @author	Leon Rosengarten
	 **************************************************************************************************/

	virtual ~FKinectSensor();

	/**********************************************************************************************//**
	 * Initializes this object.
	 *
	 * @author	Leon Rosengarten
	 * @return	true if it succeeds, false if it fails.
	 **************************************************************************************************/

	virtual bool Init() override;

	/**********************************************************************************************//**
	 * Stops this object.
	 *
	 * @author	Leon Rosengarten
	 **************************************************************************************************/

	virtual void Stop() override;

	/**********************************************************************************************//**
	 * Exits this object.
	 *
	 * @author	Leon Rosengarten
	 **************************************************************************************************/

	virtual void Exit() override;

	/**********************************************************************************************//**
	 * Gets body frame.
	 *
	 * @author	Leon Rosengarten
	 * @return	The body frame.
	 **************************************************************************************************/

	virtual bool GetBodyFrame(FBodyFrame& OutBodyFrame);

	/**********************************************************************************************//**
	 * Updates the color texture described by pTexture.
	 *
	 * @author	Leon Rosengarten
	 * @param [in,out]	pTexture	If non-null, the texture.
	 **************************************************************************************************/

	virtual void UpdateColorTexture(UTexture2D* pTexture);

	/**********************************************************************************************//**
	 * Updates the infrared texture described by pTexture.
	 *
	 * @author	Leon Rosengarten
	 * @param [in,out]	pTexture	If non-null, the texture.
	 **************************************************************************************************/

	virtual void UpdateInfraredTexture(UTexture2D* pTexture);

	/**********************************************************************************************//**
	 * Updates the depth frame texture described by pTexture.
	 *
	 * @author	Leon Rosengarten
	 * @param [in,out]	pTexture	If non-null, the texture.
	 **************************************************************************************************/

	virtual void UpdateDepthFrameTexture(UTexture2D* pTexture);

	/**************************************************************************************************
	 * Body to screen.
	 *
	 * @author	Leon Rosengarten
	 * @param	bodyPoint	The body point.
	 * @param	width	 	The width.
	 * @param	height   	The height.
	 *
	 * @return	A FVector2D.
	 **************************************************************************************************/

	virtual FVector2D BodyToScreen(const FVector& bodyPoint, int32 width, int32 height);


	/**********************************************************************************************//**
	 * Default constructor.
	 *
	 * @author	Leon Rosengarten
	 **************************************************************************************************/

	FKinectSensor();

private:




	/**********************************************************************************************//**
	 * Process the body frame described by pArgs.
	 *
	 * @author	Leon Rosengarten
	 * @param [in,out]	pArgs	If non-null, the arguments.
	 **************************************************************************************************/

	void ProcessBodyFrame(IBodyFrameArrivedEventArgs* pArgs);

	/**********************************************************************************************//**
	 * Process the color frame described by pArgs.
	 *
	 * @author	Leon Rosengarten
	 * @param [in,out]	pArgs	If non-null, the arguments.
	 **************************************************************************************************/

	void ProcessColorFrame(IColorFrameArrivedEventArgs* pArgs);

	/**********************************************************************************************//**
	 * Process the infrared frame described by pArgs.
	 *
	 * @author	Leon Rosengarten
	 * @param [in,out]	pArgs	If non-null, the arguments.
	 **************************************************************************************************/

	void ProcessInfraredFrame(IInfraredFrameArrivedEventArgs* pArgs);

	/**********************************************************************************************//**
	 * Process the depth frame described by pArgs.
	 *
	 * @author	Leon Rosengarten
	 * @param [in,out]	pArgs	If non-null, the arguments.
	 **************************************************************************************************/

	void ProcessDepthFrame(IDepthFrameArrivedEventArgs* pArgs);

	/**********************************************************************************************//**
	 * Convert infrared data.
	 *
	 * @author	Leon Rosengarten
	 * @param [in,out]	pInfraredBuffer	If non-null, buffer for infrared data.
	 * @param [in,out]	pInfraredRGBX  	If non-null, the infrared rgbx.
	 **************************************************************************************************/

	void ConvertInfraredData(uint16* pInfraredBuffer, RGBQUAD* pInfraredRGBX);

	/**********************************************************************************************//**
	 * Convert depth data.
	 *
	 * @author	Leon Rosengarten
	 * @param [in,out]	pDepthBuffer	If non-null, buffer for depth data.
	 * @param [in,out]	pDepthRGBX  	If non-null, the depth rgbx.
	 * @param	minDepth				The minimum depth.
	 * @param	maxDepth				The maximum depth.
	 **************************************************************************************************/

	void ConvertDepthData(uint16* pDepthBuffer, RGBQUAD* pDepthRGBX, USHORT minDepth, USHORT maxDepth);

	/**********************************************************************************************//**
	 * Updates the texture.
	 *
	 * @author	Leon Rosengarten
	 * @param [in,out]	pTexture	If non-null, the texture.
	 * @param	pData				The data.
	 * @param	SizeX				The size x coordinate.
	 * @param	SizeY				The size y coordinate.
	 **************************************************************************************************/

	void UpdateTexture(UTexture2D* pTexture, const RGBQUAD* pData, uint32 SizeX, uint32 SizeY);

private:


	TComPtr<IKinectSensor>			m_pKinectSensor; ///< The kinect sensor

	TComPtr<IBodyFrameReader>		m_pBodyFrameReader;   ///< The body frame reader

	TComPtr<IColorFrameReader>		m_pColorFrameReader; ///< The color frame reader

	TComPtr<IDepthFrameReader>		m_pDepthFrameReader; ///< The depth frame reader

	TComPtr<IInfraredFrameReader>	m_pInfraredFrameReader;   ///< The infrared frame reader

	TComPtr<IBodyIndexFrameReader>	m_pBodyIndexFrameReader;	///< The body index frame reader

	TComPtr<IAudioBeamFrameReader>  m_pAudioBeamFrameReader;	///< The audio beam frame reader

	TComPtr<ICoordinateMapper>		m_pCoordinateMapper; ///< The coordinate mapper

	TComPtr<IKinectCoreWindow>		m_pCoreWindow;

private:


	FCriticalSection		mBodyCriticalSection;  ///< The body critical section

	FCriticalSection		mColorCriticalSection; ///< The color critical section

	FCriticalSection		mInfraredCriticalSection;  ///< The infrared critical section

	FCriticalSection		mDepthCriticalSection; ///< The depth critical section


private:


	WAITABLE_HANDLE			BodyEventHandle;	///< Handle of the body event

	WAITABLE_HANDLE			ColorEventHandle;   ///< Handle of the color event

	WAITABLE_HANDLE			DepthEventHandle;   ///< Handle of the depth event

	WAITABLE_HANDLE			InfraredEventHandle;	///< Handle of the infrared event

	WAITABLE_HANDLE			BodyIndexEventHandle;   ///< Handle of the body index event

	WAITABLE_HANDLE			TrackingIdLostEventHandle;

	WAITABLE_HANDLE			AudioBeamEventHandle;   ///< Handle of the audio beam event

	WAITABLE_HANDLE			PointerEnteredEventHandle;

	WAITABLE_HANDLE			PointerExitedEventHandle;

	WAITABLE_HANDLE			PointerMovedEventHandle;



private:


	bool					bStop;

	FBodyFrame				BodyFrame;

	RGBQUAD*				m_pColorFrameRGBX;

	RGBQUAD*				m_pInfraredFrameRGBX;

	RGBQUAD*				m_pDepthFrameRGBX;

	FRunnableThread*		pKinectThread; ///< The kinect thread

	bool					m_bNewBodyFrame = false;

	bool					m_bNewColorFrame = false;

	bool					m_bNewDepthFrame = false;

	bool					m_bNewInfraredFrame = false;


};