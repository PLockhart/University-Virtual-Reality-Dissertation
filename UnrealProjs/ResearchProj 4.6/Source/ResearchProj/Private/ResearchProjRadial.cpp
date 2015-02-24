// Fill out your copyright notice in the Description page of Project Settings.

#include "ResearchProj.h"
#include "ResearchProjRadial.h"

#include "EngineGlobals.h"
#include "Engine.h"

#define XAXISMOVEMENT 5

AResearchProjRadial::AResearchProjRadial(const FObjectInitializer& ObjectInitializer)
: Super(ObjectInitializer) {

}

void AResearchProjRadial::BeginDestroy() {

	ARadialHUD::BeginDestroy();

	for (std::vector<FRadialItem*>::iterator it = _arraysToFree.begin(); it != _arraysToFree.end(); ++it)
		delete[](*it);
}

void AResearchProjRadial::buildRootItems(TArray<FRadialItem> &itemStore) {
	
	FRadialItem exitNode = FRadialItem("Exit");
	exitNode.SelectedEvent.BindUObject(this, &AResearchProjRadial::exitNodeCallback);
	itemStore.Add(exitNode);
	
	FRadialItem * camControls = new FRadialItem[MAX_RADIAL_PER_LEVEL];
	_arraysToFree.push_back(camControls);
	camControls[0] = FRadialItem("Forward");
	camControls[0].SelectedEvent.BindUObject(this, &AResearchProjRadial::camForward);
	camControls[1] = exitNode;
	camControls[2] = FRadialItem("Reverse");
	camControls[2].SelectedEvent.BindUObject(this, &AResearchProjRadial::camBack);
	itemStore.Add(FRadialItem("CamControls", camControls));
}


void AResearchProjRadial::exitNodeCallback(FRadialItem * const calledItem) {

	dismissHUD();
}

void AResearchProjRadial::camForward(FRadialItem * const calledItem) {
	
	adjustCameraBy(FVector(XAXISMOVEMENT, 0, 0));
}

void AResearchProjRadial::camBack(FRadialItem * const calledItem) {

	adjustCameraBy(FVector(-XAXISMOVEMENT, 0, 0));
}