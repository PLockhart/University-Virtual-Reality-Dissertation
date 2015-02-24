// Fill out your copyright notice in the Description page of Project Settings.

#include "ResearchProj.h"
#include "RadialHUD.h"

#include <iostream>

//FRADIAL STRUCTS

//METHODS
FRadialItem::FRadialItem(std::string name) {

	_displayText = name;
	_isStructEmpty = false;
	_isContainer = false;
	_numChildren = 0;
}

FRadialItem::FRadialItem(std::string name, FRadialItem children[MAX_RADIAL_PER_LEVEL]) {

	_numChildren = 0;
	for (int i = 0; i < MAX_RADIAL_PER_LEVEL; i++) {

		if (children[i].isStructEmpty() == false)
			_numChildren++;
	}

	_childItems = children;

	_displayText = name;
	_isStructEmpty = false;
	_isContainer = true;
}


FRadialItem::FRadialItem() {

	_displayText = "Unnamed";
	_isStructEmpty = true;
	_isContainer = false;
	_numChildren = 0;
}
/*
void FRadialItem::addChild(FRadialItem newItem) {

	if (isStructContainer() == false) {

		_numChildren = 0;
		_childItems = new FRadialItem[MAX_RADIAL_PER_LEVEL];
		_isContainer = true;
	}

	if (_numChildren < MAX_RADIAL_PER_LEVEL) {

		_childItems[_numChildren] = newItem;
		_numChildren++;
	}
}
*/

void FRadialItem::onInteractWith(ARadialHUD * caller) {

	SelectedEvent.ExecuteIfBound(this);

	if (isStructContainer() == true) {

		TArray<FRadialItem*> items = getChildren();
		caller->displayItems(items);
	}
}

TArray<FRadialItem*> FRadialItem::getChildren() {

	TArray<FRadialItem*> c;
	c.Init(0);

	if (isStructContainer() == true) {

		for (int i = 0; i < _numChildren; i++) {
			c.Add(&_childItems[i]);
		}
	}
	else
		UE_LOG(LogTemp, Error, TEXT("Trying to get children from non-container FRadial HUD"));

	return c;
}

//ARADIAL HUD

ARadialHUD::ARadialHUD(const FObjectInitializer& ObjectInitializer)
: Super(ObjectInitializer) {

}

void ARadialHUD::BeginPlay() {

	AHUD::BeginPlay();

	RootItems.Init(0);
	buildRootItems(RootItems);
}

void ARadialHUD::startInteracting(FVector startPos) {

	_origin = startPos;
}

void ARadialHUD::displayItems(const TArray<FRadialItem> & items) {

	if (items.Num() > MAX_RADIAL_PER_LEVEL)
		UE_LOG(LogTemp, Error, TEXT("Trying to display items more than the max radial per level"));

	assignWidgetsForItems(items);
}

void ARadialHUD::displayItems(const TArray<FRadialItem*> & items) {

	TArray<FRadialItem> itemsDynamic;
	itemsDynamic.Init(0);

	int i = 0;
	while (i < MAX_RADIAL_PER_LEVEL && i < items.Num()) {

		FRadialItem * loopedItem = items[i];

		//unbind it so empty buttons do nothing
		if (loopedItem->isStructEmpty() == false)
			itemsDynamic.Add(*loopedItem);

		i++;
	}

	displayItems(itemsDynamic);
}

void ARadialHUD::selectRadialItem(FRadialItem selectedItem) {

	selectedItem.onInteractWith(this);
}

void ARadialHUD::buildRootItems(TArray<FRadialItem> & itemStore) {

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

	displayName = FString(theItem.getDisplayName().c_str());
	isEmpty = theItem.isStructEmpty();
}

FVector ARadialHUD::getOriginPosition() const {

	return _origin;
}

bool ARadialHUD::isRadialActive() const {

	return _isActive;
}