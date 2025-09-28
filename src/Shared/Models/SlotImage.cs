using System.Collections;
using Shojy.FF7.Elena.Equipment;
using Shojy.FF7.Elena.Materias;

namespace FF7Scarlet.Shared.Models
{
	public class SlotImage
	{
		private readonly static Image defaultImage = Properties.Resources.materia_slot0;

		public static Image GetSlotImageForEquipedMateria(MateriaType materiaType, MateriaSlot materiaSlot)
		{
			Dictionary<MateriaType, Func<MateriaSlot, Image>> imageDictionary = new()
			{
				{ MateriaType.Command, GetCommandMateriaImageFor },
				{ MateriaType.Magic, GetMagicMateriaImageFor },
				{ MateriaType.Summon, GetMagicSummonImageFor },
				{ MateriaType.Support, GetMagicSupportImageFor },
				{ MateriaType.Independent, GetIndepentdentMateriaImageFor }
			};

			return imageDictionary[materiaType].Invoke(materiaSlot);
		}

		private static Image GetSlotImageForEmpty(MateriaSlot materiaSlot)
		{
			Dictionary<MateriaSlot, Image> imageDictionary = new()
			{
				{ MateriaSlot.None, Properties.Resources.materia_slot0 },
				{ MateriaSlot.NormalUnlinkedSlot, Properties.Resources.materia_slot1 },
				{ MateriaSlot.NormalLeftLinkedSlot, Properties.Resources.materia_slot2 },
				{ MateriaSlot.NormalRightLinkedSlot, Properties.Resources.materia_slot3 },
				{ MateriaSlot.EmptyUnlinkedSlot, Properties.Resources.materia_slot4 },
				{ MateriaSlot.EmptyLeftLinkedSlot, Properties.Resources.materia_slot5 },
				{ MateriaSlot.EmptyRightLinkedSlot, Properties.Resources.materia_slot6 }
			};

			return imageDictionary[materiaSlot];
		}

		private static Image GetCommandMateriaImageFor(MateriaSlot materiaSlot)
		{
			Image image = defaultImage;
			Dictionary<MateriaSlot,Image>  materiaImages = new()
			{
				{ MateriaSlot.NormalUnlinkedSlot, Properties.Resources.materia_slot_command1 },
				{ MateriaSlot.NormalLeftLinkedSlot, Properties.Resources.materia_slot_command2 },
				{ MateriaSlot.NormalRightLinkedSlot, Properties.Resources.materia_slot_command3 },
			};

			image = materiaImages[materiaSlot];

			return image;
		}

		private static Image GetMagicMateriaImageFor(MateriaSlot materiaSlot)
		{
			Image image = defaultImage;
			Dictionary<MateriaSlot,Image>  materiaImages = new()
			{
				{ MateriaSlot.NormalUnlinkedSlot, Properties.Resources.materia_slot_magic1 },
				{ MateriaSlot.NormalLeftLinkedSlot, Properties.Resources.materia_slot_magic2 },
				{ MateriaSlot.NormalRightLinkedSlot, Properties.Resources.materia_slot_magic3 },
			};

			image = materiaImages[materiaSlot];

			return image;
		}

		private static Image GetMagicSummonImageFor(MateriaSlot materiaSlot)
		{
			Image image = defaultImage;
			Dictionary<MateriaSlot,Image>  materiaImages = new()
			{
				{ MateriaSlot.NormalUnlinkedSlot, Properties.Resources.materia_slot_summon1 },
				{ MateriaSlot.NormalLeftLinkedSlot, Properties.Resources.materia_slot_summon2 },
				{ MateriaSlot.NormalRightLinkedSlot, Properties.Resources.materia_slot_summon3 },
			};

			image = materiaImages[materiaSlot];

			return image;
		}

		private static Image GetMagicSupportImageFor(MateriaSlot materiaSlot)
		{
			Image image = defaultImage;
			Dictionary<MateriaSlot,Image>  materiaImages = new()
			{
				{ MateriaSlot.NormalUnlinkedSlot, Properties.Resources.materia_slot_support1 },
				{ MateriaSlot.NormalLeftLinkedSlot, Properties.Resources.materia_slot_support2 },
				{ MateriaSlot.NormalRightLinkedSlot, Properties.Resources.materia_slot_support3 },
			};

			image = materiaImages[materiaSlot];

			return image;
		}

		private static Image GetIndepentdentMateriaImageFor(MateriaSlot materiaSlot)
		{
			Image image = defaultImage;
			Dictionary<MateriaSlot,Image>  materiaImages = new()
			{
				{ MateriaSlot.NormalUnlinkedSlot, Properties.Resources.materia_slot_independent1 },
				{ MateriaSlot.NormalLeftLinkedSlot, Properties.Resources.materia_slot_independent2 },
				{ MateriaSlot.NormalRightLinkedSlot, Properties.Resources.materia_slot_independent3 },
			};

			image = materiaImages[materiaSlot];

			return image;
		}

	}

}