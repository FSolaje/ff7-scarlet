# Análisis de Caja Blanca: Lógica de Crecimiento (`SlotLogic_Growth`)

**Objetivo:** Verificar la correcta transición de variantes de slots al cambiar el `GrowthRate` global.
**SUT:** `SlotGraphicalItem.cs` (Método `UpdateGrowthRate` o similar) y `MateriaSlotSelectorControl.cs`.

---

## 1. Análisis de Flujo de Control

La lógica es una iteración sobre todos los slots existentes tras un cambio de configuración global.

1.  **Entrada:** Nuevo `GrowthRate`.
2.  **Actualización Estática:** Se actualiza `SlotGraphicalItem.GrowthRate`.
3.  **Iteración:** Se recorre cada instancia de `SlotGraphicalItem` en la lista activa.
4.  **Recálculo:** Cada slot invoca `GetMatchingSlot()` (que ya implementamos previamente y depende de `GrowthRate`).
5.  **Actualización Local:** Si el valor recalculado es diferente, se actualiza `MateriaSlotValue`.

## 2. Tabla de Casos de Prueba (Trazabilidad)

| ID | Regla Validada | Estado Inicial (Growth, Slot) | Acción | Resultado Esperado |
|:---|:---|:---|:---|:---|
| **TG-01** | [SLOT-GROWTH-01] | Normal, `NormalUnlinked` | Set Growth -> None | `EmptyUnlinked` |
| **TG-02** | [SLOT-GROWTH-01] | None, `EmptyLeftLinked` | Set Growth -> Normal | `NormalLeftLinked` |
| **TG-03** | [SLOT-GROWTH-01] | None, `EmptyRightLinked` | Set Growth -> Double | `NormalRightLinked` |
| **TG-04** | [SLOT-GROWTH-01] | Normal, `None` (NS) | Set Growth -> None | `None` (No cambia) |
