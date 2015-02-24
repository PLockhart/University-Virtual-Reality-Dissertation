// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "RadialHUD.h"
#include <vector>
#include "ResearchProjRadial.generated.h"

/**
 * 
 */
UCLASS(abstract)
class RESEARCHPROJ_API AResearchProjRadial : public ARadialHUD
{
	GENERATED_BODY()

	//VARIABLES
	std::vector<FRadialItem*> _arraysToFree;
	
	//METHODS
	/*constructor*/
public:
	AResearchProjRadial(const FObjectInitializer& ObjectInitializer);
	virtual void BeginDestroy();

	UFUNCTION(BlueprintImplementableEvent, Category = "Research Project HUD")
	void adjustCameraBy(FVector offset);

protected:
	virtual void buildRootItems(TArray<FRadialItem> & itemStore);

private:
	/*Method called by the exit node to dismiss the hud*/
	void exitNodeCallback(FRadialItem * const calledItem);

	void camForward(FRadialItem * const calledItem);
	void camBack(FRadialItem * const calledItem);
};
