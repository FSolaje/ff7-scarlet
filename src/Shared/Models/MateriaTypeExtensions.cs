using Shojy.FF7.Elena.Materias;
using FF7Scarlet.Shared.Models.Enums;

namespace FF7Scarlet.Shared.Models
{
    public static class MateriaTypeExtensions
    {
        public static MateriaTypeExtended ToExtendedMateriaType(this MateriaType materiaType)
        {
            if ((int)materiaType == 0)
            {
                return MateriaTypeExtended.None;
            }
            return (MateriaTypeExtended)(int)materiaType;
        }
    }
}