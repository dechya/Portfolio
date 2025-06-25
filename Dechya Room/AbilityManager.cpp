#include "GameManager.h"

AGameManager::AGameManager()
{
    PrimaryActorTick.bCanEverTick = false;
    bIsGameRunning = false;
}

void AGameManager::BeginPlay()
{
    Super::BeginPlay();
}

void AGameManager::StartGame()
{
    bIsGameRunning = true;
    UE_LOG(LogTemp, Warning, TEXT("Game Started"));
}

bool AGameManager::IsGameRunning() const
{
    return bIsGameRunning;
}
