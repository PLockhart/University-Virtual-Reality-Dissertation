// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include <functional>
#include "GameFramework/HUD.h"

#include "RadialHUD.generated.h"

#define MAX_RADIAL_PER_LEVEL 4

class ARadialHUD;

USTRUCT(BlueprintType)
struct FRadialItem {

	GENERATED_USTRUCT_BODY()

private:
	//VARIABLES
	FString _displayText;	//the text to be displayed in the GUI
	bool _isStructEmpty;	//flag for whether the struct is empty or if data has been set
	
public:
	DECLARE_DELEGATE_OneParam(RadialItemDelegate, FRadialItem*const);
	RadialItemDelegate SelectedEvent;

	//METHODS
	FRadialItem(FString name) {

		_displayText = name;
		_isStructEmpty = false;
	}

	FRadialItem() {

		_displayText = FString(TEXT("Unnamed"));
		_isStructEmpty = true;
	}
	/*Called when clicked on through the GUI.
	virtual void onInteractWith(ARadialHUD * caller);

	/*Returns whether the struct is empty, or whether values have been set*/
	bool isStructEmpty() {

		return _isStructEmpty;
	}

	//UFUNCTION(BlueprintCallable, Category = "RadialItem")
	FString getDisplayName() {

		return _displayText;
	}
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
	virtual void onInteractWith(ARadialHUD * caller);
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

	virtual void BeginPlay() override;

	/*Sets the origin of the radial menu and opens it*/
	UFUNCTION(BlueprintCallable, Category = "RadialHUD")
	void startInteracting(FVector startPos);
	
	/*Displays the array of radial items.
	This will trigger the system to call the blueprint event that will
	implement the assignment of widgets*/
	void displayItems(FRadialItem items[MAX_RADIAL_PER_LEVEL]);

	/*select the parameter as the item clicked on*/
	UFUNCTION(BlueprintCallable, Category = "RadialHUD")
	void selectRadialItem(FRadialItem selectedItem);

	/*dismisses the hud*/
	UFUNCTION(BlueprintCallable, Category = "RadialHUD")
	void dismissHUD();
	
	/*Override this function in blueprints. In this function, you should
	set the visiblity of the radial gui to the parameter*/
	UFUNCTION(BlueprintNativeEvent, Category = "RadialHUD")
	void setRadialGUIWidgetVisbility(bool isVisible);
	
	/*Show the hud*/
	UFUNCTION(BlueprintCallable, Category = "RadialHUD")
	void revealHUD();

	/*Override this function in blueprints. Given the array of items,
	You use the structs data to hide buttons and set text labels*/
	UFUNCTION(BlueprintNativeEvent, Category = "RadialHUD")
	void assignWidgetsForItems(const TArray<FRadialItem> & items);

	//GETTERS 

	/*Breaks the radial item apart to reveal its variables*/
	UFUNCTION(BlueprintCallable, Category = "RadialHUD")
	void getRadialItemData(FRadialItem theItem, FString &displayName, bool &isEmpty);

	/*Gets the origin position that the radial hud started interacting at*/
	UFUNCTION(BlueprintCallable, Category = "RadialHUD")
	FVector getOriginPosition();

	/*Returns true if the radial hud is active and being used*/
	UFUNCTION(BlueprintCallable, Category = "RadialHUD")
	bool isRadialActive();


protected:
	/*Build the root items for this radial hud.
	Base class will nullify all the indexes 1st for convenience*/
	virtual void buildRootItems(FRadialItem(&itemStore)[MAX_RADIAL_PER_LEVEL]);
};