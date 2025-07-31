using Terraria.Audio;

namespace Mycology.Assets.Sounds
{
    public static class MiscellaneousSoundStore //just here to store soundstyles that'll be reused a lot, not sure how useful it'll be but idk
    {
        public static SoundStyle MushroomHit1 { get; } = new SoundStyle("Mycology/Assets/Sounds/MushroomHitMisc", 1, SoundType.Sound)
        {
            PitchVariance = 0.4f,
            Volume = 0.7f,
            MaxInstances = 0
        };
    }
}
