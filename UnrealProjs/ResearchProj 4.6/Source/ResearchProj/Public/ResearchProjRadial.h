// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "RadialHUD.h"
#include "ResearchProjRadial.generated.h"

/**
 * 
 */
UCLASS(abstract)
class RESEARCHPROJ_API AResearchProjRadial : public ARadialHUD
{
	GENERATED_BODY()
	
	//METHODS
	/*constructor*/
public:
	AResearchProjRadial(const FObjectInitializer& ObjectInitializer);

protected:
	virtual void buildRootItems(FRadialItem (&itemStore)[MAX_RADIAL_PER_LEVEL]);

private:
	/*Method called by the exit node to dismiss the hud*/
	void exitNodeCallback(FRadialItem * const calledItem);
};
