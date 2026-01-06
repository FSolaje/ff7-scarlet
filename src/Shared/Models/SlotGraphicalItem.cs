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
                if (SlotIndex > 0 && SlotIndex < SlotsArray.Length)
                    SlotsArray[SlotIndex] = value;
            }
        }

        internal SlotGraphicalItem LeftSlot
        {
            get
            {
                SlotGraphicalItem leftSlot;
                if (SlotIndex > 0)
                    leftSlot = AllSlots[SlotIndex - 1];
                else
                    leftSlot = new SlotGraphicalItem(SlotsArray);

                return leftSlot;
            }
        }

        internal SlotGraphicalItem RightSlot
        {
            get
            {
                SlotGraphicalItem rightSlot;
                if (SlotIndex < AllSlots.Count - 1)
                    rightSlot = AllSlots[SlotIndex + 1];
                else
                    rightSlot = new SlotGraphicalItem(SlotsArray);

                return rightSlot;

            }

        }

        public MateriaSlotResource GetMatchingResource(Materia? equipped = null)
        {
            bool isDoubleLinkedSlot = IsDoubleLinked();

            if (equipped != null)
            {
                var fixedSlot = GetMatchingSlot();
                var materiaType = Materia.GetMateriaType(equipped.MateriaTypeByte);
                var materiaTypeExtended = materiaType.ToExtendedMateriaType();

                if (materiaTypeExtended == MateriaTypeExtended.None)
                {
                    return SlotImage.GetSlotResourceForEmpty(MateriaSlotValue, isDoubleLinkedSlot);
                }

                return SlotImage.GetSlotResourceForEquippedMateria(materiaTypeExtended, fixedSlot, isDoubleLinkedSlot);
            }
            else
            {
                return SlotImage.GetSlotResourceForEmpty(MateriaSlotValue, isDoubleLinkedSlot);
            }
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
            lastSelectionType = slotClickedInSelectorType;
            if (SlotIndex >= 0 && SlotIndex < SlotsArray.Length)
            {
                if(IsADoubleLinkedClickedSlot())
                    forceUpdate = true;

                var materiaSlotValueFixed = GetMatchingSlot(newMateriaSlot);

                if (MateriaSlotValue != newMateriaSlot || forceUpdate)
                {
                    var currentValue = MateriaSlotValue;
                    MateriaSlotValue = materiaSlotValueFixed;

                    if (updateDirection == UpdateDirection.Left || updateDirection == UpdateDirection.Both)
                        UpdateLeftSlot();

                    if (updateDirection == UpdateDirection.Right || updateDirection == UpdateDirection.Both)
                        UpdateRightSlot();

                    return true;
                }
            }
            return false;
        }

        private void UpdateLeftSlot()
        {

            if (IsLeftLinked())
            {
                if (LeftSlot.IsLeftLinked())
                    LeftSlot.SetInSlot(MateriaSlot.EmptyUnlinkedSlot, lastSelectionType, UpdateDirection.Left);
            }

            else if (IsRightLinked())
            {
                if (LeftSlot.IsUnlinked() || LeftSlot.IsNonSlot())
                    LeftSlot.SetInSlot(MateriaSlot.EmptyLeftLinkedSlot, lastSelectionType, UpdateDirection.Left);

            }

            else if (IsUnlinked() || IsNonSlot())
            {
                if (LeftSlot.IsLeftLinked())
                    LeftSlot.SetInSlot(MateriaSlot.EmptyUnlinkedSlot, lastSelectionType, UpdateDirection.Left);

            }

        }

        private void UpdateRightSlot()
        {
            if (IsLeftLinked() && IsTheClickedSlot())
            {
                if (!RightSlot.IsRightLinked())
                    RightSlot.SetInSlot(MateriaSlot.EmptyRightLinkedSlot, lastSelectionType, UpdateDirection.Right);

            }

            else if (IsRightLinked() && IsADoubleLinkedClickedSlot())
            {
                if (!RightSlot.IsRightLinked())
                    RightSlot.SetInSlot(MateriaSlot.EmptyRightLinkedSlot, lastSelectionType, UpdateDirection.Right);
            }

            else if (IsRightLinked() && IsTheClickedSlot()) 
            {
                    if (RightSlot.IsRightLinked() && RightSlot.RightSlot.IsRightLinked())
                        RightSlot.SetInSlot(MateriaSlot.EmptyLeftLinkedSlot, lastSelectionType, UpdateDirection.Right);
            }

            else if (IsUnlinked() || IsNonSlot())
            {
                if (RightSlot.IsRightLinked() && RightSlot.RightSlot.IsRightLinked())
                    RightSlot.SetInSlot(MateriaSlot.EmptyLeftLinkedSlot, lastSelectionType, UpdateDirection.Right);
                else
                    RightSlot.SetInSlot(MateriaSlot.EmptyUnlinkedSlot, lastSelectionType, UpdateDirection.Right);
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
            bool isDoubleLinked = false;
            if (isMultilinkedEnabled)
            {
                bool isDoubleLinkedClicked = IsADoubleLinkedClickedSlot();

                bool isRightLinkedChain = IsRightLinked() && RightSlot.IsRightLinked();
                bool isLeftConnected = LeftSlot.IsLeftLinked() || LeftSlot.IsRightLinked();
                bool isMiddleOfChain = isRightLinkedChain && isLeftConnected;

                isDoubleLinked = isDoubleLinkedClicked || isMiddleOfChain;
            }

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