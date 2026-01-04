
using Shojy.FF7.Elena.Equipment;
using FF7Scarlet.Shared.Models.Enums;

namespace FF7Scarlet.Shared.Models
{
	public class TypeSelectedForSlot(int slotIndex, SlotMenuValue item, MateriaSlot prevSlotValue)
	{
		public int SlotIndex { get; set; } = slotIndex;
		public SlotMenuValue Item { get; set; } = item;
		public MateriaSlot PrevSlotValue { get; set; } = prevSlotValue;
	}

}
