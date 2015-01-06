// Fill out your copyright notice in the Description page of Project Settings.

#include "ResearchProj.h"
#include "ResearchProjRadial.h"

AResearchProjRadial::AResearchProjRadial(const FObjectInitializer& ObjectInitializer)
: Super(ObjectInitializer) {

}

void AResearchProjRadial::buildRootItems(FRadialItem (&itemStore)[MAX_RADIAL_PER_LEVEL]) {
	
	using namespace std::placeholders; // for `_1`
	FRadialItem exitNode = FRadialItem(FString(TEXT("Exit")));
	exitNode.SelectedEvent.BindUObject(this, &AResearchProjRadial::exitNodeCallback);
	itemStore[0] = exitNode;
}

void AResearchProjRadial::exitNodeCallback(FRadialItem * const calledItem) {

	dismissHUD();
}