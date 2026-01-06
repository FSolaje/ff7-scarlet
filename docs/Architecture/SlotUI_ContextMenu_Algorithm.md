# Algoritmo de Menú Contextual de Slots (`SlotUI_ContextMenu`)

Este documento define el comportamiento esperado de las opciones del menú contextual (clic derecho) para cada slot de materia.

## 1. Reglas de Disponibilidad (Enabled)
Determinan qué opciones puede ver/clicar el usuario.

*   **[UI-MENU-EN-01]**: Las opciones `No Slot` y `Unlinked` siempre están habilitadas.
*   **[UI-MENU-EN-02]**: La opción `Left Linked` está deshabilitada en el último slot (índice 7).
*   **[UI-MENU-EN-03]**: La opción `Right Linked` está deshabilitada en el primer slot (índice 0).
*   **[UI-MENU-EN-04]**: La opción `Double Linked` está deshabilitada si:
    *   La configuración `isMultilinkEnabled` es `false`.
    *   **O** el slot es el primero (0) o el último (7).

## 2. Reglas de Marcación (Checked)
Determinan qué opción aparece con el "check" visual según el estado actual del slot.

| Regla ID | Estado Real (`MateriaSlot`) | IsDoubleLinked? | Opción Marcada |
|:---|:---|:---|:---|
| **[UI-MENU-CH-01]** | NS (None) | - | `No Slot` |
| **[UI-MENU-CH-02]** | UL (Unlinked) | - | `Unlinked` |
| **[UI-MENU-CH-03]** | LL (LeftLinked) | - | `Left Linked` |
| **[UI-MENU-CH-04]** | RL (RightLinked) | **False** | `Right Linked` |
| **[UI-MENU-CH-05]** | RL (RightLinked) | **True** | `Double Linked` |

---

## 3. Reglas de Sincronización
*   **[UI-MENU-SYNC-01]**: El menú debe refrescar su estado (`Checked`) inmediatamente después de que el valor de cualquier slot en la cadena cambie debido a una propagación.
