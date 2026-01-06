using System.ComponentModel;
using System.Diagnostics;
using FF7Scarlet.KernelEditor;
using FF7Scarlet.Shared.Models;
using FF7Scarlet.Shared.Models.Enums;
using Shojy.FF7.Elena.Equipment;
using Shojy.FF7.Elena.Inventory;
using Shojy.FF7.Elena.Materias;

namespace FF7Scarlet.Shared.Controls
{
    public enum SlotSelectorType { Slots, Materia }

    public partial class MateriaSlotSelectorControl : UserControl
    {
        private const int SLOT_COUNT = 8;

        private SlotSelectorType slotSelectorType;
        private readonly MateriaSlot[] slots = new MateriaSlot[SLOT_COUNT];
        private readonly Materia?[] equippedMateria = new Materia?[SLOT_COUNT];
        private GrowthRate growthRate;
        private readonly PictureBox[] pictureBoxes;
        private readonly ContextMenuStrip[] menuStrips = new ContextMenuStrip[SLOT_COUNT];
        private TypeSelectedForSlot slotClickedInSelectorType;
        private int selectedSlot = -1;
        private bool multiLinkEnabled;
        private List<SlotGraphicalItem> graphicalSlots = [];


        public event EventHandler? SelectedSlotChanged;
        public event EventHandler? DataChanged;
        public event EventHandler? MultiLinkEnabled;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SlotSelectorType SlotSelectorType
        {
            get { return slotSelectorType; }
            set
            {
                slotSelectorType = value;
                if (value == SlotSelectorType.Slots)
                {
                    //create the context menu strips for each slot
                    for (int i = 0; i < SLOT_COUNT; ++i)
                    {
                        menuStrips[i] = new ContextMenuStrip();

                        var menuItem = new ToolStripMenuItem(SlotMenuValue.NoSlot.GetDescription());
                        menuItem.Click += new EventHandler(EmptySlotMenu_Clicked);
                        menuStrips[i].Items.Add(menuItem);

                        menuItem = new ToolStripMenuItem(SlotMenuValue.Unlinked.GetDescription());
                        menuItem.Click += new EventHandler(UnlinkedSlotMenu_Clicked);
                        menuStrips[i].Items.Add(menuItem);

                        menuItem = new ToolStripMenuItem(SlotMenuValue.LeftLinked.GetDescription());
                        menuItem.Click += new EventHandler(LeftLinkedSlotMenu_Clicked);
                        if (i == SLOT_COUNT - 1) { menuItem.Enabled = false; }
                        menuStrips[i].Items.Add(menuItem);

                        menuItem = new ToolStripMenuItem(SlotMenuValue.RightLinked.GetDescription());
                        menuItem.Click += new EventHandler(RightLinkedSlotMenu_Clicked);
                        if (i == 0) { menuItem.Enabled = false; }
                        menuStrips[i].Items.Add(menuItem);

                        if (multiLinkEnabled)
                        {
                            menuItem = new ToolStripMenuItem(SlotMenuValue.DoubleLinked.GetDescription());
                            menuItem.Click += new EventHandler(DoubleLinkedSlotMenu_Clicked);
                            if (i == 0 || i == SLOT_COUNT - 1) { menuItem.Enabled = false; }
                            menuStrips[i].Items.Add(menuItem);
                        }

                        pictureBoxes[i].ContextMenuStrip = menuStrips[i];
                    }
                }
                else
                {
                    for (int i = 0; i < SLOT_COUNT; ++i)
                    {
                        pictureBoxes[i].ContextMenuStrip = null;
                        pictureBoxes[i].Click += new EventHandler(Slot_Clicked);
                    }
                }
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GrowthRate GrowthRate
        {
            get { return growthRate; }
            set
            {
                // If slots are not initialized, just set the value and exit.
                if (graphicalSlots.Count == 0)
                {
                    growthRate = value;
                    return;
                }

                bool oldIsNone = growthRate == GrowthRate.None;
                bool newIsNone = value == GrowthRate.None;

                // Execute loop only if the 'None' state changes
                if (oldIsNone != newIsNone)
                {
                    SlotGraphicalItem.GrowthRate = value;
                    for (int i = 0; i < SLOT_COUNT; ++i)
                    {
                        graphicalSlots[i].SetInSlot(graphicalSlots[i].MateriaSlotValue, slotClickedInSelectorType, UpdateDirection.Both, true);
                    }
                }
                growthRate = value;
                InvokeDataChanged(this, EventArgs.Empty);
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int SelectedSlot
        {
            get { return selectedSlot; }
            set
            {
                selectedSlot = value;
                for (int i = 0; i < SLOT_COUNT; ++i)
                {
                    if (i == value)
                    {
                        pictureBoxes[i].BackColor = Color.LightBlue;
                    }
                    else
                    {
                        pictureBoxes[i].BackColor = Color.Transparent;
                    }
                }
                SelectedSlotChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public MateriaSlotSelectorControl()
        {
            InitializeComponent();
            pictureBoxes =
            [
                pictureBoxSlot1, pictureBoxSlot2, pictureBoxSlot3, pictureBoxSlot4, pictureBoxSlot5, pictureBoxSlot6, pictureBoxSlot7, pictureBoxSlot8
            ];

            slotClickedInSelectorType = new();
            multiLinkEnabled = DataManager.PS3TweaksEnabled;
        }

        public void EnableMultiLinkSlots()
        {
            if (!multiLinkEnabled && SlotSelectorType == SlotSelectorType.Slots)
            {
                for (int i = 0; i < SLOT_COUNT; ++i)
                {
                    var menuItem = new ToolStripMenuItem("Double linked slot");
                    menuItem.Click += new EventHandler(DoubleLinkedSlotMenu_Clicked);
                    if (i == 0 || i == SLOT_COUNT - 1) { menuItem.Enabled = false; }
                    menuStrips[i].Items.Add(menuItem);
                }
                multiLinkEnabled = true;
            }
        }

        public bool SetSlots(Weapon weapon)
        {
            return SetSlots(weapon.MateriaSlots, weapon.GrowthRate);
        }

        public bool SetSlots(Armor armor)
        {
            return SetSlots(armor.MateriaSlots, armor.GrowthRate);
        }

        // First Call to load weapon/armor slots
        public bool SetSlots(MateriaSlot[] slots, GrowthRate growthRate)
        {
            if (GrowthRate != growthRate)
                GrowthRate = growthRate;

            Array.Copy(slots, this.slots, SLOT_COUNT);
            graphicalSlots = SlotGraphicalItem.CreateSlots(this.slots, growthRate, multiLinkEnabled);

            // All multilinked slots are processed internally as right-linked
            var rightSlotsList = graphicalSlots.Where(s => s.IsRightLinked()).ToList();

            //if there are multi-linked slots but they are not enabled, ask to enable them
            if (!multiLinkEnabled && rightSlotsList.Count > 1)
            {
                AskEnableMultilinkSlots();
            }

            if (multiLinkEnabled)
            {
                graphicalSlots.Where(s => s.IsDoubleLinked())
                              .ToList()
                              .ForEach(s =>
                              {
                                  UpdateSlotPictureBox(s.SlotIndex);
                                  InvokeDataChanged(this, EventArgs.Empty);
                              });
            }
            graphicalSlots.ForEach(s => UpdateSlotPictureBox(s.SlotIndex));


            if (SlotSelectorType == SlotSelectorType.Slots)
            {
                for (int i = 0; i < SLOT_COUNT; i++)
                {
                    UpdateSlotSelectorType(i);
                }
            }

            return true;
        }

        private void UpdateSlotSelectorType(int slotIndex)
        {
            if (SlotSelectorType == SlotSelectorType.Slots)
            {
                var menuItems = menuStrips[slotIndex].Items;

                var slot = graphicalSlots[slotIndex];
                bool isDoubleLinked = multiLinkEnabled && slot.IsDoubleLinked();
                bool isRightLinked = slot.IsRightLinked() && !isDoubleLinked;
                bool isLeftLinked = slot.IsLeftLinked();
                bool isUnlinked = slot.IsUnlinked();
                bool isNone = slot.IsNonSlot();

                var slotCheckValues = new List<bool> { isNone, isUnlinked, isLeftLinked, isRightLinked, isDoubleLinked };

                for (int itemIndex = 0; itemIndex < menuItems.Count; ++itemIndex)
                {
                    if (menuItems[itemIndex] is ToolStripMenuItem mi)
                    {
                        mi.Checked = slotCheckValues[itemIndex];
                    }
                }
            }
        }

        //if multi-linked slots are not enabled, ask to enable them
        private bool AskEnableMultilinkSlots()
        {
            bool hasEnabledMultilinks = false;
            var result = MessageBox.Show("This kernel file appears to use multi-linked materia slots! Would you like to enable Postscriptthree Tweaks?",
                "Enable Postscriptthree Tweaks?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                DataManager.PS3TweaksEnabled = true;
                EnableMultiLinkSlots();
                MultiLinkEnabled?.Invoke(this, EventArgs.Empty);
                hasEnabledMultilinks = true;
            }
            return hasEnabledMultilinks;
        }

        public bool SetSlot(int slot, MateriaSlot value)
        {
            var slotGraphical = graphicalSlots[slot];
            bool result = slotGraphical.SetInSlot(value, slotClickedInSelectorType, UpdateDirection.Both);

			// TODO Delete this Debug block when UpdateSlotPictureBos was resolved
            Debug.WriteLine("----------");
            graphicalSlots.ForEach(slot => Debug.WriteLine(
                "["+slot.SlotIndex+"] " + slot.MateriaSlotValue + " DL: " + slot.IsDoubleLinked()
                )
            );

            graphicalSlots.ForEach(graphicalSlot => UpdateSlotPictureBox(graphicalSlot.SlotIndex));
            graphicalSlots.ForEach(graphicalSlot => UpdateSlotSelectorType(graphicalSlot.SlotIndex));
            InvokeDataChanged(this, EventArgs.Empty);
            return result;
        }

        /// <summary>
        /// Updates the image displayed in the PictureBox corresponding to the specified slot index.
        /// Sets the PictureBox's image based on the slot's state and the equipped materia.
        /// </summary>
        /// <param name="slotIndex">
        /// The index of the slot to update. Must be within the valid range of slot indices.
        /// </param>
        private void UpdateSlotPictureBox(int slotIndex)
        {
            if (slotIndex >= 0 && slotIndex < SLOT_COUNT)
            {
                var slotGraphicalItem = graphicalSlots[slotIndex];
                var pb = pictureBoxes[slotIndex];
                pb.Image = slotGraphicalItem.GetMatchingImage();
            }
        }

        public void SetMateria(InventoryMateria[] materia, Kernel kernel)
        {
            for (int i = 0; i < 8; ++i)
            {
                SetMateria(i, materia[i], kernel);
                SelectedSlot = -1;
            }
        }

        public void SetMateria(int slotIndex, Materia? materia)
        {
            equippedMateria[slotIndex] = materia;
            var slotGraphicalItem = graphicalSlots[slotIndex];
            graphicalSlots.ForEach(slot => UpdateSlotSelectorType(slot.SlotIndex));
            pictureBoxes[slotIndex].Image = slotGraphicalItem.GetMatchingImage(materia);
            InvokeDataChanged(this, EventArgs.Empty);
        }

        public void SetMateria(int slot, InventoryMateria materia, Kernel kernel)
        {
            var mat = kernel.GetMateriaByID(materia.Index);
            SetMateria(slot, mat);
        }

        public MateriaSlot[] GetSlots()
        {
            var s = new MateriaSlot[SLOT_COUNT];
            Array.Copy(slots, s, SLOT_COUNT);
            return s;
        }

        public InventoryMateria[] GetMateria()
        {
            var m = new InventoryMateria[SLOT_COUNT];
            Array.Copy(equippedMateria, m, SLOT_COUNT);
            return m;
        }

        private int GetSlotFromSender(object sender)
        {
            int result = -1;

            if (sender is ToolStripMenuItem menuItem && menuItem.GetCurrentParent() is ContextMenuStrip toolStrip)
            {
                result = Array.IndexOf(menuStrips, toolStrip);
            }
            else if (sender is PictureBox picture)
            {
                result = Array.IndexOf(pictureBoxes, picture);
            }

            return result;
        }

        private void EmptySlotMenu_Clicked(object? sender, EventArgs e)
        {
            if (sender != null)
            {
                int slot = GetSlotFromSender(sender);
                slotClickedInSelectorType = new(slot, SlotMenuValue.NoSlot, slots[slot]);
                SetSlot(slot, MateriaSlot.None);
            }
        }

        private void UnlinkedSlotMenu_Clicked(object? sender, EventArgs e)
        {
            if (sender != null)
            {
                int slot = GetSlotFromSender(sender);
                slotClickedInSelectorType = new(slot, SlotMenuValue.Unlinked, slots[slot]);
                SetSlot(slot, MateriaSlot.NormalUnlinkedSlot);
            }
        }

        private void LeftLinkedSlotMenu_Clicked(object? sender, EventArgs e)
        {
            if (sender != null)
            {
                int slot = GetSlotFromSender(sender);
                slotClickedInSelectorType = new(slot, SlotMenuValue.LeftLinked, slots[slot]);
                SetSlot(slot, MateriaSlot.NormalLeftLinkedSlot);
            }
        }

        private void RightLinkedSlotMenu_Clicked(object? sender, EventArgs e)
        {
            if (sender != null)
            {
                int slot = GetSlotFromSender(sender);
                slotClickedInSelectorType = new(slot, SlotMenuValue.RightLinked, slots[slot]);
                SetSlot(slot, MateriaSlot.NormalRightLinkedSlot);
            }
        }

        private void DoubleLinkedSlotMenu_Clicked(object? sender, EventArgs e)
        {
            if (sender != null)
            {
                int slot = GetSlotFromSender(sender);
                slotClickedInSelectorType = new(slot, SlotMenuValue.DoubleLinked, slots[slot]);
                SetSlot(slot, MateriaSlot.NormalRightLinkedSlot);
            }
        }

        private void Slot_Clicked(object? sender, EventArgs e)
        {
            if (sender != null)
            {
                var slot = GetSlotFromSender(sender);
                if (slots[slot] != MateriaSlot.None)
                {
                    SelectedSlot = slot;
                }
            }
        }

        private void InvokeDataChanged(object? sender, EventArgs e)
        {
            DataChanged?.Invoke(sender, e);
        }
    }
}
