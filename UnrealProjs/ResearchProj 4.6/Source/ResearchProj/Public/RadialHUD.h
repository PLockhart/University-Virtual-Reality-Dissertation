// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include <functional>
#include <string>
#include "GameFramework/HUD.h"

#include "RadialHUD.generated.h"

#define MAX_RADIAL_PER_LEVEL 4

class ARadialHUD;

USTRUCT(BlueprintType)
struct FRadialItem {

	GENERATED_USTRUCT_BODY()

private:
	//VARIABLES
	std::string _displayText;	//the text to be displayed in the GUI
	bool _isStructEmpty;	//flag for whether the struct is empty or if data has been set
	bool _isContainer;	//Flag for whether t his struct contains children
	FRadialItem * _childItems;	//the children this struct contains. May be null
	int _numChildren;

public:
	DECLARE_DELEGATE_OneParam(RadialItemDelegate, FRadialItem*const);
	RadialItemDelegate SelectedEvent;

	//METHODS
	//create a radial node
	FRadialItem(std::string name);
	/*create a container radial item
	*/
	FRadialItem(std::string name, FRadialItem children[MAX_RADIAL_PER_LEVEL]);
	//default non-valid constructor
	FRadialItem();

	/*adds a child to the container.
	If the item isn't a container already it will turn it into one*/
	//void addChild(FRadialItem newItem);

	/*Called when clicked on through the GUI.
	*/
	virtual void onInteractWith(ARadialHUD * caller);

	/*Returns whether the struct is empty, or whether values have been set*/
	bool isStructEmpty() const {

		return _isStructEmpty;
	}

	/*Returns whether this struct is a normal node or whether it contains children*/
	bool isStructContainer() const {

		return _isContainer;
	}

	/*Gets the children of this struct. Check to see if it is a container 1st.
	Array will be the length of MAX_RADIAL_PER_LEVEL*/
	TArray<FRadialItem*> getChildren();

	//UFUNCTION(BlueprintCallable, Category = "RadialItem")
	std::string getDisplayName() const {

		return _displayText;
	}
};

/**
 * 
 */
UCLASS(abstract)
class RESEARCHPROJ_API ARadialHUD : public AHUD
{
	GENERATED_BODY()

	//VARIABLES
protected:
	UPROPERTY()
	TArray<FRadialItem> RootItems;	//the root items to be displayed
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
	void displayItems(const TArray<FRadialItem*> & items);

	/*Displays the array of radial items.
	This will trigger the system to call the blueprint event that will
	implement the assignment of widgets*/
	void displayItems(const TArray<FRadialItem> & items);

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
	FVector getOriginPosition() const;

	/*Returns true if the radial hud is active and being used*/
	UFUNCTION(BlueprintCallable, Category = "RadialHUD")
	bool isRadialActive() const;


protected:
	/*Build the root items for this radial hud.
	Base class will nullify all the indexes 1st for convenience*/
	virtual void buildRootItems(TArray<FRadialItem> & itemStore);
};