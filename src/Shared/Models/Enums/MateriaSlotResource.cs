namespace FF7Scarlet.Shared.Models.Enums
{
    /// <summary>
    /// Represents the graphic resources for materia slots.
    /// <para>
    /// <b>NAMING CONVENTION:</b><br/>
    /// All values MUST follow the pattern: <c>materia_slot_[type]_[suffix]</c> or <c>materia_slot[suffix]</c>.<br/>
    /// </para>
    /// <para>
    /// <b>Suffixes:</b><br/>
    /// 0: None<br/>
    /// 1: Normal Unlinked<br/>
    /// 2: Normal LeftLinked<br/>
    /// 3: Normal RightLinked<br/>
    /// 4: Empty Unlinked<br/>
    /// 5: Empty LeftLinked<br/>
    /// 6: Empty RightLinked<br/>
    /// _dl: Double Linked (with materia)<br/>
    /// _dl1: Normal Double Linked (empty)<br/>
    /// _dl2: Empty Double Linked (empty)
    /// </para>
    /// </summary>
    public enum MateriaSlotResource
    {
        // Base slots
        materia_slot0,
        materia_slot1,
        materia_slot2,
        materia_slot3,
        materia_slot4,
        materia_slot5,
        materia_slot6,
        materia_slot_dl1,
        materia_slot_dl2,

        // Command (Yellow)
        materia_slot_command1,
        materia_slot_command2,
        materia_slot_command3,
        materia_slot_command_dl,

        // Magic (Green)
        materia_slot_magic1,
        materia_slot_magic2,
        materia_slot_magic3,
        materia_slot_magic_dl,

        // Summon (Red)
        materia_slot_summon1,
        materia_slot_summon2,
        materia_slot_summon3,
        materia_slot_summon_dl,

        // Support (Blue)
        materia_slot_support1,
        materia_slot_support2,
        materia_slot_support3,
        materia_slot_support_dl,

        // Independent (Purple)
        materia_slot_independent1,
        materia_slot_independent2,
        materia_slot_independent3,
        materia_slot_independent_dl
    }
}