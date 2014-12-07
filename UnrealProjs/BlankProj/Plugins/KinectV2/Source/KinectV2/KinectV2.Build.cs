//------------------------------------------------------------------------------
// 
//     The Kinect for Windows APIs used here are preliminary and subject to change
// 
//------------------------------------------------------------------------------
namespace UnrealBuildTool.Rules
{
    public class KinectV2 : ModuleRules
    {
        public KinectV2(TargetInfo Target)
        {
            PCHUsage = PCHUsageMode.NoSharedPCHs;

            PublicIncludePaths.AddRange(
                new string[] {
                    "KinectV2/Public"
					// ... add public include paths required here ...
				}
                );

            PrivateIncludePaths.AddRange(
                new string[] {
					"KinectV2/Private",
                  
					// ... add other private include paths required here ...
				}
                );

            PublicDependencyModuleNames.AddRange(
                new string[]
				{
                   	
                    "CoreUObject",				
                    "Core",
                    "Engine",
                    "InputDevice",
                    "K4WLib",
                    "MSSpeech",
                    "InputCore",
                    "Slate"
					// ... add other public dependencies that you statically link with here ...
				}
                );

            PrivateDependencyModuleNames.AddRange(
                new string[]
				{ 
                    "CoreUObject",
					"Core",
                    "Engine",
					// ... add private dependencies that you statically link with here ...
				}
                );


            DynamicallyLoadedModuleNames.AddRange(
                new string[]
				{                  
                    "K4WLib"

					// ... add any modules that your module loads dynamically here ...
				}
                );


            if (UEBuildConfiguration.bBuildEditor == true)
            {
                //@TODO: Needed for the triangulation code used for sprites (but only in editor mode)
                //@TOOD: Try to move the code dependent on the triangulation code to the editor-only module
                PrivateDependencyModuleNames.AddRange(new string [] {"UnrealEd","ContentBrowser"});
            }


        }
    }
}