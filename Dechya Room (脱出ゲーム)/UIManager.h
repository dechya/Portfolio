#pragma once

#include "CoreMinimal.h"
#include "GameFramework/Actor.h"
#include "UIManager.generated.h"

UCLASS()
class YOURPROJECT_API AUIManager : public AActor
{
    GENERATED_BODY()

public:
    AUIManager();

protected:
    virtual void BeginPlay() override;

public:
    void UpdateCameraStatus(const FString& StatusMessage, float FPS);

private:
    UPROPERTY(EditAnywhere)
    TSubclassOf<class UUserWidget> CameraStatusWidgetClass;

    class UCameraStatusWidget* CameraStatusWidget;
};
