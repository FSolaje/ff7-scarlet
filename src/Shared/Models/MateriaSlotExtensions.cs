using System.CodeDom;
using Shojy.FF7.Elena.Equipment;

namespace FF7Scarlet.Shared.Models
{
    public static class MateriaSlotExtensions
    {
        public static bool IsLeftLinked(this MateriaSlot slot)
        {
            return slot == MateriaSlot.NormalLeftLinkedSlot || slot == MateriaSlot.EmptyLeftLinkedSlot;
        }

        public static bool IsRightLinked(this MateriaSlot slot)
        {
            return slot == MateriaSlot.NormalRightLinkedSlot || slot == MateriaSlot.EmptyRightLinkedSlot;
        }

        public static bool IsUnlinked(this MateriaSlot slot)
        {
            return slot == MateriaSlot.NormalUnlinkedSlot || slot == MateriaSlot.EmptyUnlinkedSlot;
        }

        public static bool IsNone(this MateriaSlot slot)
        {
            return slot == MateriaSlot.None;
        }
    }
}
