using Shojy.FF7.Elena.Equipment;
using FF7Scarlet.Shared.Models.Enums;

namespace FF7Scarlet.Shared.Models
{
    public class SlotImage
    {
        private readonly static MateriaSlotResource defaultResource = MateriaSlotResource.materia_slot0;

        // Resource Naming Constants
        private const string ResourcePrefix = "materia_slot";
        
        // Suffixes for Normal Growth / With Materia
        private const string SuffixNone = "0";
        private const string SuffixUnlinked = "1";
        private const string SuffixLeftLinked = "2";
        private const string SuffixRightLinked = "3";
        private const string SuffixDoubleLinked = "_dl";

        // Suffixes for Empty Growth (No Materia)
        private const string SuffixEmptyUnlinked = "4";
        private const string SuffixEmptyLeftLinked = "5";
        private const string SuffixEmptyRightLinked = "6";
        private const string SuffixEmptyDoubleLinked = "_dl2";
        private const string SuffixNormalDoubleLinked = "_dl1"; // Specific for No Materia + Normal Growth DL

        public static Image GetSlotImage(MateriaTypeExtended materiaType, MateriaSlot materiaSlot, bool isDoubleLinked)
        {
            var resource = GetSlotResource(materiaType, materiaSlot, isDoubleLinked);
            return GetImageFromResource(resource.ToString());
        }

        public static MateriaSlotResource GetSlotResource(MateriaTypeExtended materiaType, MateriaSlot materiaSlot, bool isDoubleLinked)
        {
            bool hasMateria = materiaType != MateriaTypeExtended.None;
            string prefix = hasMateria ? $"{ResourcePrefix}_{materiaType.ToString().ToLower()}" : ResourcePrefix;
            string suffix = GetUnifiedSuffix(materiaSlot, isDoubleLinked, hasMateria);

            string resourceName = $"{prefix}{suffix}";

            if (Enum.TryParse(resourceName, out MateriaSlotResource resource))
            {
                return resource;
            }

            return defaultResource;
        }

        private static string GetUnifiedSuffix(MateriaSlot slot, bool isDL, bool hasMateria)
        {
            if (hasMateria)
            {
                if (slot.IsUnlinked()) return SuffixUnlinked;
                if (slot.IsLeftLinked()) return SuffixLeftLinked;
                if (slot.IsRightLinked()) return isDL ? SuffixDoubleLinked : SuffixRightLinked;
                return SuffixNone;
            }
            else
            {
                return slot switch
                {
                    MateriaSlot.None => SuffixNone,
                    MateriaSlot.NormalUnlinkedSlot => SuffixUnlinked,
                    MateriaSlot.NormalLeftLinkedSlot => SuffixLeftLinked,
                    MateriaSlot.NormalRightLinkedSlot => isDL ? SuffixNormalDoubleLinked : SuffixRightLinked,
                    MateriaSlot.EmptyUnlinkedSlot => SuffixEmptyUnlinked,
                    MateriaSlot.EmptyLeftLinkedSlot => SuffixEmptyLeftLinked,
                    MateriaSlot.EmptyRightLinkedSlot => isDL ? SuffixEmptyDoubleLinked : SuffixEmptyRightLinked,
                    _ => SuffixNone
                };
            }
        }

        public static Image GetImageFromResource(string resourceName)
        {
            return (Image)Properties.Resources.ResourceManager.GetObject(resourceName)!;
        }
    }
}
