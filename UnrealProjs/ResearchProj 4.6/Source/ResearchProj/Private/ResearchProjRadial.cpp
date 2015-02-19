// Fill out your copyright notice in the Description page of Project Settings.

#include "ResearchProj.h"
#include "ResearchProjRadial.h"

#include "EngineGlobals.h"
#include "Engine.h"

AResearchProjRadial::AResearchProjRadial(const FObjectInitializer& ObjectInitializer)
: Super(ObjectInitializer) {

}

void AResearchProjRadial::buildRootItems(TArray<FRadialItem> &itemStore) {
	
	FRadialItem exitNode = FRadialItem("Exit");
	exitNode.SelectedEvent.BindUObject(this, &AResearchProjRadial::exitNodeCallback);
	itemStore.Add(exitNode);
	
	FRadialItem * testChildren = new FRadialItem[MAX_RADIAL_PER_LEVEL];
	testChildren[0] = FRadialItem("1");
	testChildren[0].SelectedEvent.BindUObject(this, &AResearchProjRadial::print1);
	testChildren[1] = FRadialItem("2");
	testChildren[1].SelectedEvent.BindUObject(this, &AResearchProjRadial::print2);
	itemStore.Add(FRadialItem("Test", testChildren));
	
}

void AResearchProjRadial::exitNodeCallback(FRadialItem * const calledItem) {

	dismissHUD();
}

void AResearchProjRadial::print1(FRadialItem * const calledItem) {
	
	GEngine->AddOnScreenDebugMessage(-1, 15.0f, FColor::Red, "Test 1");
	dismissHUD();
}

void AResearchProjRadial::print2(FRadialItem * const calledItem) {

	GEngine->AddOnScreenDebugMessage(-1, 15.0f, FColor::Red, "Test 2");
	dismissHUD();
}