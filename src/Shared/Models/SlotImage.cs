using Shojy.FF7.Elena.Equipment;
using FF7Scarlet.Shared.Models.Enums;

namespace FF7Scarlet.Shared.Models
{
    public class SlotImage
    {
        private readonly static MateriaSlotResource defaultResource = MateriaSlotResource.materia_slot0;

        public static Image GetSlotImageForEmpty(MateriaSlot materiaSlot, bool isDoubleLinked)
        {
            var resource = GetSlotResourceForEmpty(materiaSlot, isDoubleLinked);
            return GetImageFromResource(resource.ToString());
        }

        public static MateriaSlotResource GetSlotResourceForEmpty(MateriaSlot materiaSlot, bool isDoubleLinked)
        {
            MateriaSlotResource emptyRightLinkedImage = isDoubleLinked ?
                MateriaSlotResource.materia_slot_dl2
                : MateriaSlotResource.materia_slot6;

            MateriaSlotResource normalRightLinkedImage = isDoubleLinked ?
                MateriaSlotResource.materia_slot_dl1
                : MateriaSlotResource.materia_slot3;

            Dictionary<MateriaSlot, MateriaSlotResource> nameDictionary = new()
            {
                { MateriaSlot.None, MateriaSlotResource.materia_slot0 },
                { MateriaSlot.NormalUnlinkedSlot, MateriaSlotResource.materia_slot1 },
                { MateriaSlot.NormalLeftLinkedSlot, MateriaSlotResource.materia_slot2 },
                { MateriaSlot.NormalRightLinkedSlot, normalRightLinkedImage },
                { MateriaSlot.EmptyUnlinkedSlot, MateriaSlotResource.materia_slot4 },
                { MateriaSlot.EmptyLeftLinkedSlot, MateriaSlotResource.materia_slot5 },
                { MateriaSlot.EmptyRightLinkedSlot, emptyRightLinkedImage }
            };

            return nameDictionary[materiaSlot];
        }

        public static Image GetSlotImageForEquippedMateria(MateriaTypeExtended materiaType, MateriaSlot materiaSlot, bool isDoubleLinkedSlot = false)
        {
            var resource = GetSlotResourceForEquippedMateria(materiaType, materiaSlot, isDoubleLinkedSlot);
            return GetImageFromResource(resource.ToString());
        }

        public static MateriaSlotResource GetSlotResourceForEquippedMateria(MateriaTypeExtended materiaType, MateriaSlot materiaSlot, bool isDoubleLinkedSlot = false)
        {
            Dictionary<MateriaTypeExtended, Func<MateriaSlot, bool, MateriaSlotResource>> resourceDictionary = new()
            {
                { MateriaTypeExtended.Command, GetCommandMateriaImageFor },
                { MateriaTypeExtended.Magic, GetMagicMateriaImageFor },
                { MateriaTypeExtended.Summon, GetMagicSummonImageFor },
                { MateriaTypeExtended.Support, GetMagicSupportImageFor },
                { MateriaTypeExtended.Independent, GetIndependentMateriaImageFor }
            };

            return resourceDictionary[materiaType].Invoke(materiaSlot, isDoubleLinkedSlot);
        }

        private static MateriaSlotResource GetCommandMateriaImageFor(MateriaSlot materiaSlot, bool isDoubleLinked)
        {
            MateriaSlotResource rightLinkedImage = isDoubleLinked ?
                MateriaSlotResource.materia_slot_command_dl
                : MateriaSlotResource.materia_slot_command3;

            Dictionary<MateriaSlot, MateriaSlotResource> materiaImages = new()
            {
                { MateriaSlot.NormalUnlinkedSlot, MateriaSlotResource.materia_slot_command1 },
                { MateriaSlot.NormalLeftLinkedSlot, MateriaSlotResource.materia_slot_command2 },
                { MateriaSlot.NormalRightLinkedSlot, rightLinkedImage },
            };

            return materiaImages.TryGetValue(materiaSlot, out MateriaSlotResource result) ? result : defaultResource;
        }

        private static MateriaSlotResource GetMagicMateriaImageFor(MateriaSlot materiaSlot, bool isDoubleLinked)
        {
            MateriaSlotResource rightLinkedImage = isDoubleLinked ?
                MateriaSlotResource.materia_slot_magic_dl
                : MateriaSlotResource.materia_slot_magic3;
            Dictionary<MateriaSlot, MateriaSlotResource> materiaImages = new()
            {
                { MateriaSlot.NormalUnlinkedSlot, MateriaSlotResource.materia_slot_magic1 },
                { MateriaSlot.NormalLeftLinkedSlot, MateriaSlotResource.materia_slot_magic2 },
                { MateriaSlot.NormalRightLinkedSlot, rightLinkedImage },
            };

            return materiaImages.TryGetValue(materiaSlot, out MateriaSlotResource result) ? result : defaultResource;
        }

        private static MateriaSlotResource GetMagicSummonImageFor(MateriaSlot materiaSlot, bool isDoubleLinked)
        {
            MateriaSlotResource rightLinkedImage = isDoubleLinked ?
                MateriaSlotResource.materia_slot_summon_dl
                : MateriaSlotResource.materia_slot_summon3;
            Dictionary<MateriaSlot, MateriaSlotResource> materiaImages = new()
            {
                { MateriaSlot.NormalUnlinkedSlot, MateriaSlotResource.materia_slot_summon1 },
                { MateriaSlot.NormalLeftLinkedSlot, MateriaSlotResource.materia_slot_summon2 },
                { MateriaSlot.NormalRightLinkedSlot, rightLinkedImage },
            };

            return materiaImages.TryGetValue(materiaSlot, out MateriaSlotResource result) ? result : defaultResource;
        }

        private static MateriaSlotResource GetMagicSupportImageFor(MateriaSlot materiaSlot, bool isDoubleLinked)
        {
            MateriaSlotResource rightLinkedImage = isDoubleLinked ?
                MateriaSlotResource.materia_slot_support_dl
                : MateriaSlotResource.materia_slot_support3;

            Dictionary<MateriaSlot, MateriaSlotResource> materiaImages = new()
            {
                { MateriaSlot.NormalUnlinkedSlot, MateriaSlotResource.materia_slot_support1 },
                { MateriaSlot.NormalLeftLinkedSlot, MateriaSlotResource.materia_slot_support2 },
                { MateriaSlot.NormalRightLinkedSlot, rightLinkedImage},
            };

            return materiaImages.TryGetValue(materiaSlot, out MateriaSlotResource result) ? result : defaultResource;
        }

        private static MateriaSlotResource GetIndependentMateriaImageFor(MateriaSlot materiaSlot, bool isDoubleLinked)
        {
            MateriaSlotResource rightLinkedImage = isDoubleLinked ?
                MateriaSlotResource.materia_slot_independent_dl
                : MateriaSlotResource.materia_slot_independent3;

            Dictionary<MateriaSlot, MateriaSlotResource> materiaImages = new()
            {
                { MateriaSlot.NormalUnlinkedSlot, MateriaSlotResource.materia_slot_independent1 },
                { MateriaSlot.NormalLeftLinkedSlot, MateriaSlotResource.materia_slot_independent2 },
                { MateriaSlot.NormalRightLinkedSlot, rightLinkedImage},
            };

            return materiaImages.TryGetValue(materiaSlot, out MateriaSlotResource result) ? result : defaultResource;
        }

        public static Image GetImageFromResource(string resourceName)
        {
            return (Image)Properties.Resources.ResourceManager.GetObject(resourceName)!;
        }

    }
}
