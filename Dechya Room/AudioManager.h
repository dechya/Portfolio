#pragma once

#include "CoreMinimal.h"
#include "GameFramework/Actor.h"
#include "AudioManager.generated.h"

UCLASS()
class YOURPROJECT_API AAudioManager : public AActor
{
    GENERATED_BODY()

public:
    AAudioManager();

protected:
    virtual void BeginPlay() override;

public:
    UFUNCTION(BlueprintCallable, Category = "Audio")
    void SetBackgroundVolume(float Volume);

    UFUNCTION(BlueprintCallable, Category = "Audio")
    void SetEffectVolume(float Volume);

    UPROPERTY(EditAnywhere, Category = "Audio")
    class USoundMix* SoundMix;

private:
    UPROPERTY()
    class UAudioComponent* BackgroundAudioComponent;

    UPROPERTY()
    class UAudioComponent* EffectAudioComponent;
};