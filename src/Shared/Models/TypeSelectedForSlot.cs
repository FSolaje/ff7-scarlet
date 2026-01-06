
using Shojy.FF7.Elena.Equipment;
using FF7Scarlet.Shared.Models.Enums;

namespace FF7Scarlet.Shared.Models
{
	public class TypeSelectedForSlot
	{
		public int SlotIndex { get; set; }
		public SlotMenuValue Item { get; set; }
		public MateriaSlot PrevSlotValue { get; set; }

	 	public TypeSelectedForSlot()
		{
			SlotIndex = -1;
			Item  = SlotMenuValue.NoSlot;
			PrevSlotValue = MateriaSlot.None;

		}
	 
	 	public TypeSelectedForSlot(int slotIndex, SlotMenuValue item, MateriaSlot prevSlotValue)
		{
			SlotIndex = slotIndex;
			Item  = item;
			PrevSlotValue = prevSlotValue;
		}
	}

}
