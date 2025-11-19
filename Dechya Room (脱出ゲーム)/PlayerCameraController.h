
#pragma once

#include "CoreMinimal.h"
#include "GameFramework/SpringArmComponent.h"
#include "Camera/CameraComponent.h"
#include "GameFramework/Character.h"
#include "PlayerCameraController.generated.h"

UCLASS()
class YOURPROJECT_API APlayerCameraController : public ACharacter
{
    GENERATED_BODY()

public:
    APlayerCameraController();

protected:
    virtual void BeginPlay() override;
    virtual void Tick(float DeltaTime) override;

public:
    virtual void SetupPlayerInputComponent(class UInputComponent* PlayerInputComponent) override;

private:
    void Turn(float Value);
    void LookUp(float Value);

    UPROPERTY(VisibleAnywhere)
    USpringArmComponent* SpringArm;

    UPROPERTY(VisibleAnywhere)
    UCameraComponent* Camera;

    float MouseSensitivity;
};
