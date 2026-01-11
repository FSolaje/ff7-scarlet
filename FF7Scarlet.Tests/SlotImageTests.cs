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
        public void GetSlotResource_Empty_MatchesState(MateriaSlot materiaSlot, bool isDoubleLinked, MateriaSlotResource expected)
        {
            var actualResource = SlotImage.GetSlotResource(MateriaTypeExtended.None, materiaSlot, isDoubleLinked);
            Assert.That(actualResource, Is.EqualTo(expected));
        }

        [Test]
        [TestCase(MateriaTypeExtended.Command, MateriaSlot.NormalUnlinkedSlot, false, MateriaSlotResource.materia_slot_command1, Description = "[SIMAGE-MAT-UL]")]
        [TestCase(MateriaTypeExtended.Command, MateriaSlot.EmptyUnlinkedSlot, false, MateriaSlotResource.materia_slot_command1, Description = "TV-06: Empty UL with materia")]
        [TestCase(MateriaTypeExtended.Magic, MateriaSlot.NormalLeftLinkedSlot, false, MateriaSlotResource.materia_slot_magic2, Description = "[SIMAGE-MAT-LL]")]
        [TestCase(MateriaTypeExtended.Magic, MateriaSlot.EmptyLeftLinkedSlot, false, MateriaSlotResource.materia_slot_magic2, Description = "Empty LL with materia")]
        [TestCase(MateriaTypeExtended.Summon, MateriaSlot.NormalRightLinkedSlot, false, MateriaSlotResource.materia_slot_summon3, Description = "[SIMAGE-MAT-RL]")]
        [TestCase(MateriaTypeExtended.Summon, MateriaSlot.EmptyRightLinkedSlot, false, MateriaSlotResource.materia_slot_summon3, Description = "Empty RL with materia")]
        [TestCase(MateriaTypeExtended.Support, MateriaSlot.NormalRightLinkedSlot, true, MateriaSlotResource.materia_slot_support_dl, Description = "[SIMAGE-MAT-DL]")]
        [TestCase(MateriaTypeExtended.Support, MateriaSlot.EmptyRightLinkedSlot, true, MateriaSlotResource.materia_slot_support_dl, Description = "TV-08: Empty DL with materia")]
        [TestCase(MateriaTypeExtended.Independent, MateriaSlot.NormalUnlinkedSlot, false, MateriaSlotResource.materia_slot_independent1)]
        [TestCase(MateriaTypeExtended.Independent, MateriaSlot.NormalLeftLinkedSlot, false, MateriaSlotResource.materia_slot_independent2)]
        [TestCase(MateriaTypeExtended.Independent, MateriaSlot.NormalRightLinkedSlot, false, MateriaSlotResource.materia_slot_independent3)]
        [TestCase(MateriaTypeExtended.Independent, MateriaSlot.NormalRightLinkedSlot, true, MateriaSlotResource.materia_slot_independent_dl)]
        public void GetSlotResource_WithMateria_MatchesState(MateriaTypeExtended materiaType, MateriaSlot materiaSlot, bool isDoubleLinked, MateriaSlotResource expected)
        {
            var actualResource = SlotImage.GetSlotResource(materiaType, materiaSlot, isDoubleLinked);
            Assert.That(actualResource, Is.EqualTo(expected));
        }

        [Test]
        public void EnumValues_FollowNamingConvention()
        {
            var values = Enum.GetNames(typeof(MateriaSlotResource));
            foreach (var name in values)
            {
                Assert.That(name, Does.StartWith("materia_slot"), $"Enum value '{name}' must start with 'materia_slot'");
            }
        }
    }
}