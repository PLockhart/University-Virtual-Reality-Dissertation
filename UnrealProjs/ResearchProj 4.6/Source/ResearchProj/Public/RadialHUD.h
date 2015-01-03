// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include <functional>
#include "GameFramework/HUD.h"

#include "RadialHUD.generated.h"

#define MAX_RADIAL_PER_LEVEL 6

class ARadialHUD;

USTRUCT()
struct FRadialItem {

	GENERATED_USTRUCT_BODY()
	
public:
	FString DisplayText;	//the text to be displayed in the GUI
	
	//std::function<void(FRadialItem * const)> selectedEvent = nullptr;	//pointer to the function to be called when selected
	DECLARE_DELEGATE_OneParam(RadialItemDelegate, FRadialItem*const);
	RadialItemDelegate SelectedEvent;

	//METHODS
	FRadialItem(FString name) {

		DisplayText = name;
	}

	FRadialItem() {

		DisplayText = FString(TEXT("Unnamed"));
	}
	/*Called when clicked on through the GUI.
	Calls the selectedEvent method*/
	virtual void OnInteractWith(ARadialHUD * caller);
};

USTRUCT()
struct FRadialItemContainer : public FRadialItem {

	GENERATED_USTRUCT_BODY()

public:
	FRadialItemContainer(FString name)
		: FRadialItem(name) {
	}
	FRadialItemContainer() {

	}
	FRadialItem ChildItems[MAX_RADIAL_PER_LEVEL];	//the radial items this radial item contains

	/*Called when clicked on through the GUI
	Sets the caller to display its child items*/
	virtual void OnInteractWith(ARadialHUD * caller);
};


/**
 * 
 */
UCLASS(abstract)
class RESEARCHPROJ_API ARadialHUD : public AHUD
{
	GENERATED_BODY()

	//VARIABLES
public:
		FRadialItem RootItems[MAX_RADIAL_PER_LEVEL];	//the root items to be displayed
private:
	FVector _origin;	//the origin of the radial menu

	bool _isActive;	//flag for whether the radial hud is being used

	//METHODS
	/*constructor*/
public:
	ARadialHUD(const FObjectInitializer& ObjectInitializer);

	/*Sets the origin of the radial menu and opens it*/
	UFUNCTION(BlueprintCallable, Category = "RadialHUD")
	void startInteracting(FVector startPos);
	
	/*Displays the array of radial items*/
	void displayItems(FRadialItem items[MAX_RADIAL_PER_LEVEL]);

	/*select the parameter as the item clicked on*/
	UFUNCTION(BlueprintCallable, Category = "RadialHUD")
	void selectRadialItem(FRadialItem& selectedItem);

	/*dismisses the hud*/
	UFUNCTION(BlueprintCallable, Category = "RadialHUD")
	void dismissHUD();

protected:
	/*Build the root items for this radial hud.*/
	virtual void buildRootItems(FRadialItem (&itemStore)[MAX_RADIAL_PER_LEVEL]);
};