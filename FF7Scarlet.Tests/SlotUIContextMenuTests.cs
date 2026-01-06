using System.Reflection;
using System.Windows.Forms;
using FF7Scarlet.Shared.Controls;
using FF7Scarlet.Shared.Models;
using FF7Scarlet.Shared.Models.Enums;
using Shojy.FF7.Elena.Equipment;

namespace FF7Scarlet.Tests
{
    [TestFixture]
    [Apartment(ApartmentState.STA)] // Required for WinForms components
    public class SlotUIContextMenuTests
    {
        private MateriaSlotSelectorControl? control;

        [SetUp]
        public void Setup()
        {
            control = new MateriaSlotSelectorControl();
            control.SlotSelectorType = SlotSelectorType.Slots;
        }

        [TearDown]
        public void TearDown()
        {
            control?.Dispose();
        }

        private ContextMenuStrip GetContextMenu(int slotIndex)
        {
            var field = typeof(MateriaSlotSelectorControl).GetField("menuStrips", BindingFlags.NonPublic | BindingFlags.Instance);
            var menuStrips = (ContextMenuStrip[]?)field?.GetValue(control);
            return menuStrips?[slotIndex] ?? throw new InvalidOperationException("ContextMenuStrips not initialized");
        }

        private void InvokeUpdateMenu(int slotIndex)
        {
            var method = typeof(MateriaSlotSelectorControl).GetMethod("UpdateSlotSelectorType", BindingFlags.NonPublic | BindingFlags.Instance);
            method?.Invoke(control, new object[] { slotIndex });
        }

        [Test]
        [Description("TM-01 to TM-04: Verify menu item selection matches slot state")]
        [TestCase(0, MateriaSlot.None, "No slot", Description = "[UI-MENU-CH-01]")]
        [TestCase(1, MateriaSlot.NormalUnlinkedSlot, "Unlinked slot", Description = "[UI-MENU-CH-02]")]
        [TestCase(2, MateriaSlot.NormalLeftLinkedSlot, "Left linked slot", Description = "[UI-MENU-CH-03]")]
        [TestCase(3, MateriaSlot.NormalRightLinkedSlot, "Right linked slot", Description = "[UI-MENU-CH-04]")]
        public void UpdateSlotSelectorType_CheckedState_MatchesState(int index, MateriaSlot state, string expectedCheckedText)
        {
            // Arrange
            var slots = new MateriaSlot[8];
            slots[index] = state;
            control?.SetSlots(slots, GrowthRate.Normal);
            
            // Act
            InvokeUpdateMenu(index);
            var menu = GetContextMenu(index);

            // Assert
            foreach (var rawItem in menu.Items)
            {
                if (rawItem is ToolStripMenuItem item)
                {
                    if (item.Text.StartsWith(expectedCheckedText, StringComparison.OrdinalIgnoreCase))
                    {
                        Assert.That(item.Checked, Is.True, $"Item '{item.Text}' should be Checked for state {state}");
                    }
                    else
                    {
                        Assert.That(item.Checked, Is.False, $"Item '{item.Text}' should NOT be Checked");
                    }
                }
            }
        }

        [Test]
        [Description("TM-05: Verify Double Linked item is checked when state is DL")]
        public void UpdateSlotSelectorType_DoubleLinked_IsChecked()
        {
            // Arrange
            control?.EnableMultiLinkSlots(); // This adds the DL item if not present
            
            var slots = new MateriaSlot[] { 
                MateriaSlot.NormalLeftLinkedSlot, 
                MateriaSlot.NormalRightLinkedSlot, 
                MateriaSlot.NormalRightLinkedSlot, 
                MateriaSlot.None, MateriaSlot.None, MateriaSlot.None, MateriaSlot.None, MateriaSlot.None 
            };
            
            control?.SetSlots(slots, GrowthRate.Normal);
            
            // Act
            InvokeUpdateMenu(1); // Index 1 in [LL, RL, RL] is DL
            var menu = GetContextMenu(1);

            // Assert
            var dlItem = menu.Items.Cast<ToolStripItem>().OfType<ToolStripMenuItem>().FirstOrDefault(i => i.Text.Contains("Double"));
            Assert.That(dlItem, Is.Not.Null, "Double Linked item not found after EnableMultiLinkSlots");
            Assert.That(dlItem!.Checked, Is.True, "[UI-MENU-CH-05] Double Linked item should be checked");
            
            var rlItem = menu.Items.Cast<ToolStripItem>().OfType<ToolStripMenuItem>().FirstOrDefault(i => i.Text.Contains("Right"));
            Assert.That(rlItem!.Checked, Is.False, "Right Linked item should NOT be checked when it is Double Linked");
        }

        [Test]
        [Description("TM-07: Right Linked should be disabled for Slot 0")]
        public void FirstSlot_RightLinkedOption_IsDisabled()
        {
            var menu = GetContextMenu(0);
            var rlItem = menu.Items.Cast<ToolStripItem>().OfType<ToolStripMenuItem>().FirstOrDefault(i => i.Text.Contains("Right"));
            
            Assert.That(rlItem, Is.Not.Null, "Right Linked item not found");
            Assert.That(rlItem!.Enabled, Is.False, "[UI-MENU-EN-03] First slot cannot be Right Linked");
        }

        [Test]
        [Description("TM-06: Left Linked should be disabled for Slot 7")]
        public void LastSlot_LeftLinkedOption_IsDisabled()
        {
            var menu = GetContextMenu(7);
            var llItem = menu.Items.Cast<ToolStripItem>().OfType<ToolStripMenuItem>().FirstOrDefault(i => i.Text.Contains("Left"));
            
            Assert.That(llItem, Is.Not.Null, "Left Linked item not found");
            Assert.That(llItem!.Enabled, Is.False, "[UI-MENU-EN-02] Last slot cannot be Left Linked");
        }
    }
}
