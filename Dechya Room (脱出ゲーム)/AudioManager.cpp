#include "AudioManager.h"
#include "Components/AudioComponent.h"
#include "Sound/SoundMix.h"

AAudioManager::AAudioManager()
{
    PrimaryActorTick.bCanEverTick = false;

    BackgroundAudioComponent = CreateDefaultSubobject<UAudioComponent>(TEXT("BackgroundAudioComponent"));
    EffectAudioComponent = CreateDefaultSubobject<UAudioComponent>(TEXT("EffectAudioComponent"));
}

void AAudioManager::BeginPlay()
{
    Super::BeginPlay();

    BackgroundAudioComponent->Play();
}

void AAudioManager::SetBackgroundVolume(float Volume)
{
    if (BackgroundAudioComponent)
    {
        BackgroundAudioComponent->SetVolumeMultiplier(Volume);
    }
}

void AAudioManager::SetEffectVolume(float Volume)
{
    if (EffectAudioComponent)
    {
        EffectAudioComponent->SetVolumeMultiplier(Volume);
    }
}