# GoMad — Mockups y flujo

Archivos en esta carpeta:

- `mockup.html` — mockup interactivo de las pantallas: splash, onboarding (única vez), main (OFF/ON) y voluntarios.
- `style.css` — estilos del mockup (fuentes grandes, colores y touch targets).
- `flow.mmd` — diagrama Mermaid del flujo de la app.

Cómo abrir el mockup:

1. Abre `Design/mockup.html` en tu navegador (doble clic o `File > Open`).
2. Usa los botones de la pantalla de Splash para navegar entre las vistas.

Notas de accesibilidad y diseño:
- Tipografías grandes (mínimo 18–24pt para legibilidad); botones y micrófono con touch targets amplios (≥ 150px para el micrófono, 80px para acciones principales).
- Colores: verde principal `#228B22`, naranja acento `#FF8C00`, rojo `#FF3B30` para estado apagado del micrófono.
- El onboarding debe aparecer solo una vez; en la app real hay que persistir localmente (cookie / storage) la marca `OnboardCompleted`.

Si quieres, genero también el código equivalente en Flutter siguiendo exactamente estas medidas y estructura.
