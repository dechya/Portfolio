
#include "PlayerCameraController.h"
#include "GameFramework/SpringArmComponent.h"
#include "Camera/CameraComponent.h"

APlayerCameraController::APlayerCameraController()
{
    PrimaryActorTick.bCanEverTick = true;

    SpringArm = CreateDefaultSubobject<USpringArmComponent>(TEXT("SpringArm"));
    SpringArm->SetupAttachment(RootComponent);
    SpringArm->TargetArmLength = 300.0f;
    SpringArm->bUsePawnControlRotation = true;

    Camera = CreateDefaultSubobject<UCameraComponent>(TEXT("Camera"));
    Camera->SetupAttachment(SpringArm);
    Camera->bUsePawnControlRotation = false;

    MouseSensitivity = 1.0f;
}

void APlayerCameraController::BeginPlay()
{
    Super::BeginPlay();
}

void APlayerCameraController::Tick(float DeltaTime)
{
    Super::Tick(DeltaTime);
}

void APlayerCameraController::SetupPlayerInputComponent(UInputComponent* PlayerInputComponent)
{
    Super::SetupPlayerInputComponent(PlayerInputComponent);
    PlayerInputComponent->BindAxis("Turn", this, &APlayerCameraController::Turn);
    PlayerInputComponent->BindAxis("LookUp", this, &APlayerCameraController::LookUp);
}

void APlayerCameraController::Turn(float Value)
{
    AddControllerYawInput(Value * MouseSensitivity);
}

void APlayerCameraController::LookUp(float Value)
{
    AddControllerPitchInput(Value * MouseSensitivity);
}
