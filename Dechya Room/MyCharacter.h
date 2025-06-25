#pragma once

#include "CoreMinimal.h"
#include "GameFramework/Character.h"
#include "MyCharacter.generated.h"

UCLASS()
class YOURPROJECT_API AMyCharacter : public ACharacter
{
    GENERATED_BODY()

public:
    AMyCharacter();

protected:
    virtual void SetupPlayerInputComponent(class UInputComponent* PlayerInputComponent) override;

    void MoveForward(float Value);
    void MoveRight(float Value);
    void TurnAtRate(float Rate);
    void LookUpAtRate(float Rate);

    UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = Camera)
    float BaseTurnRate;

    UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = Camera)
    float BaseLookUpRate;
};
