using Shojy.FF7.Elena.Equipment;
using Shojy.FF7.Elena.Materias;

namespace FF7Scarlet.Shared.Models
{
    public class SlotGraphicalItem
    {
        private static readonly List<SlotGraphicalItem> AllSlots = [];
        public int SlotIndex { get; private set; }
        private MateriaSlot slotLinkType;
        public ExtendedMateriaType materiaType;
        public GrowthRate growthRate;
        private TypeSelectedForSlot lastSelectionType;
        private readonly SlotLinkState slotLinkState;
        private readonly MateriaSlot[] slotsArray;

        private SlotGraphicalItem(int slotIndex, MateriaSlot[] slotsArray, GrowthRate growthRate)
        {
            this.SlotIndex = slotIndex;
            this.slotsArray = slotsArray;
            this.slotLinkType = slotsArray[slotIndex];
            this.growthRate = growthRate;
            this.lastSelectionType = new(-1, SlotMenuValue.NoSlot, MateriaSlot.None);
            slotLinkState = new SlotLinkState(this);
            this.materiaType = ExtendedMateriaType.None;
        }

        public static List<SlotGraphicalItem> CreateSlots(MateriaSlot[] slotsArray, GrowthRate growthRate)
        {
            AllSlots.Clear();

            for (int index = 0; index < slotsArray.Length; index++)
            {
                var newSlot = new SlotGraphicalItem(index, slotsArray, growthRate);
                AllSlots.Add(newSlot);
            }

            AllSlots.ForEach(slot => slot.slotLinkState.SetState());

            return AllSlots;
        }

        private SlotGraphicalItem? LeftSlot
        {
            get
            {
                if (SlotIndex > 0)
                {
                    return AllSlots[SlotIndex - 1];
                }
                return null;
            }
        }

        private SlotGraphicalItem? RightSlot
        {
            get
            {
                if (SlotIndex < AllSlots.Count - 1)
                {
                    return AllSlots[SlotIndex + 1];
                }
                return null;
            }
        }

        public MateriaSlot MateriaSlot
        {
            get { return slotLinkType; }
            set
            {
                slotLinkType = value;
                slotsArray[SlotIndex] = value;
            }
        }

        private class SlotLinkState
        {
            private readonly SlotGraphicalItem slot;
            public bool IsUnlinked { get; set; }
            public bool IsRightLinked { get; set; }
            public bool IsLeftLinked { get; set; }
            public bool IsDoubleLinked { get; set; }
            public bool IsNonSlot { get; set; }

            public SlotLinkState(SlotGraphicalItem slot)
            {
                this.slot = slot;
                SetState();
            }

            public void SetState()
            {
                bool isUnlinkedFromLeft = slot.IsUnlinkedFromLeft();
                bool isUnlinkedFromRight = slot.IsUnlinkedFromRight();

                DeterminateSlotState(isUnlinkedFromLeft, isUnlinkedFromRight);
            }

            private void DeterminateSlotState(bool isUnlinkedFromLeft, bool isUnlinkedFromRight)
            {
                IsUnlinked = isUnlinkedFromLeft && isUnlinkedFromRight;
                IsRightLinked = !isUnlinkedFromLeft && isUnlinkedFromRight;
                IsLeftLinked = isUnlinkedFromLeft && !isUnlinkedFromRight;
                IsDoubleLinked = !isUnlinkedFromLeft && !isUnlinkedFromRight;
                IsNonSlot = isUnlinkedFromLeft && slot.slotLinkType == MateriaSlot.None;
            }
        }

        private bool IsUnlinkedFromRight()
        {
            return RightSlot?.slotLinkType is null 
                or MateriaSlot.None 
                or MateriaSlot.EmptyLeftLinkedSlot 
                or MateriaSlot.NormalLeftLinkedSlot
                or MateriaSlot.NormalUnlinkedSlot; 
        }

        private bool IsUnlinkedFromLeft(UpdateDirection updateDirection = UpdateDirection.None)
        {
            bool isUnlinkedFromLeft = LeftSlot?.slotLinkType is null 
                or MateriaSlot.None 
                or MateriaSlot.NormalUnlinkedSlot 
                or MateriaSlot.EmptyUnlinkedSlot;

            if (LeftSlot != null && updateDirection == UpdateDirection.Right)
                {
                    bool leftSlotWasSelected = LeftSlot.SlotIndex == lastSelectionType.SlotIndex;
                    bool selectionWasRightLinked = lastSelectionType.Item == SlotMenuValue.RightLinked;
                    isUnlinkedFromLeft |= leftSlotWasSelected && selectionWasRightLinked;
            }
            return isUnlinkedFromLeft;
        }

        public Image GetMatchingImage(Materia? equipped = null)
        {
            bool isDoubleLinkedSlot = IsDoubleLinked();

            if (equipped != null)
            {
                var fixedSlot = GetMatchingSlot();
                var materiaType = Materia.GetMateriaType(equipped.MateriaTypeByte);
                var extendedMateriaType = materiaType.ToExtendedMateriaType(); // Using extension method

                if (extendedMateriaType == ExtendedMateriaType.None)
                {
                    return SlotImage.GetSlotImageForEmpty(slotLinkType, isDoubleLinkedSlot);
                }

                return SlotImage.GetSlotImageForEquippedMateria(extendedMateriaType, fixedSlot, isDoubleLinkedSlot);
            }
            else
            {
                return SlotImage.GetSlotImageForEmpty(slotLinkType, isDoubleLinkedSlot);
            }
        }

        public MateriaSlot GetMatchingSlot(MateriaSlot? newMateriaSlot = null)
        {
            var outputMateriaSlot = newMateriaSlot ?? MateriaSlot;
            bool isDoubleLinked() => IsDoubleLinked();

            var slotTypeMatcher = new List<(Func<bool> condition, Func<GrowthRate, MateriaSlot> getMateriaSlot)>
            {
                (IsUnlinked, growthRate =>
                    growthRate == GrowthRate.None ? MateriaSlot.EmptyUnlinkedSlot : MateriaSlot.NormalUnlinkedSlot),
                (isDoubleLinked, growthRate =>
                    growthRate == GrowthRate.None ? MateriaSlot.EmptyRightLinkedSlot : MateriaSlot.NormalRightLinkedSlot),
                (IsLeftLinked, growthRate =>
                    growthRate == GrowthRate.None ? MateriaSlot.EmptyLeftLinkedSlot : MateriaSlot.NormalLeftLinkedSlot),
                (IsRightLinked, growthRate =>
                    growthRate == GrowthRate.None ? MateriaSlot.EmptyRightLinkedSlot : MateriaSlot.NormalRightLinkedSlot)
            };

            var matcher = slotTypeMatcher.FirstOrDefault(m => m.condition());
            return matcher.getMateriaSlot?.Invoke(growthRate) ?? outputMateriaSlot;
        }

        public bool SetInSlot(MateriaSlot newMateriaSlot, TypeSelectedForSlot slotClickedInSelectorType, UpdateDirection updateDirection, bool forceUpdate = false)
        {
            lastSelectionType = slotClickedInSelectorType;
            if (SlotIndex >= 0 && SlotIndex < slotsArray.Length)
            {
                if (!forceUpdate)
                    if (lastSelectionType.SlotIndex == SlotIndex)
                        forceUpdate = HasChangeSlotToDoubleLinked()
                            || HasChangedSlotFomDoubleLinkedToRightLinked()
                        ;

                var newMateriaSlotValue = GetMatchingSlot(newMateriaSlot);
                if (MateriaSlot != newMateriaSlotValue || forceUpdate)
                {
                    var currentValue = newMateriaSlot;

                    MateriaSlot = newMateriaSlot;

                    if (updateDirection == UpdateDirection.Left || updateDirection == UpdateDirection.Both)
                        UpdateSlotArrayForward(UpdateDirection.Left, currentValue);

                    if (updateDirection == UpdateDirection.Right || updateDirection == UpdateDirection.Both)
                        UpdateSlotArrayForward(UpdateDirection.Right, currentValue);

                    return true;
                }
            }
            return false;
        }

        private bool HasChangeSlotToDoubleLinked()
        {
            return lastSelectionType.Item == SlotMenuValue.DoubleLinked;
        }

        private bool HasChangedSlotFomDoubleLinkedToRightLinked()
        {
            bool hasChanged = lastSelectionType.Item == SlotMenuValue.RightLinked
                    && IsDoubleLinked();

            return hasChanged;
        }

        public void UpdateSlotArrayForward(UpdateDirection updateDirection, MateriaSlot oldRightSlotValue)
        {
            if (SlotIndex - 1 >= 0 && SlotIndex + 1 < slotsArray.Length)
            {
                SlotGraphicalItem nextSlotIndex = updateDirection == UpdateDirection.Right ? RightSlot : LeftSlot;
                nextSlotIndex.UpdateSlot(oldRightSlotValue, updateDirection);
            }
        }

        private void UpdateSlot(MateriaSlot oldSlotValue, UpdateDirection updateDirection)
        {
            if (SlotIndex >= 0 && SlotIndex < slotsArray.Length)
            {
                bool wasDoubleLinked = WasSlotDoubleLinked(oldSlotValue, updateDirection);

                if (updateDirection == UpdateDirection.Left)
                {
                    UpdateSlotToLeft(slotLinkState, wasDoubleLinked);
                }
                else
                {
                    UpdateSlotToRight(slotLinkState);
                }
            }
        }

        private bool WasSlotDoubleLinked(MateriaSlot oldNeighborValue, UpdateDirection updateDirection)
        {
            bool wasSlotDoubleLinked = false;
            SlotGraphicalItem slotToCheck = updateDirection == UpdateDirection.Right ? LeftSlot : RightSlot;

            if (slotToCheck != null)
            {
                var backup = slotToCheck.MateriaSlot;
                slotToCheck.MateriaSlot = oldNeighborValue;
                wasSlotDoubleLinked = slotToCheck.IsDoubleLinked();
                slotToCheck.MateriaSlot = backup;
            }

            return wasSlotDoubleLinked;
        }

        private void UpdateSlotToLeft(SlotLinkState linkState, bool wasDoubleLinked)
        {
            if (linkState.IsRightLinked)
                SetInSlot(MateriaSlot.NormalRightLinkedSlot, lastSelectionType, UpdateDirection.Left, wasDoubleLinked);
            else if (linkState.IsUnlinked && !linkState.IsNonSlot)
                SetInSlot(MateriaSlot.NormalUnlinkedSlot, lastSelectionType, UpdateDirection.Left);
            else if (linkState.IsLeftLinked)
                SetInSlot(MateriaSlot.NormalLeftLinkedSlot, lastSelectionType, UpdateDirection.Left);
            else if (linkState.IsDoubleLinked)
                SetInSlot(MateriaSlot.NormalRightLinkedSlot, lastSelectionType, UpdateDirection.Left, true);
        }

        private void UpdateSlotToRight(SlotLinkState linkState)
        {
            if (linkState.IsLeftLinked)
                SetInSlot(MateriaSlot.NormalLeftLinkedSlot, lastSelectionType, UpdateDirection.Right);
            else if (linkState.IsRightLinked)
                SetInSlot(MateriaSlot.NormalRightLinkedSlot, lastSelectionType, UpdateDirection.Right);
            else if (linkState.IsUnlinked && !linkState.IsNonSlot)
                SetInSlot(MateriaSlot.NormalUnlinkedSlot, lastSelectionType, UpdateDirection.Right);
            else if (linkState.IsDoubleLinked)
                SetInSlot(MateriaSlot.NormalRightLinkedSlot, lastSelectionType, UpdateDirection.Right);
        }


        public bool IsDoubleLinked(MateriaSlot? newMateriaSlot = null)
        {
            return slotLinkState.IsDoubleLinked;
        }

        public bool IsUnlinked()
        {
            return slotLinkState.IsUnlinked;
        }

        public bool IsLeftLinked()
        {
            return slotLinkState.IsLeftLinked;
        }

        public bool IsRightLinked()
        {
            return slotLinkState.IsRightLinked;
        }
    }
}