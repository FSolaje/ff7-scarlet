using Shojy.FF7.Elena.Materias;

namespace FF7Scarlet.Shared.Models
{
    public static class MateriaTypeExtensions
    {
        public static ExtendedMateriaType ToExtendedMateriaType(this MateriaType materiaType)
        {
            if ((int)materiaType == 0)
            {
                return ExtendedMateriaType.None;
            }
            return (ExtendedMateriaType)(int)materiaType;
        }
    }
}