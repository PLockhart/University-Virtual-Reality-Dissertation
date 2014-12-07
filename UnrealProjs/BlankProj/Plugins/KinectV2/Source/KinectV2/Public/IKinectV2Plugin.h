//------------------------------------------------------------------------------
// 
//     The Kinect for Windows APIs used here are preliminary and subject to change
// 
//------------------------------------------------------------------------------

#pragma warning(disable:4653)
#pragma once
#include "KinectV2PluginStats.h"
#include "ModuleManager.h"
#include "IInputDeviceModule.h"

//#include "ICustomHardwareInputModule.h"
/**
 * The public interface to this module.  In most cases, this interface is only public to sibling modules 
 * within this plugin.
 */


extern KINECTV2_API class UKinectEventManager*			GKinectManeger;

class KINECTV2_API IKinectV2Plugin : public IInputDeviceModule
{

public:

	/**
	 * Singleton-like access to this module's interface.  This is just for convenience!
	 * Beware of calling this during the shutdown phase, though.  Your module might have been unloaded already.
	 *
	 * @return Returns singleton instance, loading the module on demand if needed
	 */
	static inline IKinectV2Plugin& Get()
	{
		return FModuleManager::LoadModuleChecked< IKinectV2Plugin >("KinectV2");
	}

	/**
	 * Checks to see if this module is loaded and ready.  It is only valid to call Get() if IsAvailable() returns true.
	 *
	 * @return True if the module is loaded and ready to use
	 */
	static inline bool IsAvailable()
	{
		return FModuleManager::Get().IsModuleLoaded( "KinectV2" );
	}
};

