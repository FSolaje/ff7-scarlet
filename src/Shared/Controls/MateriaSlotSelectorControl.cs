using FF7Scarlet.KernelEditor;
using Shojy.FF7.Elena.Equipment;
using Shojy.FF7.Elena.Inventory;
using Shojy.FF7.Elena.Materias;
using System.ComponentModel;
using System.Diagnostics.Eventing.Reader;
using System.Reflection;

namespace FF7Scarlet.Shared.Controls
{
    /// <summary>
    /// Provides extension methods for enum types.
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Retrieves the description of an enum value from its <see cref="System.ComponentModel.DescriptionAttribute"/>.
        /// </summary>
        /// <param name="value">The enum value.</param>
        /// <returns>The description string from the attribute, or the enum's name if the attribute is not found.</returns>
        public static string GetDescription(this Enum value)
        {
            FieldInfo? field = value.GetType().GetField(value.ToString());
            if (field == null)
            {
                return value.ToString();
            }

            DescriptionAttribute? attribute = (DescriptionAttribute?)field.GetCustomAttribute(typeof(DescriptionAttribute));

            return attribute?.Description ?? value.ToString();
        }
    }

    public enum SlotSelectorType { Slots, Materia }


    public enum SlotMenuValue
    {
        [Description("No slot")]
        NoSlot,

        [Description("Unlinked slot")]
        Unlinked,

        [Description("Left linked slot")]
        LeftLinked,

        [Description("Right linked slot")]
        RightLinked,

        [Description("Double linked slot")]
        DoubleLinked
    }

    public partial class MateriaSlotSelectorControl : UserControl
    {
        private enum UpdateDirection { Left, Right }
        private const int SLOT_COUNT = 8;

        private SlotSelectorType slotSelectorType;
        private readonly MateriaSlot[] slots = new MateriaSlot[SLOT_COUNT];
        private readonly Materia?[] equippedMateria = new Materia?[SLOT_COUNT];
        private GrowthRate growthRate;
        private PictureBox[] pictureBoxes;
        private ContextMenuStrip[] menuStrips = new ContextMenuStrip[SLOT_COUNT];
        private (SlotMenuValue Item, int SlotIndex, MateriaSlot PervSlotValue) valueClickedForSlot;
        private int selectedSlot = -1;
        private bool multiLinkEnabled;

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
                // for (int i = 0; i < SLOT_COUNT; ++i)
                // {
                //     SetSlotInner(i, GetMatchingSlot(slots[i], i), value, true, true);
                // }
                growthRate = value;
                // InvokeDataChanged(this, EventArgs.Empty);
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
            pictureBoxes = new PictureBox[SLOT_COUNT]
            {
                pictureBoxSlot1, pictureBoxSlot2, pictureBoxSlot3, pictureBoxSlot4, pictureBoxSlot5,
                pictureBoxSlot6, pictureBoxSlot7, pictureBoxSlot8
            };
            multiLinkEnabled = DataManager.PS3TweaksEnabled;
        }

        private Image GetMatchingImage(int slotIndex, MateriaSlot slot, Materia? equipped)
        {
            if (equipped != null) //materia is equipped
            {
                var fixedSlot = GetMatchingSlot(GrowthRate.Normal, slot, slotIndex);

                if (SlotIsDoubleLinked(slotIndex))
                {
                    switch (MateriaExt.GetMateriaType(equipped.MateriaTypeByte))
                    {
                        case MateriaType.Independent:
                            return Properties.Resources.materia_slot_independent_dl;
                        case MateriaType.Support:
                            return Properties.Resources.materia_slot_support_dl;
                        case MateriaType.Magic:
                            return Properties.Resources.materia_slot_magic_dl;
                        case MateriaType.Summon:
                            return Properties.Resources.materia_slot_summon_dl;
                        case MateriaType.Command:
                            return Properties.Resources.materia_slot_command_dl;
                    }
                }
                else
                {

                    switch (fixedSlot)
                    {
                        case MateriaSlot.NormalUnlinkedSlot:
                            switch (MateriaExt.GetMateriaType(equipped.MateriaTypeByte))
                            {
                                case MateriaType.Independent:
                                    return Properties.Resources.materia_slot_independent1;
                                case MateriaType.Support:
                                    return Properties.Resources.materia_slot_support1;
                                case MateriaType.Magic:
                                    return Properties.Resources.materia_slot_magic1;
                                case MateriaType.Summon:
                                    return Properties.Resources.materia_slot_summon1;
                                case MateriaType.Command:
                                    return Properties.Resources.materia_slot_command1;
                            }
                            break;
                        case MateriaSlot.NormalLeftLinkedSlot:
                            switch (MateriaExt.GetMateriaType(equipped.MateriaTypeByte))
                            {
                                case MateriaType.Independent:
                                    return Properties.Resources.materia_slot_independent2;
                                case MateriaType.Support:
                                    return Properties.Resources.materia_slot_support2;
                                case MateriaType.Magic:
                                    return Properties.Resources.materia_slot_magic2;
                                case MateriaType.Summon:
                                    return Properties.Resources.materia_slot_summon2;
                                case MateriaType.Command:
                                    return Properties.Resources.materia_slot_command2;
                            }
                            break;
                        case MateriaSlot.NormalRightLinkedSlot:
                            switch (MateriaExt.GetMateriaType(equipped.MateriaTypeByte))
                            {
                                case MateriaType.Independent:
                                    return Properties.Resources.materia_slot_independent3;
                                case MateriaType.Support:
                                    return Properties.Resources.materia_slot_support3;
                                case MateriaType.Magic:
                                    return Properties.Resources.materia_slot_magic3;
                                case MateriaType.Summon:
                                    return Properties.Resources.materia_slot_summon3;
                                case MateriaType.Command:
                                    return Properties.Resources.materia_slot_command3;
                            }
                            break;
                    }
                }
            }
            else //no materia equipped
            {
                bool isDoubleLinked = SlotIsDoubleLinked(slotIndex);
                if (slotIndex == valueClickedForSlot.SlotIndex)
                    isDoubleLinked = valueClickedForSlot.Item == SlotMenuValue.DoubleLinked;

                if (isDoubleLinked)
                {
                    if (GrowthRate == GrowthRate.None)
                        return Properties.Resources.materia_slot_dl2;
                    else
                        return Properties.Resources.materia_slot_dl1;
                }
                else
                {
                    switch (slot)
                    {
                        case MateriaSlot.NormalUnlinkedSlot:
                            return Properties.Resources.materia_slot1;
                        case MateriaSlot.NormalLeftLinkedSlot:
                            return Properties.Resources.materia_slot2;
                        case MateriaSlot.NormalRightLinkedSlot:
                            return Properties.Resources.materia_slot3;
                        case MateriaSlot.EmptyUnlinkedSlot:
                            return Properties.Resources.materia_slot4;
                        case MateriaSlot.EmptyLeftLinkedSlot:
                            return Properties.Resources.materia_slot5;
                        case MateriaSlot.EmptyRightLinkedSlot:
                            return Properties.Resources.materia_slot6;
                    }
                }
            }
            return Properties.Resources.materia_slot0;
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
        public bool SetSlots(MateriaSlot[] slots, GrowthRate growRate)
        {
            bool success = true;
            if (GrowthRate != growRate)
                GrowthRate = growRate;

            for (int i = 0; i < SLOT_COUNT; ++i)
            {
                this.slots[i] = new MateriaSlot();
            }

            // Convert the array into a pared down version using LINQ
            // Every item is composed of the slot and its index in the original array.
            var slotsProcessedSuccessfully = slots.Select((slot, index) => new { slot, index })
                .Where(item => SetSlotInner(item.index, item.slot, growRate, true, true, true))
                .ToList()
            ;

            // Check if all slots were processed successfully
            success = slotsProcessedSuccessfully.Count == SLOT_COUNT;

            // All multilinked slots are processed internally as right-linked
            var rightSlotsList = slotsProcessedSuccessfully
                .Where(item => SlotIsRightLinked(item.slot))
                .ToList()
            ;

            //if there are multi-linked slots but they are not enabled, ask to enable them
            if (!multiLinkEnabled && rightSlotsList.Count > 1)
            {
                AskEnableMultilinkSlots();
            }

            if (multiLinkEnabled)
                rightSlotsList
                    .Where(item => SlotIsDoubleLinked(item.index))
                    .ToList()
                    .ForEach(item =>
                    {
                        // For each double-linked slot, update its visual representation (PictureBox).
                        UpdateSlotPictureBox(item.index);
                        InvokeDataChanged(this, EventArgs.Empty);
                    });

            // TODO Sería conveniente convetir esto en una función aparte.
            if (SlotSelectorType == SlotSelectorType.Slots)
            {
                slotsProcessedSuccessfully
                    .ForEach(item => UpdateSlotSelectorType(item.index));
            }

            return success;
        }

        private void UpdateSlotSelectorType(int slotIndex)
        {
            if (SlotSelectorType == SlotSelectorType.Slots)
            {
                var slotMateria = slots[slotIndex];
                var menuItems = menuStrips[slotIndex].Items;

                var isDoubleLinked = multiLinkEnabled && SlotIsDoubleLinked(slotIndex);
                Func<int, bool> isEffectiveDoubleLinked = (slotIndex) =>
                {
                    if (valueClickedForSlot.SlotIndex == slotIndex)
                        isDoubleLinked = valueClickedForSlot.Item == SlotMenuValue.DoubleLinked;

                    return isDoubleLinked;
                };

                var slotCheckConditions = new List<Func<bool>>
                {
                    (() => slotMateria == MateriaSlot.None),
                    (() => SlotIsUnlinked(slotMateria)),
                    (() => SlotIsLeftLinked(slotMateria)),
                    (() => SlotIsRightLinked(slotMateria) && !isEffectiveDoubleLinked(slotIndex)),
                    (() => isEffectiveDoubleLinked(slotIndex))
                };

                for (int itemIndex = 0; itemIndex < menuItems.Count; ++itemIndex)
                {
                    if (menuItems[itemIndex] is ToolStripMenuItem mi)
                    {
                        mi.Checked = slotCheckConditions[itemIndex]();
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
            return SetSlotInner(slot, value, GrowthRate, false, false);
        }

        private bool SetSlotInner(int slotIndex, MateriaSlot newMateriaSlot, GrowthRate growRate, bool ignoreLeft, bool ignoreRight, bool forceUpdate = false)
        {
            if (slotIndex >= 0 && slotIndex < SLOT_COUNT)
            {
                if (!forceUpdate)
                    if (valueClickedForSlot.SlotIndex == slotIndex)
                        forceUpdate = valueClickedForSlot.Item == SlotMenuValue.DoubleLinked
                            || (valueClickedForSlot.Item == SlotMenuValue.RightLinked && SlotIsDoubleLinked(slotIndex))
                        ;

                var newMateriaSlotValue = GetMatchingSlot(growRate, newMateriaSlot, slotIndex);
                if (slots[slotIndex] != newMateriaSlotValue || forceUpdate)
                {
                    var currentValue = GetMatchingSlot(growRate, slots[slotIndex], slotIndex);

                    //update slot value
                    slots[slotIndex] = newMateriaSlotValue;
                    UpdateSlotPictureBox(slotIndex);

                    if (SlotSelectorType == SlotSelectorType.Slots)
                    {
                        UpdateSlotSelectorType(slotIndex);
                    }

                    //attempt to update neighboring slot(s) as well
                    if (!ignoreLeft)
                        UpdateSlotArrayInDirection(UpdateDirection.Left, slotIndex, currentValue);
                    if (!ignoreRight)
                        UpdateSlotArrayInDirection(UpdateDirection.Right, slotIndex, currentValue);

                    InvokeDataChanged(this, EventArgs.Empty);
                    return true;
                }
            }
            return false;
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
                var pb = pictureBoxes[slotIndex];
                pb.Image = GetMatchingImage(slotIndex, slots[slotIndex], equippedMateria[slotIndex]);
            }
        }

        private void UpdateSlotArrayInDirection(UpdateDirection updateDirection, int fromSlotIndex, MateriaSlot oldRightSlotValue)
        {
            int slotIndex = updateDirection == UpdateDirection.Right ? fromSlotIndex + 1 : fromSlotIndex - 1;
            if (slotIndex >= 0 && slotIndex < SLOT_COUNT)
            {
                UpdateSlot(slotIndex, oldRightSlotValue, updateDirection);

            }
        }

        private void UpdateSlot(int slotIndex, MateriaSlot oldSlotValue, UpdateDirection updateDirection)
        {
            int lastSlotIndex = SLOT_COUNT - 1;
            if (slotIndex >= 0 && slotIndex < SLOT_COUNT)
            {
                // Define update direction.
                bool ignoreLeft = UpdateDirection.Right == updateDirection;
                bool ignoreRight = UpdateDirection.Left == updateDirection;

                int rightSlotIndex = slotIndex + 1 > lastSlotIndex ? lastSlotIndex : slotIndex + 1;
                int leftSlotIndex = slotIndex - 1 < 0 ? 0 : slotIndex - 1;

                var rightSlotValue = GetMatchingSlot(slots[rightSlotIndex], rightSlotIndex);
                var leftSlotValue = GetMatchingSlot(slots[leftSlotIndex], leftSlotIndex);
                bool isLeftSlotDoubleLinked = GetEffectiveIsLeftSlotDoubleLinked(leftSlotIndex);
                bool leftSlotHasChangeToRightLinked = GetLeftSlotHasChangeToRightLinked(leftSlotIndex, leftSlotValue);

                // Comprueba el estado de los link del slot actual.
                bool isUnlinkedFromRightSlot = IsUnlinkedFromRight(slotIndex, rightSlotIndex, rightSlotValue);
                bool isUnlinkedFromLeftSlot = IsUnlinkedFromLeft(slotIndex, leftSlotIndex, leftSlotValue, isLeftSlotDoubleLinked, leftSlotHasChangeToRightLinked, updateDirection);

                // Define el estado acutal del slot según el estado de los links.
                var linkState = GetSlotLinkState(isUnlinkedFromLeftSlot, isUnlinkedFromRightSlot, slots[slotIndex]);
                bool wasDoubleLinked = WasSlotDoubleLinked(slotIndex, oldSlotValue, updateDirection);

                if (updateDirection == UpdateDirection.Left)
                {
                    UpdateSlotFromLeft(slotIndex, ignoreLeft, ignoreRight, linkState, wasDoubleLinked);
                }
                else
                {
                    UpdateSlotFromRight(slotIndex, ignoreLeft, ignoreRight, linkState);
                }
            }
        }

        /// <summary>
        /// Calculates if the left slot should be considered as double-linked,
        /// taking into account the user's most recent action.
        /// </summary>
        /// <param name="leftSlotIndex">The index of the left slot.</param>
        /// <returns>True if the left slot is or will be double-linked; otherwise, false.</returns>
        private bool GetEffectiveIsLeftSlotDoubleLinked(int leftSlotIndex)
        {
            bool isLeftSlotDoubleLinked = SlotIsDoubleLinked(leftSlotIndex);
            if (valueClickedForSlot.SlotIndex == leftSlotIndex)
            {
                isLeftSlotDoubleLinked = isLeftSlotDoubleLinked || valueClickedForSlot.Item == SlotMenuValue.DoubleLinked;
            }
            return isLeftSlotDoubleLinked;
        }

        /// <summary>
        /// Determines if the left slot has just changed from a left-linked to a right-linked state.
        /// This is a specific edge case that breaks the link with the current slot.
        /// </summary>
        /// <param name="leftSlotIndex">The index of the left slot.</param>
        /// <param name="leftSlotValue">The current value of the left slot.</param>
        /// <returns>True if the slot has changed from a left-link to a right-link; otherwise, false.</returns>
        private bool GetLeftSlotHasChangeToRightLinked(int leftSlotIndex, MateriaSlot leftSlotValue)
        {
            bool result = false;
            if (valueClickedForSlot.SlotIndex == leftSlotIndex)
            {
                result = SlotIsLeftLinked(valueClickedForSlot.PervSlotValue)
                && SlotIsRightLinked(leftSlotValue)
                && !(valueClickedForSlot.Item == SlotMenuValue.DoubleLinked);
            }
            return result;
        }

        private void UpdateSlotFromLeft(int slotIndex, bool ignoreLeft, bool ignoreRight, SlotLinkState linkState, bool wasDoubleLinked)
        {

            if (linkState.IsRightLinked)
                // ForceUpdate because doublelinked and rightlinked has the same value.
                SetSlotInner(slotIndex, MateriaSlot.NormalRightLinkedSlot, growthRate, ignoreLeft, ignoreRight, wasDoubleLinked);
            else if (linkState.IsUnlinked && !linkState.IsNonSlot)
                SetSlotInner(slotIndex, MateriaSlot.NormalUnlinkedSlot, growthRate, ignoreLeft, ignoreRight);
            else if (linkState.IsLeftLinked)
                SetSlotInner(slotIndex, MateriaSlot.NormalLeftLinkedSlot, growthRate, ignoreLeft, ignoreRight);
            else if (linkState.IsDoubleLinked)
                SetSlotInner(slotIndex, MateriaSlot.NormalRightLinkedSlot, growthRate, ignoreLeft, ignoreRight, true);
        }

        private void UpdateSlotFromRight(int slotIndex, bool ignoreLeft, bool ignoreRight, SlotLinkState linkState)
        {
            if (linkState.IsLeftLinked)
                SetSlotInner(slotIndex, MateriaSlot.NormalLeftLinkedSlot, growthRate, ignoreLeft, ignoreRight);
            else if (linkState.IsRightLinked)
                SetSlotInner(slotIndex, MateriaSlot.NormalRightLinkedSlot, growthRate, ignoreLeft, ignoreRight);
            else if (linkState.IsUnlinked && !linkState.IsNonSlot)
                SetSlotInner(slotIndex, MateriaSlot.NormalUnlinkedSlot, growthRate, ignoreLeft, ignoreRight);
            else if (linkState.IsDoubleLinked)
                SetSlotInner(slotIndex, MateriaSlot.NormalRightLinkedSlot, growthRate, ignoreLeft, ignoreRight);
        }

        private struct SlotLinkState
        {
            public bool IsUnlinked { get; set; }
            public bool IsRightLinked { get; set; }
            public bool IsLeftLinked { get; set; }
            public bool IsDoubleLinked { get; set; }
            public bool IsNonSlot { get; set; }
        }

        private SlotLinkState GetSlotLinkState(bool isUnlinkedFromLeft, bool isUnlinkedFromRight, MateriaSlot currentSlot)
        {
            return new SlotLinkState
            {
                IsUnlinked = isUnlinkedFromLeft && isUnlinkedFromRight,
                IsRightLinked = !isUnlinkedFromLeft && isUnlinkedFromRight,
                IsLeftLinked = isUnlinkedFromLeft && !isUnlinkedFromRight,
                IsDoubleLinked = !isUnlinkedFromLeft && !isUnlinkedFromRight,
                IsNonSlot = isUnlinkedFromLeft && currentSlot == MateriaSlot.None
            };
        }

        private bool IsUnlinkedFromRight(int slotIndex, int rightSlotIndex, MateriaSlot rightSlotValue)
        {
            bool result = slotIndex == rightSlotIndex;
            result |= rightSlotValue == MateriaSlot.None;
            result |= SlotIsLeftLinked(rightSlotValue);
            result |= SlotIsUnlinked(rightSlotValue);

            return result;
        }

        private bool IsUnlinkedFromLeft(int slotIndex, int leftSlotIndex, MateriaSlot leftSlotValue, bool isLeftSlotDoubleLinked, bool leftSlotHasChangeToRightLinked, UpdateDirection updateDirection)
        {
            bool result = slotIndex == leftSlotIndex;
            result |= leftSlotValue == MateriaSlot.None;
            result |= leftSlotHasChangeToRightLinked;
            result |= !SlotIsLeftLinked(leftSlotValue) && !isLeftSlotDoubleLinked;
            result |= SlotIsUnlinked(leftSlotValue);
            result |= updateDirection == UpdateDirection.Right
                                && leftSlotIndex == valueClickedForSlot.SlotIndex
                                && valueClickedForSlot.Item == SlotMenuValue.RightLinked;

            return result;
        }

        /// <summary>
        /// Checks if a slot was double-linked before its neighbor was updated.
        /// </summary>
        /// <param name="slotIndex">The index of the slot to check.</param>
        /// <param name="oldNeighborValue">The original value of the neighboring slot.</param>
        /// <param name="updateDirection">The direction of the neighbor that was changed.</param>
        /// <returns>True if the slot was double-linked, otherwise false.</returns>
        private bool WasSlotDoubleLinked(int slotIndex, MateriaSlot oldNeighborValue, UpdateDirection updateDirection)
        {
            bool wasSlotDoubleLinked = false;
            int indexToCheck = updateDirection == UpdateDirection.Right ? slotIndex - 1 : slotIndex + 1;
            if (indexToCheck >= 0 && indexToCheck < SLOT_COUNT)
            {
                var backup = slots[indexToCheck];
                slots[indexToCheck] = oldNeighborValue;
                wasSlotDoubleLinked = SlotIsDoubleLinked(slotIndex);
                slots[indexToCheck] = backup;
            }

            return wasSlotDoubleLinked;

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
            pictureBoxes[slotIndex].Image = GetMatchingImage(slotIndex, slots[slotIndex], materia);
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

        /// <summary>
        /// Determines whether the slot at the specified index is "double linked".
        /// A slot is considered double linked if it's a right-linked slot and it is not the first or last slot,
        /// and if the previous slot is left- or right-linked and the next slot is right-linked.
        /// This method helps to identify slots that are linked on both sides, which may affect
        /// how materia can be connected or interact in the UI.
        /// </summary>
        /// <param name="index">The index of the slot to check.</param>
        /// <returns>
        /// <c>true</c> if the slot at the specified index is double linked; otherwise, <c>false</c>.
        /// </returns>
        private bool SlotIsDoubleLinked(int index, MateriaSlot? newMateriaSlot = null)
        {
            bool isDoubleLinked = false;

            if (index > 0 && index < SLOT_COUNT - 1)
            {
                var prev = slots[index - 1];
                var curr = newMateriaSlot ?? slots[index];
                var next = slots[index + 1];
                isDoubleLinked = (SlotIsLeftLinked(prev) || SlotIsRightLinked(prev)) && SlotIsRightLinked(curr) && SlotIsRightLinked(next);
            }

            return isDoubleLinked;
        }
        private bool SlotIsUnlinked(MateriaSlot slot)
        {
            return (slot == MateriaSlot.NormalUnlinkedSlot || slot == MateriaSlot.EmptyUnlinkedSlot);
        }

        private bool SlotIsLeftLinked(MateriaSlot slot)
        {
            return (slot == MateriaSlot.NormalLeftLinkedSlot || slot == MateriaSlot.EmptyLeftLinkedSlot);
        }

        private bool SlotIsRightLinked(MateriaSlot slot)
        {
            return (slot == MateriaSlot.NormalRightLinkedSlot || slot == MateriaSlot.EmptyRightLinkedSlot);
        }

        private MateriaSlot GetMatchingSlot(MateriaSlot slot, int indexSlot)
        {
            return GetMatchingSlot(GrowthRate, slot, indexSlot);
        }

        private MateriaSlot GetMatchingSlot(GrowthRate growRate, MateriaSlot slot, int indexSlot)
        {
            var outputMateriaSlot = slot;
            bool isDoubleLinked(MateriaSlot slot) => SlotIsDoubleLinked(indexSlot, slot);

            var slotTypeMatcher = new List<(Predicate<MateriaSlot> matchSlotType, Func<GrowthRate, MateriaSlot> getMateriaSlot)>
            {
                (SlotIsUnlinked, growthRate =>
                    growthRate == GrowthRate.None ? MateriaSlot.EmptyUnlinkedSlot : MateriaSlot.NormalUnlinkedSlot),
                (isDoubleLinked, growthRate =>
                    growthRate == GrowthRate.None ? MateriaSlot.EmptyRightLinkedSlot : MateriaSlot.NormalRightLinkedSlot),
                (SlotIsLeftLinked, growthRate =>
                    growthRate == GrowthRate.None ? MateriaSlot.EmptyLeftLinkedSlot : MateriaSlot.NormalLeftLinkedSlot),
                (SlotIsRightLinked, growthRate =>
                    growthRate == GrowthRate.None ? MateriaSlot.EmptyRightLinkedSlot : MateriaSlot.NormalRightLinkedSlot)
            };

            var matcher = slotTypeMatcher.FirstOrDefault(m => m.matchSlotType(slot));
            return matcher.getMateriaSlot?.Invoke(growRate) ?? outputMateriaSlot;
        }

        private int GetSlotFromSender(object sender)
        {
            if (sender is ToolStripMenuItem)
            {
                var menuItem = sender as ToolStripMenuItem;
                var toolStrip = menuItem?.GetCurrentParent() as ContextMenuStrip;
                if (toolStrip == null) { return -1; }
                return menuStrips.ToList().IndexOf(toolStrip);
            }
            else if (sender is PictureBox)
            {
                var picture = sender as PictureBox;
                if (picture == null) { return -1; }
                return pictureBoxes.ToList().IndexOf(picture);
            }
            else { return -1; }
        }

        private void EmptySlotMenu_Clicked(object? sender, EventArgs e)
        {
            if (sender != null)
            {
                int slot = GetSlotFromSender(sender);
                valueClickedForSlot = (SlotMenuValue.NoSlot, slot, slots[slot]);
                SetSlot(slot, MateriaSlot.None);
            }
        }

        private void UnlinkedSlotMenu_Clicked(object? sender, EventArgs e)
        {
            if (sender != null)
            {
                int slot = GetSlotFromSender(sender);
                valueClickedForSlot = (SlotMenuValue.Unlinked, slot, slots[slot]);
                SetSlot(slot, GetMatchingSlot(MateriaSlot.NormalUnlinkedSlot, slot));
            }
        }

        private void LeftLinkedSlotMenu_Clicked(object? sender, EventArgs e)
        {
            if (sender != null)
            {
                int slot = GetSlotFromSender(sender);
                valueClickedForSlot = (SlotMenuValue.LeftLinked, slot, slots[slot]);
                SetSlot(slot, GetMatchingSlot(MateriaSlot.NormalLeftLinkedSlot, slot));
            }
        }

        private void RightLinkedSlotMenu_Clicked(object? sender, EventArgs e)
        {
            if (sender != null)
            {
                int slot = GetSlotFromSender(sender);
                valueClickedForSlot = (SlotMenuValue.RightLinked, slot, slots[slot]);
                SetSlot(slot, GetMatchingSlot(MateriaSlot.NormalRightLinkedSlot, slot));
            }
        }

        private void DoubleLinkedSlotMenu_Clicked(object? sender, EventArgs e)
        {
            if (sender != null)
            {
                int slot = GetSlotFromSender(sender);
                valueClickedForSlot = (SlotMenuValue.DoubleLinked, slot, slots[slot]);
                SetSlot(slot, GetMatchingSlot(MateriaSlot.NormalRightLinkedSlot, slot));
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
