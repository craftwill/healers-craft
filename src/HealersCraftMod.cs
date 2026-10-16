using Vintagestory.API.Common;

namespace HealersCraft
{
    public class HealersCraftMod : ModSystem
    {
        public override void Start(ICoreAPI api)
        {
            api.RegisterItemClass("healerscraft.ItemMortar", typeof(ItemMortar));
        }
    }
}
