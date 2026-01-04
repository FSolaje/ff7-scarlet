using System.ComponentModel;

namespace FF7Scarlet.Shared.Models.Enums
{
    public enum SlotMenuValue
    {
        [Description("No slot")]
        NoSlot = SlotLinkType.NoSlot,

        [Description("Unlinked slot")]
        Unlinked = SlotLinkType.Unlinked,

        [Description("Left linked slot")]
        LeftLinked = SlotLinkType.LeftLinked,

        [Description("Right linked slot")]
        RightLinked = SlotLinkType.RightLinked,

        [Description("Double linked slot")]
        DoubleLinked = SlotLinkType.DoubleLinked
    }
}
