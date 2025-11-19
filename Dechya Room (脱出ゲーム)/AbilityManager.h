#pragma once

#include "CoreMinimal.h"
#include "GameFramework/GameModeBase.h"
#include "GameManager.generated.h"

UCLASS()
class YOURPROJECT_API AGameManager : public AGameModeBase
{
    GENERATED_BODY()

public:
    AGameManager();

    virtual void BeginPlay() override;

    UFUNCTION(BlueprintCallable, Category = "Game Flow")
    void StartGame();

    UFUNCTION(BlueprintCallable, Category = "Game Flow")
    bool IsGameRunning() const;

private:
    bool bIsGameRunning;
};
