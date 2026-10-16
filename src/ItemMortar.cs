using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;

namespace HealersCraft
{
    /// <summary>
    /// Hold right mouse to grind catmint (offhand or inventory) into medicine.
    /// </summary>
    public class ItemMortar : Item
    {
        const float MaxUsageTime = 5f;
        const int AmountGivenPerUse = 1;

        static readonly AssetLocation ReadySoundLoc = new AssetLocation("healerscraft:sounds/effect/grind-fine1");
        static readonly AssetLocation UseSoundLoc = new AssetLocation("healerscraft:sounds/effect/grind-coarse1");
        const float useVolume = 0.4f;
        long soundStartMs = -1;
        ILoadedSound useSound;

        bool _step1ThresholdCompleted = false;

        public override void OnHeldInteractStart(ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel, bool firstEvent, ref EnumHandHandling handling)
        {
            handling = EnumHandHandling.PreventDefault;
            if (api.Side == EnumAppSide.Client)
            {
                _step1ThresholdCompleted = false;
                soundStartMs = api.World.ElapsedMilliseconds;
                StartUseSound();
            }
            else
            {
                // The player hears the client's stoppable copy, everyone else hears this one
                api.World.PlaySoundAt(UseSoundLoc, byEntity, (byEntity as EntityPlayer)?.Player, true, 16, useVolume);
            }
        }

        public override bool OnHeldInteractStep(float secondsUsed, ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel)
        {
            if(api.Side == EnumAppSide.Client && secondsUsed > 2.5f && !_step1ThresholdCompleted)
            {
                _step1ThresholdCompleted = true;
                api.World.PlaySoundAt(ReadySoundLoc, byEntity, (byEntity as EntityPlayer)?.Player, true, 16, useVolume);
            }

            return secondsUsed < MaxUsageTime;
        }

        public override void OnHeldInteractStop(float secondsUsed, ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel)
        {
            StopUseSound();
            if (api.Side != EnumAppSide.Server) return;
            if(secondsUsed < 3) return;

            GiveResults(byEntity);
            DamageItem(api.World, byEntity, slot);
        }

        public override bool OnHeldInteractCancel(float secondsUsed, ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel, EnumItemUseCancelReason cancelReason)
        {
            StopUseSound();
            return true;
        }

        private void GiveResults(EntityAgent toEntity)
        {
            Item medecine = api.World.GetItem(new AssetLocation("game:poultice-reed-horsetail"));
            var stack = new ItemStack(medecine, AmountGivenPerUse);
            if (!toEntity.TryGiveItemStack(stack))
            {
                api.World.SpawnItemEntity(stack, toEntity.Pos.XYZ.Add(0, 0.5, 0));
            }
        }

        void StartUseSound()
        {
            if (!(api is ICoreClientAPI capi)) return;
            StopUseSound();
            useSound = capi.World.LoadSound(new SoundParams
            {
                Location = UseSoundLoc,
                RelativePosition = true,
                Position = new Vec3f(),
                Volume = useVolume,
                Range = 16,
                DisposeOnFinish = false
            });
            useSound?.Start();
        }

        void StopUseSound()
        {
            if (useSound == null) return;
            soundStartMs = -1;
            useSound.Stop();
            useSound.Dispose();
            useSound = null;
        }
    }
}
