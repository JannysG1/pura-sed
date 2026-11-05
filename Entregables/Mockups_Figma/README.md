# Entregables de Diseño UI/UX y Mockups en Figma — PuraSed

**Cátedra:** ITID 803 — Diseño de Aplicaciones y Servicios para Internet  
**Docente:** Ing. Jose Naranjo | Escuela Politécnica Nacional (EPN - FIEE)  
**Semestre / Sprint:** Sprint 2 — Sistema Central PuraSed (Fase de Diseño & Mockups en Figma)  

**Equipo de Desarrollo:**
* **Angelo Conterón:** Frontend & UI/UX Lead
* **Ángel Chicaiza:** Backend & Data Architect
* **Jannys Garrido Pazmiño:** Scrum Master & Fullstack Developer

---

## 1. Organización del Taller de Diseño y Mockups

Este directorio es el **taller integral de prototipado frontend y mockups para Figma**. No es un simple depósito de imágenes; aquí se maquetan las interfaces con código ligero (HTML/CSS/JS, Three.js y animaciones motion) para luego convertirlas a Figma con herramientas de *HTML to Figma*, y finalmente exportar las pantallas terminadas:

```text
Entregables/Mockups_Figma/
├── README.md                      <-- Este documento formal de control y bitácora
├── taller_frontend/               <-- Taller de código para maquetación ágil de pantallas
│   └── 01_menu_orbital_pos.html   <-- Prototipo interactivo (3 columnas, Mucha Chicha -45°, ticket)
└── capturas_exportadas/           <-- Capturas oficiales de alta resolución (@2x) para evaluación
    ├── 01_Menu_POS_Principal.png  <-- Vista inicial de 3 columnas y paginador central
    └── 02_Mucha_Chicha_Detalle.png<-- Vista expandida con ticket artesanal y grabado andino
```

---

## 2. Enlaces Oficiales al Proyecto en Figma

Para revisión directa, inspección de capas vectoriales, componentes y pruebas del prototipo interactivo:

* **Tablero Oficial de Diseño (Figma Workspace):** `[INSERTAR_ENLACE_A_FIGMA_AQUÍ]`
* **Prototipo Interactivo (Modo Presentación):** `[INSERTAR_ENLACE_AL_PROTOTIPO_AQUÍ]`

---

## 3. Catálogo de Pantallas Oficiales Exportadas (`capturas_exportadas/`)

| Archivo | Pantalla / Módulo | Estado | Responsable |
| :--- | :--- | :---: | :--- |
| `01_Menu_POS_Principal.png` | Catálogo Tríada (POS) — Columnas, paginador central, botella -45° | ✅ Listo | Angelo Conterón |
| `02_Mucha_Chicha_Detalle.png` | Detalle Mucha Chicha — Botella centrada, ticket blanco 1.2°, Diablo Huma | ✅ Listo | Angelo Conterón |
| `03_Pida_No_Mas_Cocina.png` | Capítulo 1: Pida No Más — Cazuela locro de papa, adiciones y mesa | 🟡 En diseño | Jannys Garrido |
| `04_Lleve_Lleve_Mercado.png` | Capítulo 2: Lleve Lleve — Empaques al vacío de costillas y despensa | 🟡 En diseño | Ángel Chicaiza |
| `05_Si_Hay_Si_Hay_Merch.png` | Capítulo 3: Si Hay Si Hay — Camisetas Sereno Moreno, tallas y merch | 🟡 En diseño | Jannys Garrido |
| `06_Oro_Ancestral_Identidad.png` | Capítulo 4: Oro Ancestral — Máscara de Aya Huma y cultura | 🟡 En diseño | Angelo Conterón |
| `07_Comanda_Ticket_Detalle.png` | Ticket de Comanda / Cocina — Tarjeta troquelada, desglose y total | 🟡 En diseño | Equipo Scrum |

---

## 4. Metodología de Trabajo: Prototipado en HTML y Conversión a Figma ("HTML to Figma")

El equipo utiliza un flujo de trabajo ágil y desacoplado:
1. **Maquetación en `taller_frontend/`:** Se crea o ajusta la pantalla en HTML usando los tokens oficiales de la guía y las 25 imágenes depuradas de `PuraSed.Client/wwwroot/catalogo_imagenes/`.
2. **Three.js y Motion Graphics:** Si la pantalla requiere orientación tridimensional precisa (como la botella rotada a -45° o perspectiva de objetos), se usa Three.js cargado por CDN.
3. **Conversión a Figma (*html.to.design*):** Se abre el plugin en Figma, se carga el archivo HTML local y se generan automáticamente marcos nativos con capas vectoriales, textos y Auto Layout.
4. **Exportación PNG Oficial:** Una vez pulida la pantalla en Figma, se exporta a la carpeta `capturas_exportadas/` en escala `@2x` y se confirma en Git.
