using UnrealBuildTool;
using System.IO;

public class KinectV2Editor : ModuleRules
{
    public KinectV2Editor(TargetInfo Target)
    {
        PrivateIncludePaths.Add("KinectV2Editor/Private");

        PublicDependencyModuleNames.AddRange(
            new string[] { 
				"Core", 
				"CoreUObject", 
				"Engine", 
				"Slate",
                "UnrealEd",
				"BlueprintGraph",
                "AnimGraph",
                "KinectV2",
                "K4WLib"
			}
            );

        PrivateDependencyModuleNames.AddRange(
            new string[] {
				"Core",
				"CoreUObject",
				"Slate",
				"SlateCore",
				"Engine",
                "InputCore",
				"AssetTools",
				"UnrealEd", // for FAssetEditorManager
				"KismetWidgets",
				"GraphEditor",
				"Kismet",  // for FWorkflowCentricApplication
				"PropertyEditor",
				"RenderCore",
				"LevelEditor", // for EdModes to get a toolkit host  //@TODO: PAPER: Should be a better way to do this (see the @todo in EdModeTileMap.cpp)
				"ContentBrowser",
				"WorkspaceMenuStructure",
				"EditorStyle",
                "KinectV2",
                "K4WLib",
				"AssetTools",
				"AssetRegistry"
			});

		DynamicallyLoadedModuleNames.AddRange(
			new string[] {
				"AssetTools",
				"AssetRegistry"
			});
	
        CircularlyReferencedDependentModules.AddRange(
            new string[] {
                "UnrealEd",
                "GraphEditor",
            }
            );
    }
}