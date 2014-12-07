// Copyright 1998-2014 Epic Games, Inc. All Rights Reserved.

#include "CSC4001.h"
#include "CSC4001GameMode.h"
#include "CSC4001Character.h"

ACSC4001GameMode::ACSC4001GameMode(const class FPostConstructInitializeProperties& PCIP)
	: Super(PCIP)
{
	// set default pawn class to our Blueprinted character
	static ConstructorHelpers::FClassFinder<APawn> PlayerPawnBPClass(TEXT("/Game/Blueprints/MyCharacter"));
	if (PlayerPawnBPClass.Class != NULL)
	{
		DefaultPawnClass = PlayerPawnBPClass.Class;
	}
}
