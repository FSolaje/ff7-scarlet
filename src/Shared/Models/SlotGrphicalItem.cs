
using Shojy.FF7.Elena.Equipment;
using Shojy.FF7.Elena.Materias;

namespace FF7Scarlet.Shared.Models
{
	public class SlotGraphicalItem
	{
		public int slotIndex;
		private MateriaSlot slotLinkType;
		public MateriaType materiaType;
		public GrowthRate growthRate;
		private TypeSelectedForSlot lastSelectionType;
		private readonly SlotLinkState slotState;
		private readonly MateriaSlot[] slotsArray;

		public SlotGraphicalItem(int slotIndex, MateriaSlot[] slotsArray, GrowthRate growthRate)
		{
			this.slotIndex = slotIndex;
			this.slotsArray = slotsArray;
			slotLinkType = slotsArray[slotIndex];
			this.growthRate = growthRate;

			SlotGraphicalItem leftSlot = GetLeftSlot();
			SlotGraphicalItem rightSlot = GetRightSlot();

			slotState = new SlotLinkState(this, leftSlot, rightSlot);
			lastSelectionType = new(-1, SlotMenuValue.NoSlot, MateriaSlot.None);

		}
		public MateriaSlot MateriaSlot
		{
			get { return slotLinkType; }
			set
			{
				slotLinkType = value;
				slotsArray[slotIndex] = value;
			}
		}

		private class SlotLinkState
		{
			private readonly SlotGraphicalItem container;
			public bool IsUnlinked { get; set; }
			public bool IsRightLinked { get; set; }
			public bool IsLeftLinked { get; set; }
			public bool IsDoubleLinked { get; set; }
			public bool IsNonSlot { get; set; }

			public SlotLinkState(SlotGraphicalItem container, SlotGraphicalItem leftSlot, SlotGraphicalItem rightSlot)
			{
				this.container = container;
				SetState(leftSlot, rightSlot);
			}


			public void SetState(SlotGraphicalItem leftSlot, SlotGraphicalItem rightSlot)
			{
				bool isUnlinkedFromLeft = container.IsUnlinkedFromLeft(leftSlot);
				bool isUnlinkedFromRight = container.IsUnlinkedFromRight(rightSlot);

				DeterminateSlotState(isUnlinkedFromLeft, isUnlinkedFromRight);

			}

			private void DeterminateSlotState(bool isUnlinkedFromLeft, bool isUnlinkedFromRight)
			{
				IsUnlinked = isUnlinkedFromLeft && isUnlinkedFromRight;
				IsRightLinked = !isUnlinkedFromLeft && isUnlinkedFromRight;
				IsLeftLinked = isUnlinkedFromLeft && !isUnlinkedFromRight;
				IsDoubleLinked = !isUnlinkedFromLeft && !isUnlinkedFromRight;
				IsNonSlot = isUnlinkedFromLeft && container.slotLinkType == MateriaSlot.None;
			}
		}

		private SlotGraphicalItem GetLeftSlot()
		{
			SlotGraphicalItem leftSlot = slotIndex > 0 ? new(slotIndex - 1, slotsArray, growthRate) : this;
			return leftSlot;
		}

		private SlotGraphicalItem GetRightSlot()
		{
			SlotGraphicalItem rightSlot = slotIndex < slotsArray.Length ? new(slotIndex + 1, slotsArray, growthRate) : this;
			return rightSlot;
		}

		private Image GetMatchingImage(Materia? equipped)
		{
			bool isDoubleLinkedSlot = IsDoubleLinked();

			if (equipped != null) //materia is equipped
			{
				var fixedSlot = GetMatchingSlot();
				var materiaType = Materia.GetMateriaType(equipped.MateriaTypeByte);

				return SlotImage.GetSlotImageForEquipedMateria(materiaType, fixedSlot, isDoubleLinkedSlot);
			}
			else //no materia equipped
			{
				return SlotImage.GetSlotImageForEmpty(slotLinkType, isDoubleLinkedSlot);
			}

		}

		private MateriaSlot GetMatchingSlot(MateriaSlot? newMateriaSlot = null)
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
			if (slotIndex >= 0 && slotIndex < slotsArray.Length)
			{
				if (!forceUpdate)
					if (lastSelectionType.SlotIndex == slotIndex)
						forceUpdate = HasChangeSlotToDoubleLinked()
							|| HasChangedSlotFomDoubleLinkedToRightLinked()
						;

				var newMateriaSlotValue = GetMatchingSlot(newMateriaSlot);
				if (MateriaSlot != newMateriaSlotValue || forceUpdate)
				{
					var currentValue = newMateriaSlot;

					//update slot value
					MateriaSlot = newMateriaSlot;

					//attempt to update neighboring slot(s) as well
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
			if (slotIndex - 1 >= 0 && slotIndex + 1 < slotsArray.Length)
			{
				SlotGraphicalItem nextSlotIndex = updateDirection == UpdateDirection.Right ? GetRightSlot() : GetLeftSlot();
				nextSlotIndex.UpdateSlot(oldRightSlotValue, updateDirection);

			}
		}

		private void UpdateSlot(MateriaSlot oldSlotValue, UpdateDirection updateDirection)
		{
			if (slotIndex >= 0 && slotIndex < slotsArray.Length)
			{
				// Define update direction.
				bool ignoreLeft = UpdateDirection.Right == updateDirection;
				bool ignoreRight = UpdateDirection.Left == updateDirection;

				SlotGraphicalItem rightSlot = GetRightSlot();
				SlotGraphicalItem leftSlot = GetLeftSlot();

				bool wasDoubleLinked = WasSlotDoubleLinked(oldSlotValue, updateDirection);

				if (updateDirection == UpdateDirection.Left)
				{
					UpdateSlotToLeft(slotState, wasDoubleLinked);
				}
				else
				{
					UpdateSlotToRight(slotState);
				}
			}
		}

		/// <summary>
		/// Checks if a slot was double-linked before its neighbor was updated.
		/// </summary>
		/// <param name="slotIndex">The index of the slot to check.</param>
		/// <param name="oldNeighborValue">The original value of the neighboring slot.</param>
		/// <param name="updateDirection">The direction of the neighbor that was changed.</param>
		/// <returns>True if the slot was double-linked, otherwise false.</returns>
		private bool WasSlotDoubleLinked(MateriaSlot oldNeighborValue, UpdateDirection updateDirection)
		{
			bool wasSlotDoubleLinked = false;
			SlotGraphicalItem slotToCheck = updateDirection == UpdateDirection.Right ? GetLeftSlot() : GetRightSlot();

			var backup = slotToCheck.MateriaSlot;
			slotToCheck.MateriaSlot = oldNeighborValue;
			wasSlotDoubleLinked = slotToCheck.IsDoubleLinked();
			slotToCheck.MateriaSlot = backup;

			return wasSlotDoubleLinked;

		}

		private void UpdateSlotToLeft(SlotLinkState linkState, bool wasDoubleLinked)
		{

			if (linkState.IsRightLinked)
				// ForceUpdate because doublelinked and rightlinked has the same value.
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

		private bool IsUnlinkedFromRight(SlotGraphicalItem rightSlot)
		{
			bool result = slotIndex == rightSlot.slotIndex;
			result |= rightSlot.slotLinkType == MateriaSlot.None;
			result |= rightSlot.IsLeftLinked();
			result |= rightSlot.IsUnlinked();

			return result;
		}

		private bool IsUnlinkedFromLeft(SlotGraphicalItem leftSlot, UpdateDirection updateDirection = UpdateDirection.None)
		{
			int leftSlotIndex = slotIndex - 1;


			bool result = slotIndex == leftSlotIndex;
			result |= leftSlot.slotLinkType == MateriaSlot.None;
			result |= !leftSlot.IsLeftLinked() && !leftSlot.IsDoubleLinked();
			result |= leftSlot.IsUnlinked();
			// result |= leftSlotHasChangeToRightLinked;
			if (updateDirection != UpdateDirection.None)
				result |= updateDirection == UpdateDirection.Right
									&& leftSlotIndex == lastSelectionType.SlotIndex
									&& lastSelectionType.Item == SlotMenuValue.RightLinked;

			return result;
		}

		/// <summary>
		/// Determines whether the slot at the specified index is "double linked".
		/// A slot is considered double linked if it's a right-linked slot and it is not the first or last slot,
		/// and if the previous slot is left- or right-linked and the next slot is right-linked.
		/// This method helps to identify slots that are linked on both sides, which may affect
		/// how materia can be connected or interact in the UI.
		/// </summary>
		/// <param name="slotIndex">The index of the slot to check.</param>
		/// <returns>
		/// <c>true</c> if the slot at the specified index is double linked; otherwise, <c>false</c>.
		/// </returns>
		private bool IsDoubleLinked(MateriaSlot? newMateriaSlot = null)
		{
			bool isDoubleLinked = false;

			if (slotIndex > 0 && slotIndex < slotsArray.Length - 1)
			{
				// var prev = slotsArray[slotIndex - 1];
				// var curr = newMateriaSlot ?? slotsArray[slotIndex];
				// var next = slotsArray[slotIndex + 1];
				var leftSlot = GetLeftSlot();
				var rightSlot = GetRightSlot();
				isDoubleLinked = (leftSlot.IsLeftLinked() || leftSlot.IsRightLinked()) && IsRightLinked() && rightSlot.IsRightLinked();
				// isDoubleLinked = (SlotIsLeftLinked(prev) || SlotIsRightLinked(prev)) && SlotIsRightLinked(curr) && SlotIsRightLinked(next);
			}

			return isDoubleLinked;
		}
		private bool IsUnlinked()
		{
			return slotLinkType == MateriaSlot.NormalUnlinkedSlot || slotLinkType == MateriaSlot.EmptyUnlinkedSlot;

		}

		private bool IsLeftLinked()
		{
			return slotLinkType == MateriaSlot.NormalLeftLinkedSlot || slotLinkType == MateriaSlot.EmptyLeftLinkedSlot;
		}

		private bool IsRightLinked()
		{
			return slotLinkType == MateriaSlot.NormalRightLinkedSlot || slotLinkType == MateriaSlot.EmptyRightLinkedSlot;
		}

	}
}