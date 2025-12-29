
using FF7Scarlet.Shared.Models;
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
        [TestCase(ExtendedMateriaType.Command, MateriaSlot.NormalUnlinkedSlot, false, "materia_slot_command1")]
        [TestCase(ExtendedMateriaType.Command, MateriaSlot.NormalLeftLinkedSlot, false, "materia_slot_command2")]
        [TestCase(ExtendedMateriaType.Command, MateriaSlot.NormalRightLinkedSlot, false, "materia_slot_command3")]
        [TestCase(ExtendedMateriaType.Command, MateriaSlot.NormalRightLinkedSlot, true, "materia_slot_command_dl")]
        [TestCase(ExtendedMateriaType.Magic, MateriaSlot.NormalUnlinkedSlot, false, "materia_slot_magic1")]
        [TestCase(ExtendedMateriaType.Magic, MateriaSlot.NormalLeftLinkedSlot, false, "materia_slot_magic2")]
        [TestCase(ExtendedMateriaType.Magic, MateriaSlot.NormalRightLinkedSlot, false, "materia_slot_magic3")]
        [TestCase(ExtendedMateriaType.Magic, MateriaSlot.NormalRightLinkedSlot, true, "materia_slot_magic_dl")]
        [TestCase(ExtendedMateriaType.Summon, MateriaSlot.NormalUnlinkedSlot, false, "materia_slot_summon1")]
        [TestCase(ExtendedMateriaType.Summon, MateriaSlot.NormalLeftLinkedSlot, false, "materia_slot_summon2")]
        [TestCase(ExtendedMateriaType.Summon, MateriaSlot.NormalRightLinkedSlot, false, "materia_slot_summon3")]
        [TestCase(ExtendedMateriaType.Summon, MateriaSlot.NormalRightLinkedSlot, true, "materia_slot_summon_dl")]
        [TestCase(ExtendedMateriaType.Support, MateriaSlot.NormalUnlinkedSlot, false, "materia_slot_support1")]
        [TestCase(ExtendedMateriaType.Support, MateriaSlot.NormalLeftLinkedSlot, false, "materia_slot_support2")]
        [TestCase(ExtendedMateriaType.Support, MateriaSlot.NormalRightLinkedSlot, false, "materia_slot_support3")]
        [TestCase(ExtendedMateriaType.Support, MateriaSlot.NormalRightLinkedSlot, true, "materia_slot_support_dl")]
        [TestCase(ExtendedMateriaType.Independent, MateriaSlot.NormalUnlinkedSlot, false, "materia_slot_independent1")]
        [TestCase(ExtendedMateriaType.Independent, MateriaSlot.NormalLeftLinkedSlot, false, "materia_slot_independent2")]
        [TestCase(ExtendedMateriaType.Independent, MateriaSlot.NormalRightLinkedSlot, false, "materia_slot_independent3")]
        [TestCase(ExtendedMateriaType.Independent, MateriaSlot.NormalRightLinkedSlot, true, "materia_slot_independent_dl")]
        public void GetSlotImageForEquippedMateriaTest(ExtendedMateriaType materiaType, MateriaSlot materiaSlot, bool isDoubleLinked, string expected)
        {
            var resourceObject = Resources.ResourceManager.GetObject(expected);
            Assert.That(resourceObject, Is.Not.Null, $"Resource '{expected}' not found");
            var expectedImage = (Image)resourceObject;
            var actualImage = SlotImage.GetSlotImageForEquippedMateria(materiaType, materiaSlot, isDoubleLinked);
            Assert.That(GetImageHash(actualImage), Is.EqualTo(GetImageHash(expectedImage)));
        }
    }
}
