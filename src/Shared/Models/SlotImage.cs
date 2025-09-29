using System.Collections;
using Shojy.FF7.Elena.Equipment;
using Shojy.FF7.Elena.Materias;

namespace FF7Scarlet.Shared.Models
{
	public class SlotImage
	{
		private readonly static Image defaultImage = Properties.Resources.materia_slot0;

		public static Image GetSlotImageForEquipedMateria(MateriaType materiaType, MateriaSlot materiaSlot, bool isDoubleLinkedSlot = false)
		{
			Dictionary<MateriaType, Func<MateriaSlot, bool, Image>> imageDictionary = new()
			{
				{ MateriaType.Command, GetCommandMateriaImageFor },
				{ MateriaType.Magic, GetMagicMateriaImageFor },
				{ MateriaType.Summon, GetMagicSummonImageFor },
				{ MateriaType.Support, GetMagicSupportImageFor },
				{ MateriaType.Independent, GetIndepentdentMateriaImageFor }
			};

			return imageDictionary[materiaType].Invoke(materiaSlot, isDoubleLinkedSlot);
		}

		public static Image GetSlotImageForEmpty(MateriaSlot materiaSlot, bool isDoubleLinked)
		{
			Image emptyRightLinkedImage = isDoubleLinked ?
				Properties.Resources.materia_slot_dl2
				: Properties.Resources.materia_slot6;

			Image normalRightLinkedImage = isDoubleLinked ?
				Properties.Resources.materia_slot_dl1
				: Properties.Resources.materia_slot3;

			Dictionary<MateriaSlot, Image> imageDictionary = new()
			{
				{ MateriaSlot.None, Properties.Resources.materia_slot0 },
				{ MateriaSlot.NormalUnlinkedSlot, Properties.Resources.materia_slot1 },
				{ MateriaSlot.NormalLeftLinkedSlot, Properties.Resources.materia_slot2 },
				{ MateriaSlot.NormalRightLinkedSlot, normalRightLinkedImage },
				{ MateriaSlot.EmptyUnlinkedSlot, Properties.Resources.materia_slot4 },
				{ MateriaSlot.EmptyLeftLinkedSlot, Properties.Resources.materia_slot5 },
				{ MateriaSlot.EmptyRightLinkedSlot, emptyRightLinkedImage }
			};

			return imageDictionary[materiaSlot];
		}

		private static Image GetCommandMateriaImageFor(MateriaSlot materiaSlot, bool isDoubleLinked)
		{
			Image image = defaultImage;

			Image rightLinkedImage = isDoubleLinked ?
				Properties.Resources.materia_slot_command_dl
				: Properties.Resources.materia_slot_command3;

			Dictionary<MateriaSlot, Image> materiaImages = new()
			{
				{ MateriaSlot.NormalUnlinkedSlot, Properties.Resources.materia_slot_command1 },
				{ MateriaSlot.NormalLeftLinkedSlot, Properties.Resources.materia_slot_command2 },
				{ MateriaSlot.NormalRightLinkedSlot, rightLinkedImage },
			};

			materiaImages[MateriaSlot.NormalRightLinkedSlot] = Properties.Resources.materia_slot_command_dl;


			image = materiaImages[materiaSlot];

			return image;
		}

		private static Image GetMagicMateriaImageFor(MateriaSlot materiaSlot, bool isDoubleLinked)
		{
			Image image = defaultImage;
			Image rightLinkedImage = isDoubleLinked ?
				Properties.Resources.materia_slot_magic_dl
				: Properties.Resources.materia_slot_magic3;
			Dictionary<MateriaSlot, Image> materiaImages = new()
			{
				{ MateriaSlot.NormalUnlinkedSlot, Properties.Resources.materia_slot_magic1 },
				{ MateriaSlot.NormalLeftLinkedSlot, Properties.Resources.materia_slot_magic2 },
				{ MateriaSlot.NormalRightLinkedSlot, rightLinkedImage },
			};

			materiaImages[MateriaSlot.NormalRightLinkedSlot] = Properties.Resources.materia_slot_magic_dl;

			image = materiaImages[materiaSlot];

			return image;
		}

		private static Image GetMagicSummonImageFor(MateriaSlot materiaSlot, bool isDoubleLinked)
		{
			Image image = defaultImage;
			Image rightLinkedImage = isDoubleLinked ?
				Properties.Resources.materia_slot_summon_dl
				: Properties.Resources.materia_slot_summon3;
			Dictionary<MateriaSlot, Image> materiaImages = new()
			{
				{ MateriaSlot.NormalUnlinkedSlot, Properties.Resources.materia_slot_summon1 },
				{ MateriaSlot.NormalLeftLinkedSlot, Properties.Resources.materia_slot_summon2 },
				{ MateriaSlot.NormalRightLinkedSlot, rightLinkedImage },
			};
			materiaImages[MateriaSlot.NormalRightLinkedSlot] = Properties.Resources.materia_slot_summon_dl;

			image = materiaImages[materiaSlot];

			return image;
		}

		private static Image GetMagicSupportImageFor(MateriaSlot materiaSlot, bool isDoubleLinked)
		{
			Image image = defaultImage;
			Image rightLinkedImage = isDoubleLinked ?
				Properties.Resources.materia_slot_support_dl
				: Properties.Resources.materia_slot_support3;

			Dictionary<MateriaSlot, Image> materiaImages = new()
			{
				{ MateriaSlot.NormalUnlinkedSlot, Properties.Resources.materia_slot_support1 },
				{ MateriaSlot.NormalLeftLinkedSlot, Properties.Resources.materia_slot_support2 },
				{ MateriaSlot.NormalRightLinkedSlot, rightLinkedImage},
			};
			materiaImages[MateriaSlot.NormalRightLinkedSlot] = Properties.Resources.materia_slot_support_dl;

			image = materiaImages[materiaSlot];

			return image;
		}

		private static Image GetIndepentdentMateriaImageFor(MateriaSlot materiaSlot, bool isDoubleLinked)
		{
			Image image = defaultImage;
			Image rightLinkedImage = isDoubleLinked ?
				Properties.Resources.materia_slot_independent_dl
				: Properties.Resources.materia_slot_independent3;

			Dictionary<MateriaSlot, Image> materiaImages = new()
			{
				{ MateriaSlot.NormalUnlinkedSlot, Properties.Resources.materia_slot_independent1 },
				{ MateriaSlot.NormalLeftLinkedSlot, Properties.Resources.materia_slot_independent2 },
				{ MateriaSlot.NormalRightLinkedSlot, rightLinkedImage},
			};

			image = materiaImages[materiaSlot];

			return image;
		}

	}

}