#include "UIManager.h"
#include "Blueprint/UserWidget.h"
#include "CameraStatusWidget.h"

AUIManager::AUIManager()
{
    PrimaryActorTick.bCanEverTick = false;
}

void AUIManager::BeginPlay()
{
    Super::BeginPlay();

    if (CameraStatusWidgetClass)
    {
        CameraStatusWidget = CreateWidget<UCameraStatusWidget>(GetWorld(), CameraStatusWidgetClass);
        if (CameraStatusWidget)
        {
            CameraStatusWidget->AddToViewport();
        }
    }
}

void AUIManager::UpdateCameraStatus(const FString& StatusMessage, float FPS)
{
    if (CameraStatusWidget)
    {
        CameraStatusWidget->UpdateStatus(StatusMessage, FPS);
    }
}
