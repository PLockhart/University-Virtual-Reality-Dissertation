// Fill out your copyright notice in the Description page of Project Settings.

#include "ResearchProj.h"
#include "RadialHUD.h"

//FRADIAL STRUCTS
void FRadialItem::OnInteractWith(ARadialHUD * caller) {

	SelectedEvent.ExecuteIfBound(this);
}

void FRadialItemContainer::OnInteractWith(ARadialHUD * caller) {
	
	FRadialItem::OnInteractWith(caller);
	caller->displayItems(ChildItems);
}

//ARADIAL HUD

ARadialHUD::ARadialHUD(const FObjectInitializer& ObjectInitializer)
: Super(ObjectInitializer) {

	buildRootItems(RootItems);

	_isActive = false;
	if (this->bShowHUD == true)
		AHUD::ShowHUD();
}

void ARadialHUD::startInteracting(FVector startPos) {

	_origin = startPos;

	if (this->bShowHUD == false)
		AHUD::ShowHUD();

	_isActive = true;
}

void ARadialHUD::displayItems(FRadialItem items[MAX_RADIAL_PER_LEVEL]) {

}

void ARadialHUD::selectRadialItem(FRadialItem& selectedItem) {

	selectedItem.OnInteractWith(this);
}

void ARadialHUD::buildRootItems(FRadialItem(&itemStore)[MAX_RADIAL_PER_LEVEL]) {

	//unreal why you no support pure virtual functions
}

void ARadialHUD::dismissHUD() {

	_isActive = false;

	if (this->bShowHUD == true)
		AHUD::ShowHUD();
}