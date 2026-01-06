# Análisis de Caja Blanca: `SlotUI_ContextMenu`

**Objetivo:** Verificar que el menú contextual refleja fielmente el estado interno de los slots y respeta las restricciones de posición.
**SUT:** `MateriaSlotSelectorControl.cs` (Método `UpdateSlotSelectorType`).

---

## 1. Análisis de Flujo de Control (`UpdateSlotSelectorType`)

El flujo es iterativo sobre los elementos del menú de un slot específico.

1.  **Entrada:** `slotIndex`.
2.  **Obtención de Estado:** Recupera `SlotGraphicalItem` y calcula variables booleanas (`isDoubleLinked`, `isRightLinked`, etc.).
3.  **Iteración:** Recorre los `ToolStripMenuItem` asociados al slot.
4.  **Decisión:** Asigna `mi.Checked` basándose en el índice del item y el valor booleano correspondiente.

## 2. Complejidad Ciclomática ($V(G)$)
Basado en el método refactorizado:
1.  `if (SlotSelectorType == SlotSelectorType.Slots)`
2.  `mi.Checked = slotCheckValues[itemIndex]` (Dentro de un bucle, pero la lógica de decisión ya fue pre-calculada).
3.  Cálculo de `isDoubleLinked`: `multiLinkEnabled && slot.IsDoubleLinked()` (2 predicados).
4.  Cálculo de `isRightLinked`: `slot.IsRightLinked() && !isDoubleLinked` (2 predicados).

**$V(G) \approx 6$** (Complejidad baja, lógica linealizada gracias al uso de variables intermedias).

---

## 3. Tabla de Casos de Prueba (Trazabilidad)

| ID | Regla Validada | Entrada (SlotIndex, State, ML_Enabled) | Resultado Esperado (Menu Checked) |
|:---|:---|:---|:---|
| **TM-01** | [UI-MENU-CH-01] | Index 0, NS, ML=On | `No Slot` is Checked |
| **TM-02** | [UI-MENU-CH-02] | Index 1, UL, ML=On | `Unlinked` is Checked |
| **TM-03** | [UI-MENU-CH-03] | Index 2, LL, ML=On | `Left Linked` is Checked |
| **TM-04** | [UI-MENU-CH-04] | Index 3, RL (No DL), ML=On | `Right Linked` is Checked |
| **TM-05** | [UI-MENU-CH-05] | Index 4, RL (DL), ML=On | `Double Linked` is Checked |
| **TM-06** | [UI-MENU-EN-02] | Index 7 | `Left Linked` is Disabled |
| **TM-07** | [UI-MENU-EN-03] | Index 0 | `Right Linked` is Disabled |
| **TM-08** | [UI-MENU-EN-04] | Index 0, ML=On | `Double Linked` is Disabled |
| **TM-09** | [UI-MENU-EN-04] | Index 1, ML=Off | `Double Linked` is Hidden/Disabled |
