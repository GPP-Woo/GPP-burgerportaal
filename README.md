# Burgerportaal

## Omgevingsvariabelen

| Variabele                                            | Uitleg                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                         |
| ---------------------------------------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `ODRC_BASE_URL`                                      | De base url van de Publicatiebank waarmee gekoppeld moet worden. <details> <summary>Meer informatie </summary>Bijvoorbeeld: `https://publicatiebank.mijn-gemeente.nl` </details>                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                               |
| `ODRC_API_KEY`                                       | De geheime sleutel voor de Publicatiebank waarmee gekoppeld moet worden. <details> <summary>Meer informatie </summary>Bijvoorbeeld: `VM2B!ccnebNe.M*gxH63*NXc8iTiAGhp`</details>                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                               |
| `SEARCH_BASE_URL`                                    | De base url van de Zoekcomponent waarmee gekoppeld moet worden. <details> <summary>Meer informatie </summary>Bijvoorbeeld: `https://zoekcomponent.mijn-gemeente.nl` </details>                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                 |
| `SEARCH_API_KEY`                                     | De geheime sleutel voor de Zoekcomponent waarmee gekoppeld moet worden. <details> <summary>Meer informatie </summary>Bijvoorbeeld: `VM2B!ccnebNe.M*gxH63*NXc8iTiAGhp`</details>                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                |
| `RESOURCES:PORTAAL_TITEL`                            | De titel van het burgerportaal. <details><summary>Meer informatie</summary> Bijvoorbeeld: Open Mijn Gemeente</details>                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                         |
| `RESOURCES:GEMEENTE_NAAM`                            | De naam van de gemeente die wordt gebruikt binnen het burgerportaal. <details><summary>Meer informatie</summary> Bijvoorbeeld: Mijn Gemeente</details>                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                         |
| `RESOURCES:GEMEENTE_DESIGN_TOKENS_URL`               | Publiek URL waar het css-bestand met NL Design System tokens beschikbaar is, om het burgerportaal te stylen in gemeentehuisstijl. <details><summary>Meer informatie </summary>Bijvoorbeeld: `https://unpkg.com/@gemeente/design-tokens/dist/index.css`</details>                                                                                                                                                                                                                                                                                                                                                                                                                               |
| `RESOURCES:GEMEENTE_WEB_FONT_SOURCES`                | Publiek URL - of meerdere door spaties gescheiden publieke URL's - als verwijzing naar web-font-bestand(en) horend bij de gemeentehuisstijl. <details><summary>Meer informatie </summary>Bijvoorbeeld: `https://fonts.mijn-gemeente.nl/custom-regular-font.woff2 https://fonts.mijn-gemeente.nl/custom-bold-font.woff2`. Een enkele verwijzing naar de locatie waar alle font-style-bestanden staan kan ook: `https://fonts.mijn-gemeente.nl/custom-font/`. **Let op:** deze configuratie is alleen bedoeld om de font-bestanden onder CORS te kunnen inladen. Verwijzingen naar bestanden zullen ook in de theme styling onder een @font-face ruleset gespecificeerd moeten worden.</details> |
| `RESOURCES:GEMEENTE_THEME_NAAM`                      | De naam van de selector uit het css-bestand die wordt gebruikt om de NLDS-tokens te scopen. <details><summary>Meer informatie</summary> Bijvoorbeeld: `gemeente-theme` </details>                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                              |
| `RESOURCES:GEMEENTE_WEBSITE_URL`                     | Het website-adres van de gemeente, om vanuit het burgerportaal naar de website van de gemeente te linken. <details><summary>Meer informatie</summary> Bijvoorbeeld: `https://www.mijn-gemeente.nl`</details>                                                                                                                                                                                                                                                                                                                                                                                                                                                                                   |
| `RESOURCES:GEMEENTE_PRIVACY_URL`                     | Het website-adres waar de privacyverklaring van de gemeente staat. Wordt gebruikt om vanuit burgerportaal naar te linken. <details><summary>Meer informatie</summary> Bijvoorbeeld: `https://www.mijn-gemeente.nl/privacy`</details>                                                                                                                                                                                                                                                                                                                                                                                                                                                           |
| `RESOURCES:GEMEENTE_CONTACT_URL`                     | Het website-adres waar contactgegevens van de gemeente te vinden zijn. Wordt gebruikt om vanuit burgerportaal naar te linken. <details><summary>Meer informatie</summary> Bijvoorbeeld: `https://www.mijn-gemeente.nl/contact`</details>                                                                                                                                                                                                                                                                                                                                                                                                                                                       |
| `RESOURCES:TOEGANKELIJKHEIDSVERKLARING_REGISTER_URL` | Het website-adres van het overheidsregister van toegankelijkheids­verklaringen. Wordt gebruikt om vanuit burgerportaal naar te linken. <details><summary>Meer informatie</summary> Waarschijnlijk: `https://www.toegankelijkheidsverklaring.nl/register`</details>                                                                                                                                                                                                                                                                                                                                                                                                                             |
| `DOWNLOAD_TIMEOUT_MINUTES`                           | Het aantal minuten dat het downloaden van bestanden maximaal mag duren. <br/> (default waarde is `10`)                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                         |
| `SITEMAP_CACHE_DURATION_HOURS`                       | Het aantal uur dat de sitemap in de cache bewaard wordt. <br/> (default waarde is `23`)                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        |
| `BLOCK_ROBOTS`                                       | Waarde (boolean) die aangeeft of zoekmachines/robots geblokkeerd moeten worden zodat site niet geïndexeerd wordt. <br/> (default waarde is `false`)                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                            |

## Beheer

Het burgerportaal heeft een beheeromgeving die toegankelijk is via `/beheer`.

### Welkomsttekst

De welkomsttekst op de homepage kan worden beheerd via de beheeromgeving. De tekst kan worden opgemaakt, het gebruik van alinea's, titels en sub-titels, opsommingen (genummerd en ongenummerd) en linkjes wordt ondersteund.

Als er geen welkomsttekst is ingesteld in de beheeromgeving, wordt een standaard welkomstbericht getoond op basis van de gemeentenaam (`RESOURCES:GEMEENTE_NAAM`).

### Video

Een promotie- of instructievideo kan worden ingesteld via de beheeromgeving. De URL moet een geldige YouTube of Vimeo **embed URL** zijn:

- YouTube: `https://www.youtube.com/embed/VIDEO_ID`
- Vimeo: `https://player.vimeo.com/video/VIDEO_ID`

Bij het opslaan wordt gevalideerd of de video beschikbaar is. De video wordt getoond op de homepage van het burgerportaal.

**Let op:** De embedder is verantwoordelijk voor juiste toegankelijkheid van video content, inclusief ondertiteling en toetsenbordnavigatie.

### Afbeeldingen

Het logo, favicon en de sfeerfoto kunnen worden beheerd via de beheeromgeving. Per afbeelding gelden de volgende beperkingen:

| Afbeelding | Toegestane formaten      | Maximale grootte |
| :--------- | :----------------------- | :--------------- |
| Logo       | SVG, PNG, JPG, GIF, WebP | 2 MB             |
| Favicon    | ICO, SVG, PNG            | 512 KB           |
| Sfeerfoto  | SVG, PNG, JPG, GIF, WebP | 5 MB             |

Als er geen afbeelding is geüpload, wordt een standaard afbeelding getoond.

## Cross-Origin Resource Sharing (CORS) en Cross-Origin-Embedder-Policy (COEP)

Deze applicatie maakt gebruik van Cross-Origin-Embedder-Policy (COEP: require-corp), maar de externe resources worden geladen onder CORS (met cross-origin-attributen). Dat betekent dat die externe resources de juiste Access-Control-Allow-Origin-header moeten bevatten.

**Video embedding:** COEP wordt uitgeschakeld wanneer een video is ingesteld via de beheeromgeving, omdat YouTube en Vimeo geen COEP-headers ondersteunen. Zie dit [decision record](./coep_video.md) voor de volledige afweging.

### Headers:

Voor alle externe resources moet `Access-Control-Allow-Origin: *` of bijvoorbeeld `Access-Control-Allow-Origin: *.mijn-gemeente.nl` ingesteld worden.

Als een resource niet correct is geconfigureerd, zal deze niet geladen worden door de browser. **Let op:** met uitzondering van de geconfigureerde Web fonts kunnen geconfigureerde externe resources geen verwijzingen bevatten naar andere externe resources of data URIs.

## NLDS – NL Design System

De interface van het Burgerportaal is opgebouwd met componenten uit het **NL Design System (NLDS)**. Dit is een verzameling ontwerp- en ontwikkelrichtlijnen voor digitale overheidsdiensten in Nederland. Door gebruik te maken van NLDS-componenten blijft de gebruikerservaring consistent en toegankelijk, in lijn met de standaarden van de overheid.

🔗 Meer informatie: [Introductie NLDS](https://nldesignsystem.nl/handboek/introductie)

### Aanpasbaarheid voor gemeentes

Dankzij NLDS kunnen verschillende installaties van het Burgerportaal eenvoudig worden aangepast aan de huisstijl van diverse gemeentes. Dit wordt mogelijk gemaakt door het gebruik van design tokens, die de stijlkenmerken zoals kleuren, typografie en componenten bepalen.

### Implementatie op basis van Utrecht Design System

Op dit moment is de implementatie gebaseerd op alleen het Utrecht Design System, een specifieke variant van NLDS. Voor een correcte weergave en de beste resultaten moeten ten minste de Brand en Common tokens correct ingevuld zijn.

### Gebruikte CSS-componenten

| Component                                           | Storybook                                                                                                                      |
| :-------------------------------------------------- | :----------------------------------------------------------------------------------------------------------------------------- |
| **Document** (`utrecht-document`)                   | [🔗 Design Tokens](https://nl-design-system.github.io/utrecht/storybook/?path=/story/css_css-document--design-tokens)          |
| **Surface** (`utrecht-surface`)                     | [🔗 Design Tokens](https://nl-design-system.github.io/utrecht/storybook/?path=/story/css_css-surface--design-tokens)           |
| **Page** (`utrecht-page`)                           | [🔗 Design Tokens](https://nl-design-system.github.io/utrecht/storybook/?path=/story/css_css-page--design-tokens)              |
| **Page header** (`utrecht-page-header`)             | [🔗 Design Tokens](https://nl-design-system.github.io/utrecht/storybook/?path=/story/css_css-page-header--design-tokens)       |
| **Page content** (`utrecht-page-content`)           | [🔗 Design Tokens](https://nl-design-system.github.io/utrecht/storybook/?path=/story/css_css-page-content--design-tokens)      |
| **Page footer** (`utrecht-page-footer`)             | [🔗 Design Tokens](https://nl-design-system.github.io/utrecht/storybook/?path=/story/css_css-page-footer--design-tokens)       |
| **Navigation bar** (`utrecht-nav-bar`)              | [🔗 Design Tokens](https://nl-design-system.github.io/utrecht/storybook/?path=/story/css_css-nav-bar--design-tokens)           |
| **Link** (`utrecht-link`)                           | [🔗 Design Tokens](https://nl-design-system.github.io/utrecht/storybook/?path=/story/css_css-link--design-tokens)              |
| **Skip link** (`utrecht-skip-link`)                 | [🔗 Design Tokens](https://nl-design-system.github.io/utrecht/storybook/?path=/story/css_css-skip-link--design-tokens)         |
| **Article** (`utrecht-article`)                     | [🔗 Design Tokens](https://nl-design-system.github.io/utrecht/storybook/?path=/story/css_css-article--design-tokens)           |
| **Heading** (`utrecht-heading`)                     | [🔗 Design Tokens](https://nl-design-system.github.io/utrecht/storybook/?path=/story/css_css-heading-1--design-tokens)         |
| **Paragraph** (`utrecht-paragraph`)                 | [🔗 Design Tokens](https://nl-design-system.github.io/utrecht/storybook/?path=/story/css_css-paragraph--design-tokens)         |
| **Unordered list** (`utrecht-unordered-list`)       | [🔗 Design Tokens](https://nl-design-system.github.io/utrecht/storybook/?path=/story/css_css-unordered-list--design-tokens)    |
| **Button** (`utrecht-button`)                       | [🔗 Design Tokens](https://nl-design-system.github.io/utrecht/storybook/?path=/story/css_css-button--design-tokens)            |
| **Form field** (`utrecht-form-field`)               | [🔗 Design Tokens](https://nl-design-system.github.io/utrecht/storybook/?path=/story/css_css-form-field--design-tokens)        |
| **Form label** (`utrecht-form-label`)               | [🔗 Design Tokens](https://nl-design-system.github.io/utrecht/storybook/?path=/story/css_css-form-label--design-tokens)        |
| **Textbox** (`utrecht-textbox`)                     | [🔗 Design Tokens](https://nl-design-system.github.io/utrecht/storybook/?path=/story/css_css-textbox--design-tokens)           |
| **Table** (`utrecht-table`)                         | [🔗 Design Tokens](https://nl-design-system.github.io/utrecht/storybook/?path=/story/css_css-table--design-tokens)             |
| **Logo** (`utrecht-logo`)                           | [🔗 Design Tokens](https://nl-design-system.github.io/utrecht/storybook/?path=/story/css_css-logo--design-tokens)              |
| **Spotlight section** (`utrecht-spotlight-section`) | [🔗 Design Tokens](https://nl-design-system.github.io/utrecht/storybook/?path=/story/css_css-spotlight-section--design-tokens) |

### GPP Woo Theme

Naast de bovenstaande componenten bestaat de interface van het Burgerportaal ook uit verschillende custom componenten en elementen. Deze (gpp-woo) componenten en elementen kunnen via een aantal voorgedefinieerde css-variabelen worden aangepast voor een uniforme uitstraling binnen de huisstijl van de gemeente.

Ter referentie [`gpp-woo-theme`](./odbp.client/src/assets/_mixin-theme.scss).
