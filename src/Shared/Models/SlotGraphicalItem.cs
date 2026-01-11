using Shojy.FF7.Elena.Equipment;
using Shojy.FF7.Elena.Materias;
using FF7Scarlet.Shared.Models.Enums;

namespace FF7Scarlet.Shared.Models
{
    public class SlotGraphicalItem
    {
        private static readonly List<SlotGraphicalItem> AllSlots = [];
        private static bool isMultilinkedEnabled = false;
        private static MateriaSlot[] SlotsArray {get; set;} = [];
        private static TypeSelectedForSlot lastSelectionType = new();
        public static GrowthRate GrowthRate {get; set;}

        static SlotGraphicalItem()
        {
            lastSelectionType = new(-1, SlotMenuValue.NoSlot, MateriaSlot.None);
        }

        public int SlotIndex { get; private set; }
        public MateriaTypeExtended materiaType;

        private SlotGraphicalItem(MateriaSlot[] slotsArray)
        {
            SlotIndex = -1;
            MateriaSlotValue = MateriaSlot.None;
            this.materiaType = MateriaTypeExtended.None;
        }

        private SlotGraphicalItem(int slotIndex, MateriaSlot[] slotsArray, GrowthRate growthRate, bool isMultiLinkedEnabled)
        {
            this.SlotIndex = slotIndex;
            SlotGraphicalItem.SlotsArray = slotsArray;
            this.MateriaSlotValue = slotsArray[slotIndex];
            SlotGraphicalItem.GrowthRate = growthRate;
            SlotGraphicalItem.isMultilinkedEnabled = isMultiLinkedEnabled;
            this.materiaType = MateriaTypeExtended.None;
        }

        public static List<SlotGraphicalItem> CreateSlots(MateriaSlot[] slotsArray, GrowthRate growthRate, bool isMultiLinkedEnabled)
        {
            AllSlots.Clear();

            for (int index = 0; index < slotsArray.Length; index++)
            {
                var newSlot = new SlotGraphicalItem(index, slotsArray, growthRate, isMultiLinkedEnabled);
                AllSlots.Add(newSlot);
            }

            return AllSlots;
        }

        public MateriaSlot MateriaSlotValue
        {
            get
            {
                var materiaSlot = SlotIndex < 0 || SlotIndex >= SlotsArray.Length
                    ? MateriaSlot.None
                    : SlotsArray[SlotIndex];
                return materiaSlot;
            }
            set
            {
                if (SlotIndex >= 0 && SlotIndex < SlotsArray.Length)
                    SlotsArray[SlotIndex] = value;
            }
        }

        internal SlotGraphicalItem LeftSlot
        {
            get
            {
                if (SlotIndex > 0)
                    return AllSlots[SlotIndex - 1];
                else
                    return new SlotGraphicalItem(SlotsArray);
            }
        }

        internal SlotGraphicalItem RightSlot
        {
            get
            {
                if (SlotIndex < AllSlots.Count - 1)
                    return AllSlots[SlotIndex + 1];
                else
                    return new SlotGraphicalItem(SlotsArray);
            }
        }

        public MateriaSlotResource GetMatchingResource(Materia? equipped = null)
        {
            bool isDoubleLinkedSlot = IsDoubleLinked();
            MateriaTypeExtended materiaTypeExtended = MateriaTypeExtended.None;
            MateriaSlot slotToUse = MateriaSlotValue;

            if (equipped != null)
            {
                slotToUse = GetMatchingSlot();
                var materiaType = Materia.GetMateriaType(equipped.MateriaTypeByte);
                materiaTypeExtended = materiaType.ToExtendedMateriaType();
            }

            return SlotImage.GetSlotResource(materiaTypeExtended, slotToUse, isDoubleLinkedSlot);
        }

        public Image GetMatchingImage(Materia? equipped = null)
        {
            var resource = GetMatchingResource(equipped);
            return SlotImage.GetImageFromResource(resource.ToString());
        }

        public MateriaSlot GetMatchingSlot(MateriaSlot? newMateriaSlot = null)
        {
            var outputMateriaSlot = newMateriaSlot ?? MateriaSlotValue;

            var slotTypeMatcher = new List<(Func<bool> condition, Func<GrowthRate, MateriaSlot> getMateriaSlot)>
            {
                (() => outputMateriaSlot.IsUnlinked(), growthRate =>
                    growthRate == GrowthRate.None ? MateriaSlot.EmptyUnlinkedSlot : MateriaSlot.NormalUnlinkedSlot),
                (() => outputMateriaSlot.IsLeftLinked(), growthRate =>
                    growthRate == GrowthRate.None ? MateriaSlot.EmptyLeftLinkedSlot : MateriaSlot.NormalLeftLinkedSlot),
                (() => outputMateriaSlot.IsRightLinked(), growthRate =>
                    growthRate == GrowthRate.None ? MateriaSlot.EmptyRightLinkedSlot : MateriaSlot.NormalRightLinkedSlot)
            };

            var (condition, getMateriaSlot) = slotTypeMatcher.FirstOrDefault(m => m.condition());
            var materiaSlot = getMateriaSlot?.Invoke(GrowthRate) ?? outputMateriaSlot;
            return materiaSlot;
        }

        public bool SetInSlot(MateriaSlot newMateriaSlot, TypeSelectedForSlot slotClickedInSelectorType, UpdateDirection updateDirection, bool forceUpdate = false)
        {
            bool result = false;
            lastSelectionType = slotClickedInSelectorType;
            
            if (SlotIndex >= 0 && SlotIndex < SlotsArray.Length)
            {
                if(IsADoubleLinkedClickedSlot())
                    forceUpdate = true;

                var materiaSlotValueFixed = GetMatchingSlot(newMateriaSlot);

                if (MateriaSlotValue != materiaSlotValueFixed || forceUpdate)
                {
                    MateriaSlotValue = materiaSlotValueFixed;

                    if (updateDirection == UpdateDirection.Left || updateDirection == UpdateDirection.Both)
                        UpdateLeftSlot();

                    if (updateDirection == UpdateDirection.Right || updateDirection == UpdateDirection.Both)
                        UpdateRightSlot();

                    result = true;
                }
            }
            return result;
        }

        private void UpdateLeftSlot()
        {
            // [SLOT-NAT-L-01] & [SLOT-ML-L-01]: Break left link
            if (IsLeftLinked() || IsUnlinked() || IsNonSlot())
            {
                if (LeftSlot.IsLeftLinked())
                {
                    LeftSlot.SetInSlot(MateriaSlot.NormalUnlinkedSlot, lastSelectionType, UpdateDirection.Left);
                }
            }
            // [SLOT-NAT-L-02] & [SLOT-ML-L-02]: Create left link (Auto-link)
            else if (IsRightLinked())
            {
                if (LeftSlot.IsUnlinked() || LeftSlot.IsNonSlot())
                {
                    LeftSlot.SetInSlot(MateriaSlot.NormalLeftLinkedSlot, lastSelectionType, UpdateDirection.Left);
                }
            }
        }

        private void UpdateRightSlot()
        {
            bool createRightLink = false;
            bool breakRightLink = false;

            // [SLOT-ML-R-01] & [SLOT-ML-R-04]: Create right link (LL or DL explicit)
            if ((IsLeftLinked() || IsADoubleLinkedClickedSlot()) && IsTheClickedSlot())
            {
                createRightLink = true;
            }
            // [SLOT-ML-R-02] & [SLOT-ML-R-03]: Break right link (UL, NS, or RL explicit)
            else if (IsUnlinked() || IsNonSlot() || (IsRightLinked() && IsTheClickedSlot()))
            {
                breakRightLink = true;
            }

            // Apply Logic
            if (createRightLink)
            {
                // [SLOT-NAT-R-02]: Assign RL to RS(1)
                if (!RightSlot.IsRightLinked())
                {
                    RightSlot.SetInSlot(MateriaSlot.NormalRightLinkedSlot, lastSelectionType, UpdateDirection.Right);
                }
            }
            else if (breakRightLink)
            {
                // [SLOT-NAT-R-01]: Check if RS(1) is linked
                if (RightSlot.IsRightLinked())
                {
                    // [SLOT-ML-R-02] Condition 2: Save chain if RS(2) is RL
                    if (RightSlot.RightSlot.IsRightLinked())
                    {
                        RightSlot.SetInSlot(MateriaSlot.NormalLeftLinkedSlot, lastSelectionType, UpdateDirection.Right);
                    }
                    else
                    {
                        RightSlot.SetInSlot(MateriaSlot.NormalUnlinkedSlot, lastSelectionType, UpdateDirection.Right);
                    }
                }
            }
        }

        private bool IsADoubleLinkedClickedSlot()
        {
            return IsTheClickedSlot() && lastSelectionType.Item == SlotMenuValue.DoubleLinked;
        }

        private bool IsTheClickedSlot()
        {
            return SlotIndex == lastSelectionType.SlotIndex;
        }

        public bool IsDoubleLinked(MateriaSlot? newMateriaSlot = null)
        {
            if (!isMultilinkedEnabled) return false;

            bool isDoubleLinkedClicked = IsADoubleLinkedClickedSlot();

            bool isRightLinkedChain = IsRightLinked() && RightSlot.IsRightLinked();
            bool isLeftConnected = LeftSlot.IsLeftLinked() || LeftSlot.IsRightLinked();
            bool isMiddleOfChain = isRightLinkedChain && isLeftConnected;

            bool isDoubleLinked = isDoubleLinkedClicked || isMiddleOfChain;
            return isDoubleLinked;
        }

        public bool IsUnlinked()
        {
            return MateriaSlotValue.IsUnlinked();
        }

        public bool IsLeftLinked()
        {
            return MateriaSlotValue.IsLeftLinked();
        }

        public bool IsRightLinked()
        {
            return MateriaSlotValue.IsRightLinked();
        }
        public bool IsNonSlot()
        {
            return MateriaSlotValue.IsNone();
        }
    }
}
