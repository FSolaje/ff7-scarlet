
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
        [TestCase(MateriaSlot.None, false, "materia_slot0")]
        [TestCase(MateriaSlot.NormalUnlinkedSlot, false, "materia_slot1")]
        [TestCase(MateriaSlot.NormalLeftLinkedSlot, false, "materia_slot2")]
        [TestCase(MateriaSlot.NormalRightLinkedSlot, false, "materia_slot3")]
        [TestCase(MateriaSlot.EmptyUnlinkedSlot, false, "materia_slot4")]
        [TestCase(MateriaSlot.EmptyLeftLinkedSlot, false, "materia_slot5")]
        [TestCase(MateriaSlot.EmptyRightLinkedSlot, false, "materia_slot6")]
        [TestCase(MateriaSlot.NormalRightLinkedSlot, true, "materia_slot_dl1")]
        [TestCase(MateriaSlot.EmptyRightLinkedSlot, true, "materia_slot_dl2")]
        public void GetSlotImageForEmptyTest(MateriaSlot materiaSlot, bool isDoubleLinked, string expected)
        {
            var resourceObject = Resources.ResourceManager.GetObject(expected);
            Assert.That(resourceObject, Is.Not.Null, $"Resource '{expected}' not found");
            var expectedImage = (Image)resourceObject;
            var actualImage = SlotImage.GetSlotImageForEmpty(materiaSlot, isDoubleLinked);
            Assert.That(GetImageHash(actualImage), Is.EqualTo(GetImageHash(expectedImage)));
        }

        [Test]
        [TestCase(MateriaTypeExtended.Command, MateriaSlot.NormalUnlinkedSlot, false, "materia_slot_command1")]
        [TestCase(MateriaTypeExtended.Command, MateriaSlot.NormalLeftLinkedSlot, false, "materia_slot_command2")]
        [TestCase(MateriaTypeExtended.Command, MateriaSlot.NormalRightLinkedSlot, false, "materia_slot_command3")]
        [TestCase(MateriaTypeExtended.Command, MateriaSlot.NormalRightLinkedSlot, true, "materia_slot_command_dl")]
        [TestCase(MateriaTypeExtended.Magic, MateriaSlot.NormalUnlinkedSlot, false, "materia_slot_magic1")]
        [TestCase(MateriaTypeExtended.Magic, MateriaSlot.NormalLeftLinkedSlot, false, "materia_slot_magic2")]
        [TestCase(MateriaTypeExtended.Magic, MateriaSlot.NormalRightLinkedSlot, false, "materia_slot_magic3")]
        [TestCase(MateriaTypeExtended.Magic, MateriaSlot.NormalRightLinkedSlot, true, "materia_slot_magic_dl")]
        [TestCase(MateriaTypeExtended.Summon, MateriaSlot.NormalUnlinkedSlot, false, "materia_slot_summon1")]
        [TestCase(MateriaTypeExtended.Summon, MateriaSlot.NormalLeftLinkedSlot, false, "materia_slot_summon2")]
        [TestCase(MateriaTypeExtended.Summon, MateriaSlot.NormalRightLinkedSlot, false, "materia_slot_summon3")]
        [TestCase(MateriaTypeExtended.Summon, MateriaSlot.NormalRightLinkedSlot, true, "materia_slot_summon_dl")]
        [TestCase(MateriaTypeExtended.Support, MateriaSlot.NormalUnlinkedSlot, false, "materia_slot_support1")]
        [TestCase(MateriaTypeExtended.Support, MateriaSlot.NormalLeftLinkedSlot, false, "materia_slot_support2")]
        [TestCase(MateriaTypeExtended.Support, MateriaSlot.NormalRightLinkedSlot, false, "materia_slot_support3")]
        [TestCase(MateriaTypeExtended.Support, MateriaSlot.NormalRightLinkedSlot, true, "materia_slot_support_dl")]
        [TestCase(MateriaTypeExtended.Independent, MateriaSlot.NormalUnlinkedSlot, false, "materia_slot_independent1")]
        [TestCase(MateriaTypeExtended.Independent, MateriaSlot.NormalLeftLinkedSlot, false, "materia_slot_independent2")]
        [TestCase(MateriaTypeExtended.Independent, MateriaSlot.NormalRightLinkedSlot, false, "materia_slot_independent3")]
        [TestCase(MateriaTypeExtended.Independent, MateriaSlot.NormalRightLinkedSlot, true, "materia_slot_independent_dl")]
        public void GetSlotImageForEquippedMateriaTest(MateriaTypeExtended materiaType, MateriaSlot materiaSlot, bool isDoubleLinked, string expected)
        {
            var resourceObject = Resources.ResourceManager.GetObject(expected);
            Assert.That(resourceObject, Is.Not.Null, $"Resource '{expected}' not found");
            var expectedImage = (Image)resourceObject;
            var actualImage = SlotImage.GetSlotImageForEquippedMateria(materiaType, materiaSlot, isDoubleLinked);
            Assert.That(GetImageHash(actualImage), Is.EqualTo(GetImageHash(expectedImage)));
        }
    }
}
