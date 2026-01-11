# Algoritmo de Imagen de Slots (`SlotImage`)

Este documento define las reglas para seleccionar el recurso gráfico (`MateriaSlotResource`) que representa un slot en la interfaz, basándose en su estado lógico y contenido.

## 1. Entradas del Algoritmo
*   **Estado del Slot**: `MateriaSlot` (NS, UL, LL, RL).
*   **Contexto DL**: `IsDoubleLinked` (Booleano).
*   **Crecimiento**: `GrowthRate` (None = Empty, Otro = Normal).
*   **Materia**: Tipo de materia equipada (`None`, `Command`, `Magic`, etc.).

---

## 2. Convención de Nombres (Mandatoria)

Para que la resolución dinámica funcione, todos los recursos en el archivo `.resx` y en el enum `MateriaSlotResource` deben seguir estrictamente este patrón:

`materia_slot_[tipo]_[sufijo]`

*   **Prefijo**: `materia_slot`
*   **Tipo (Opcional)**: `_command`, `_magic`, etc. (Solo si hay materia equipada).
*   **Sufijo**: Identificador numérico o especial del estado.

---

## 3. Reglas de Selección: Slot Vacío (Sin Materia)

Cuando el slot no tiene materia (`Materia.None`), la imagen depende del estado físico y del crecimiento implícito en el valor del enum `MateriaSlot`.

### 3.1 Crecimiento Normal (Normal Growth)
*   **[SIMAGE-EMPTY-NS]**: Estado `None` -> Sufijo `0`.
*   **[SIMAGE-EMPTY-UL-NORM]**: Estado `NormalUnlinked` -> Sufijo `1`.
*   **[SIMAGE-EMPTY-LL-NORM]**: Estado `NormalLeftLinked` -> Sufijo `2`.
*   **[SIMAGE-EMPTY-RL-NORM]**: Estado `NormalRightLinked` (NO DL) -> Sufijo `3`.
*   **[SIMAGE-EMPTY-DL-NORM]**: Estado `NormalRightLinked` (SI DL) -> Sufijo `_dl1`.

### 3.2 Crecimiento Nulo (No Growth / Empty)
*   **[SIMAGE-EMPTY-UL-NO]**: Estado `EmptyUnlinked` -> Sufijo `4`.
*   **[SIMAGE-EMPTY-LL-NO]**: Estado `EmptyLeftLinked` -> Sufijo `5`.
*   **[SIMAGE-EMPTY-RL-NO]**: Estado `EmptyRightLinked` (NO DL) -> Sufijo `6`.
*   **[SIMAGE-EMPTY-DL-NO]**: Estado `EmptyRightLinked` (SI DL) -> Sufijo `_dl2`.

---

## 4. Reglas de Selección: Slot Ocupado (Con Materia)

Cuando hay una materia equipada, el factor de crecimiento (Normal vs Empty) se ignora visualmente. Se utilizan prefijos según el tipo de materia y sufijos unificados.

### Patrón de Sufijos Unificados
*   **[SIMAGE-MAT-UL]**: Cualquier estado `Unlinked` (Normal o Empty) -> Sufijo `1`.
*   **[SIMAGE-MAT-LL]**: Cualquier estado `LeftLinked` (Normal o Empty) -> Sufijo `2`.
*   **[SIMAGE-MAT-RL]**: Cualquier estado `RightLinked` (Normal o Empty, NO DL) -> Sufijo `3`.
*   **[SIMAGE-MAT-DL]**: Cualquier estado `RightLinked` (Normal o Empty, SI DL) -> Sufijo `_dl`.

### Excepciones
*   Si el tipo de materia no tiene gráficos específicos (ej: `None`), se aplica la lógica de Slot Vacío (Sección 3).