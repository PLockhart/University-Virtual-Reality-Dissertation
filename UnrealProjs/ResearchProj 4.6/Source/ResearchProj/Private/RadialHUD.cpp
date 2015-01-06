// Fill out your copyright notice in the Description page of Project Settings.

#include "ResearchProj.h"
#include "RadialHUD.h"

//FRADIAL STRUCTS
void FRadialItem::onInteractWith(ARadialHUD * caller) {

	SelectedEvent.ExecuteIfBound(this);
}

void FRadialItemContainer::onInteractWith(ARadialHUD * caller) {
	
	FRadialItem::onInteractWith(caller);
	caller->displayItems(ChildItems);
}

//ARADIAL HUD

ARadialHUD::ARadialHUD(const FObjectInitializer& ObjectInitializer)
: Super(ObjectInitializer) {

}

void ARadialHUD::BeginPlay() {

	AHUD::BeginPlay();

	UE_LOG(LogTemp, Warning, TEXT("Begin Play Code"));
	buildRootItems(RootItems);
}

void ARadialHUD::startInteracting(FVector startPos) {

	_origin = startPos;
}

void ARadialHUD::displayItems(FRadialItem items[MAX_RADIAL_PER_LEVEL]) {

	TArray<FRadialItem> itemsDynamic;
	itemsDynamic.Init(0);
	
	int i = 0;
	while (i < MAX_RADIAL_PER_LEVEL) {

		//unbind it so empty buttons do nothing
		//TODO: work out if hidden buttons can be pressed so as to avoid this step
		if (items[i].isStructEmpty() == true)
			items[i].SelectedEvent.Unbind();
		else
			itemsDynamic.Add(items[i]);

		i++;
	}

	assignWidgetsForItems(itemsDynamic);
}

void ARadialHUD::selectRadialItem(FRadialItem selectedItem) {

	selectedItem.onInteractWith(this);
}

void ARadialHUD::buildRootItems(FRadialItem(&itemStore)[MAX_RADIAL_PER_LEVEL]) {

	//unreal why you no support pure virtual functions
}

void ARadialHUD::dismissHUD() {

	if (_isActive == true) {

		_isActive = false;
		setRadialGUIWidgetVisbility(false);
	}
}

void ARadialHUD::revealHUD() {

	if (_isActive == false) {

		_isActive = true;
		setRadialGUIWidgetVisbility(true);
		displayItems(RootItems);
	}
}

void ARadialHUD::setRadialGUIWidgetVisbility_Implementation(bool isVisible) {

	UE_LOG(LogTemp, Error, TEXT("Implement this method in blueprints to change the visiblity of your GUI to the parameter"));
}

void ARadialHUD::assignWidgetsForItems_Implementation(const TArray<FRadialItem> &items) {

	UE_LOG(LogTemp, Error, TEXT("Implement this method in blueprints to assign the widgets in your GUI to match up to the array's display names"));
}

void ARadialHUD::getRadialItemData(FRadialItem theItem, FString &displayName, bool &isEmpty) {

	displayName = theItem.getDisplayName();
	isEmpty = theItem.isStructEmpty();
}

FVector ARadialHUD::getOriginPosition() {

	return _origin;
}

bool ARadialHUD::isRadialActive() {

	return _isActive;
}