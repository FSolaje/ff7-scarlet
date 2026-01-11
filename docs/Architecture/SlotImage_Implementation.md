# Implementación de Imagen de Slots (`SlotImage.cs`)

Este documento detalla la implementación técnica del [Algoritmo de Imagen de Slots](SlotImage_Algorithm.md) en C#.

## 1. Responsabilidad
La clase estática `SlotImage` actúa como un **Mapeador de Recursos**. Su única responsabilidad es traducir el estado lógico de un slot (definido por `MateriaSlot`, `IsDoubleLinked` y tipo de materia) a un identificador de recurso concreto (`MateriaSlotResource`).

## 2. Lógica Unificada (`GetSlotResource`)

Se ha consolidado toda la lógica de selección en un único método maestro que construye el nombre del recurso dinámicamente.

### 2.1 Construcción del Nombre
El nombre del recurso se genera siguiendo el patrón:
`materia_slot_[tipo_materia]_[sufijo]`

1.  **Prefijo**:
    *   Si hay materia: `materia_slot_{materiaType}` (ej: `materia_slot_magic`).
    *   Si no hay materia: `materia_slot`.

2.  **Sufijo Unificado (`GetUnifiedSuffix`)**:
    Determina el sufijo numérico o especial basándose en el estado del enlace y la presencia de materia.

    *   **Con Materia**: Se ignoran las variantes "Empty" (4-6). Se usan solo los sufijos estándar (1, 2, 3, _dl).
    *   **Sin Materia**: Se distingue entre variantes Normales (1-3) y Empty (4-6) según el valor exacto del enum `MateriaSlot`.

### 2.2 Resolución Dinámica
El string construido se convierte al enum `MateriaSlotResource` mediante `Enum.TryParse`. Si el recurso no existe, se devuelve el recurso por defecto (`materia_slot0`).

## 3. Desacoplamiento Visual
Se han separado dos métodos clave para facilitar el testing:
1.  `GetSlotResource(...)`: Retorna el `enum`, permitiendo tests unitarios rápidos y deterministas sin cargar gráficos.
2.  `GetImageFromResource(...)`: Encapsula la llamada a `Properties.Resources.ResourceManager`.

## 4. Trazabilidad
*   **[SIMAGE-MAT-*]**: Implementado en la rama `if (hasMateria)` de `GetUnifiedSuffix`.
*   **[SIMAGE-EMPTY-*]**: Implementado en la rama `else` de `GetUnifiedSuffix` mediante una expresión `switch`.
