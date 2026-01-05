using System.Drawing;
using System.Reflection;
using FF7Scarlet.Properties;
using FF7Scarlet.Shared.Models;
using FF7Scarlet.Shared.Models.Enums;
using Shojy.FF7.Elena.Equipment;
using Shojy.FF7.Elena.Materias;

namespace FF7Scarlet.Tests
{
    public class SlotGraphicalItemTests : ImageTestBase
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void CreateSlots_CreatesInstances_WithoutThrowing()
        {
            bool isMultiLinkedEnabled = true;
            // Arrange
            var slots = new MateriaSlot[8];

            // Act & Assert

            Assert.DoesNotThrow(() =>
            {
                var items = SlotGraphicalItem.CreateSlots(slots, GrowthRate.Normal, isMultiLinkedEnabled);
            });
        }

        [Test]
        public void CreateSlots_InitializesPropertiesCorrectly()
        {
            // Arrange
            bool isMultiLinkedEnabled = true;
            var slots = new MateriaSlot[8];
            slots[0] = MateriaSlot.NormalLeftLinkedSlot;
            var growth = GrowthRate.Normal;

            // Act
            var items = SlotGraphicalItem.CreateSlots(slots, growth, isMultiLinkedEnabled);
            var item = items[0];

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(item.SlotIndex, Is.EqualTo(0));
                Assert.That(item.MateriaSlotValue, Is.EqualTo(MateriaSlot.NormalLeftLinkedSlot));
                Assert.That(item.growthRate, Is.EqualTo(growth));
            });
        }

        [Test]
        public void GetMatchingImage_WithEquippedMateria_ReturnsImage()
        {
            // Arrange
            bool isMultiLinkedEnabled = true;
            var slots = new MateriaSlot[8];
            var items = SlotGraphicalItem.CreateSlots(slots, GrowthRate.Normal, isMultiLinkedEnabled);
            var item = items[1];
            var materia = new Materia { MateriaTypeByte = (byte)MateriaType.Command };

            // Act
            var image = item.GetMatchingImage(materia);

            // Assert
            Assert.That(image, Is.Not.Null);
        }

        [Test]
        public void GetMatchingImage_WithoutEquippedMateria_ReturnsImage()
        {
            // Arrange
            bool isMultiLinkedEnabled = true;
            var slots = new MateriaSlot[8];
            var items = SlotGraphicalItem.CreateSlots(slots, GrowthRate.Normal,isMultiLinkedEnabled);
            var item = items[1];

            // Act
            var image = item.GetMatchingImage();

            // Assert
            Assert.That(image, Is.Not.Null);
        }

        [Test]
        public void Linking_ForFirstSlot_WhenUnlinked_IsUnlinked()
        {
            // Arrange
            bool isMultiLinkedEnabled = true;
            var slots = new MateriaSlot[8];
            slots[0] = MateriaSlot.NormalUnlinkedSlot;
            var items = SlotGraphicalItem.CreateSlots(slots, GrowthRate.Normal, isMultiLinkedEnabled);
            var firstSlot = items[0];

            // Act
            var isUnlinked = firstSlot.IsUnlinked();

            // Assert
            Assert.That(isUnlinked, Is.True);
        }

        [TestCase(MateriaSlot.NormalRightLinkedSlot, true, TestName = "LL-RL should be linked")]
        [TestCase(MateriaSlot.NormalUnlinkedSlot, true, TestName = "LL-UL should be linked")]
        public void Linking_ForFirstSlot_WhenPaired_IsLeftLinked(MateriaSlot secondSlotType, bool expectedIsLeftLinked)
        {
            // Arrange
            bool isMultiLinkedEnabled = true;
            var slots = new MateriaSlot[8];
            slots[0] = MateriaSlot.NormalLeftLinkedSlot;
            slots[1] = secondSlotType;
            var items = SlotGraphicalItem.CreateSlots(slots, GrowthRate.Normal,isMultiLinkedEnabled);
            var firstSlot = items[0];

            // Act
            var isLeftLinked = firstSlot.IsLeftLinked();

            // Assert
            Assert.That(isLeftLinked, Is.EqualTo(expectedIsLeftLinked));
        }

        [Test]
        public void Linking_ForMiddleSlot_IsDoubleLinked()
        {
            // Arrange: LL -> RL -> RL chain
            bool isMultiLinkedEnabled = true;
            var slots = new MateriaSlot[8];
            slots[0] = MateriaSlot.NormalLeftLinkedSlot; // User def: links right
            slots[1] = MateriaSlot.NormalRightLinkedSlot; // User def: links left
            slots[2] = MateriaSlot.NormalRightLinkedSlot; // User def: links left
            var items = SlotGraphicalItem.CreateSlots(slots, GrowthRate.Normal,isMultiLinkedEnabled);
            var middleSlot = items[1];

            // Act
            var isDoubleLinked = middleSlot.IsDoubleLinked();

            // Assert
            Assert.That(isDoubleLinked, Is.True);
        }

        [Test]
        public void Linking_ForLastSlot_WhenUnlinked_IsUnlinked()
        {
            // Arrange
            bool isMultiLinkedEnabled = true;
            var slots = new MateriaSlot[8];
            slots[7] = MateriaSlot.NormalUnlinkedSlot;
            var items = SlotGraphicalItem.CreateSlots(slots, GrowthRate.Normal, isMultiLinkedEnabled);
            var lastSlot = items[7];

            // Act
            var isUnlinked = lastSlot.IsUnlinked();

            // Assert
            Assert.That(isUnlinked, Is.True);
        }

        [Test]
        public void Linking_ForLastSlot_WhenPaired_IsRightLinked()
        {
            // Arrange
            bool isMultiLinkedEnabled = true;
            var slots = new MateriaSlot[8];
            slots[6] = MateriaSlot.NormalLeftLinkedSlot;
            slots[7] = MateriaSlot.NormalRightLinkedSlot;
            var items = SlotGraphicalItem.CreateSlots(slots, GrowthRate.Normal,isMultiLinkedEnabled);
            var lastSlot = items[7];

            // Act
            var isRightLinked = lastSlot.IsRightLinked();

            // Assert
            Assert.That(isRightLinked, Is.True);
        }

        [Test]
        public void CreateSlotsAndGetMatchingImageTest()
        {
            // Arrange
            MateriaSlot[] slots =
            [
                MateriaSlot.NormalUnlinkedSlot,
                MateriaSlot.NormalUnlinkedSlot,
                MateriaSlot.NormalUnlinkedSlot,
                MateriaSlot.NormalUnlinkedSlot,
                MateriaSlot.NormalLeftLinkedSlot,
                MateriaSlot.NormalRightLinkedSlot,
                MateriaSlot.NormalRightLinkedSlot,
                MateriaSlot.NormalRightLinkedSlot
            ];
            GrowthRate growthRate = GrowthRate.Normal;

            MateriaSlotResource[] expectedResources =
            [
                MateriaSlotResource.materia_slot1,
                MateriaSlotResource.materia_slot1,
                MateriaSlotResource.materia_slot1,
                MateriaSlotResource.materia_slot1,
                MateriaSlotResource.materia_slot2,
                MateriaSlotResource.materia_slot_dl1,
                MateriaSlotResource.materia_slot_dl1,
                MateriaSlotResource.materia_slot3
            ];

            // Act
            bool isMultiLinkedEnabled = true;
            List<SlotGraphicalItem> graphicalSlots = SlotGraphicalItem.CreateSlots(slots, growthRate,isMultiLinkedEnabled);

            // Assert
            for (int i = 0; i < graphicalSlots.Count; i++)
            {
                Image actualImage = graphicalSlots[i].GetMatchingImage();
                var resourceObject = Resources.ResourceManager.GetObject(expectedResources[i].ToString());
                Assert.That(resourceObject, Is.Not.Null, $"Resource '{expectedResources[i]}' not found");
                Image expectedImage = (Image)resourceObject;
                Assert.That(GetImageHash(actualImage), Is.EqualTo(GetImageHash(expectedImage)),
                    $"Image mismatch at index {i}. Expected {expectedResources[i]}");
            }
        }
    }
}