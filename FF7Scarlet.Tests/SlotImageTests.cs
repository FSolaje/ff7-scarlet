using FF7Scarlet.Shared.Models;
using FF7Scarlet.Shared.Models.Enums;
using Shojy.FF7.Elena.Equipment;
using FF7Scarlet.Properties;
using System.Drawing;

namespace FF7Scarlet.Tests
{
    [TestFixture]
    public class SlotImageTests : ImageTestBase
    {
        [Test]
        [TestCase(MateriaSlotResource.materia_slot0)]
        [TestCase(MateriaSlotResource.materia_slot_command1)]
        public void GetImageFromResourceTest(MateriaSlotResource resource)
        {
            var resourceName = resource.ToString();
            var image = SlotImage.GetImageFromResource(resourceName);
            
            var expectedObject = Resources.ResourceManager.GetObject(resourceName);
            Assert.That(image, Is.Not.Null);
            Assert.That(GetImageHash(image), Is.EqualTo(GetImageHash((Image)expectedObject!)));
        }

        [Test]
        [TestCase(MateriaSlot.None, false, MateriaSlotResource.materia_slot0)]
        [TestCase(MateriaSlot.NormalUnlinkedSlot, false, MateriaSlotResource.materia_slot1)]
        [TestCase(MateriaSlot.NormalLeftLinkedSlot, false, MateriaSlotResource.materia_slot2)]
        [TestCase(MateriaSlot.NormalRightLinkedSlot, false, MateriaSlotResource.materia_slot3)]
        [TestCase(MateriaSlot.EmptyUnlinkedSlot, false, MateriaSlotResource.materia_slot4)]
        [TestCase(MateriaSlot.EmptyLeftLinkedSlot, false, MateriaSlotResource.materia_slot5)]
        [TestCase(MateriaSlot.EmptyRightLinkedSlot, false, MateriaSlotResource.materia_slot6)]
        [TestCase(MateriaSlot.NormalRightLinkedSlot, true, MateriaSlotResource.materia_slot_dl1)]
        [TestCase(MateriaSlot.EmptyRightLinkedSlot, true, MateriaSlotResource.materia_slot_dl2)]
        public void GetSlotResourceForEmptyTest(MateriaSlot materiaSlot, bool isDoubleLinked, MateriaSlotResource expected)
        {
            var actualResource = SlotImage.GetSlotResourceForEmpty(materiaSlot, isDoubleLinked);
            Assert.That(actualResource, Is.EqualTo(expected));
        }

        [Test]
        [TestCase(MateriaTypeExtended.Command, MateriaSlot.NormalUnlinkedSlot, false, MateriaSlotResource.materia_slot_command1)]
        [TestCase(MateriaTypeExtended.Command, MateriaSlot.NormalLeftLinkedSlot, false, MateriaSlotResource.materia_slot_command2)]
        [TestCase(MateriaTypeExtended.Command, MateriaSlot.NormalRightLinkedSlot, false, MateriaSlotResource.materia_slot_command3)]
        [TestCase(MateriaTypeExtended.Command, MateriaSlot.NormalRightLinkedSlot, true, MateriaSlotResource.materia_slot_command_dl)]
        [TestCase(MateriaTypeExtended.Magic, MateriaSlot.NormalUnlinkedSlot, false, MateriaSlotResource.materia_slot_magic1)]
        [TestCase(MateriaTypeExtended.Magic, MateriaSlot.NormalLeftLinkedSlot, false, MateriaSlotResource.materia_slot_magic2)]
        [TestCase(MateriaTypeExtended.Magic, MateriaSlot.NormalRightLinkedSlot, false, MateriaSlotResource.materia_slot_magic3)]
        [TestCase(MateriaTypeExtended.Magic, MateriaSlot.NormalRightLinkedSlot, true, MateriaSlotResource.materia_slot_magic_dl)]
        [TestCase(MateriaTypeExtended.Summon, MateriaSlot.NormalUnlinkedSlot, false, MateriaSlotResource.materia_slot_summon1)]
        [TestCase(MateriaTypeExtended.Summon, MateriaSlot.NormalLeftLinkedSlot, false, MateriaSlotResource.materia_slot_summon2)]
        [TestCase(MateriaTypeExtended.Summon, MateriaSlot.NormalRightLinkedSlot, false, MateriaSlotResource.materia_slot_summon3)]
        [TestCase(MateriaTypeExtended.Summon, MateriaSlot.NormalRightLinkedSlot, true, MateriaSlotResource.materia_slot_summon_dl)]
        [TestCase(MateriaTypeExtended.Support, MateriaSlot.NormalUnlinkedSlot, false, MateriaSlotResource.materia_slot_support1)]
        [TestCase(MateriaTypeExtended.Support, MateriaSlot.NormalLeftLinkedSlot, false, MateriaSlotResource.materia_slot_support2)]
        [TestCase(MateriaTypeExtended.Support, MateriaSlot.NormalRightLinkedSlot, false, MateriaSlotResource.materia_slot_support3)]
        [TestCase(MateriaTypeExtended.Support, MateriaSlot.NormalRightLinkedSlot, true, MateriaSlotResource.materia_slot_support_dl)]
        [TestCase(MateriaTypeExtended.Independent, MateriaSlot.NormalUnlinkedSlot, false, MateriaSlotResource.materia_slot_independent1)]
        [TestCase(MateriaTypeExtended.Independent, MateriaSlot.NormalLeftLinkedSlot, false, MateriaSlotResource.materia_slot_independent2)]
        [TestCase(MateriaTypeExtended.Independent, MateriaSlot.NormalRightLinkedSlot, false, MateriaSlotResource.materia_slot_independent3)]
        [TestCase(MateriaTypeExtended.Independent, MateriaSlot.NormalRightLinkedSlot, true, MateriaSlotResource.materia_slot_independent_dl)]
        public void GetSlotResourceForEquippedMateriaTest(MateriaTypeExtended materiaType, MateriaSlot materiaSlot, bool isDoubleLinked, MateriaSlotResource expected)
        {
            var actualResource = SlotImage.GetSlotResourceForEquippedMateria(materiaType, materiaSlot, isDoubleLinked);
            Assert.That(actualResource, Is.EqualTo(expected));
        }
    }
}