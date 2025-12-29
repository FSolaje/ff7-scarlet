using System;
using System.ComponentModel;
using System.Reflection;

namespace FF7Scarlet.Shared.Models
{
    /// <summary>
    /// Provides extension methods for enum types.
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Retrieves the description of an enum value from its <see cref="System.ComponentModel.DescriptionAttribute"/>.
        /// </summary>
        /// <param name="value">The enum value.</param>
        /// <returns>The description string from the attribute, or the enum's name if the attribute is not found.</returns>
        public static string GetDescription(this Enum value)
        {
            FieldInfo? field = value.GetType().GetField(value.ToString());
            return field?.GetCustomAttribute<DescriptionAttribute>()?.Description ?? value.ToString();
        }
    }

    public enum SlotMenuValue
    {
        [Description("No slot")]
        NoSlot,

        [Description("Unlinked slot")]
        Unlinked,

        [Description("Left linked slot")]
        LeftLinked,

        [Description("Right linked slot")]
        RightLinked,

        [Description("Double linked slot")]
        DoubleLinked
    }

    public enum UpdateDirection { None, Left, Right, Both }
}