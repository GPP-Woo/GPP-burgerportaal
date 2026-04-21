.. _handleiding_index:

Handleiding
==============================

- Vormgeving bij installatie
- Vormgeving en beheer-fuctionaliteit
- Zoeken, vinden en raadplegen voor burgers
- Aansluiting op de landelijke voorziening

.. _handleiding_index_vormgevingbijinstallatie

Vormgeving bij installatie
---------------------------

Het GPP-Burgerportaal, maakt gebruik van het `NL Design System (NLDS) <https://nldesignsystem.nl/>`_. Daarnaast zijn er diverse onderdelen van de interface die organisatie-specifiek ingericht kunnen worden. 

In de `lijst met omgevingsvariabelen <https://github.com/GPP-Woo/GPP-burgerportaal?tab=readme-ov-file#burgerportaal>`_ staat uitgelegd welke gegevens het Burgerportaal nodig heeft van de organisatie, om aan te sluiten bij de huistijl van de website. Het gaat dan om die variabelen die beginnen met ``RESOURCES:``

Let op: het is belangrijk om na te denken over deze onderdelen vóórdat een organisatie het GPP-Burgerportaal gaat installeren. Zie ook `het stappenplan op de community website <https://www.gpp-woo.nl/implementatie>`_. 

.. _handleiding_index_vormgevingenbeheer

Vormgeving en beheer-fuctionaliteit
------------------------------------

Het GPP-burgerprortaal beschikt over een beheerscherm waarmee de welkomsttekst en video op de homepage, afbeeldingen en URL's die op de webiste getoond worden, beheerd kunnen worden.

Hoe krijgt een gebruiker toegang tot het beheerscherm?
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^

Het beheerscherm kan gevonden worden door in de browser in de navigatiebalk de URL van het burgerportaal in te voeren, aangevuld met *"/beheer"*. Bijvoorbeeld: `https://burgerprotaal.gemeente.nl/beheer`. 
De gebruiker wordt vervolgens gevraagd om in te loggen.

Om een gebruiker beheerders-rechten te geven, moet deze een specifieke rol krijgen in de OpenID Connect Identity Provider (bijv. Azure AD). 
De naam van deze rol moet zijn afgestemd met de beheerders van de Identity Provider, en bij installatie van de App zijn ingeregeld. 
Neem hiervoor contact op met de beheerders van de Identity Provider.

Na inloggen ziet de beheerder rechtsboven de volgende menu-items, die we hieronder verder toelichten:

- Homepage
- Afbeeldingen
- Externe links
- Uitloggen

Homepage
^^^^^^^^

Op het beheerscherm kunnen onder het menu-item "Hompage" twee items gewijzigd worden:

- Welkomsttekst
   Dit is de welkomsttekst die op de homepage van het Burgerprotaal aan de burger wordt getoond. Het is mogelijk om de tekst op te maken met titels (*heading 1* en *heading 2*), hyperlinks, genummerde en ongenummerde lijsten.

- Promotie- of instructievideo
   Op de homepage kan ook een embedded Youtube- of Vimeo-video getoond worden aan de burger. Dit is optioneel. 
   Zo'n video kan helpen om aan de burger duidelijk te maken wat de Wet open overheid is en/of wat hij/zij/hen kan vinden op het Burgerportaal.
   Bij het veld staat een toelichting achter een (?)-knop over hoe de video-URL samengesteld moet zijn om correct te werken.

Onderaan het scherm staan knoppen om de wijzigingen op te slaan of de wijzigingen te annuleren.

.. warning:: Let op: Cross-Origin-Embedder-Policy wordt uitgeschakeld wanneer een video is ingesteld. De embedder is verantwoordelijk voor juiste toegankelijkheid van video content, inclusief ondertiteling en toetsenbordnavigatie. 

Afbeeldingen
^^^^^^^^^^^^^

Op het beheerscherm kunnen onder het menu-item "Afbeeldingen" drie items gewijzigd worden:

- Logo organisatie
   Hier kan het logo van de organisatie geüpload worden. Deze wordt op het Burgerportaal op iedere webpagina linksboven getoond. 
   De afbeelding wordt geschaald zodat deze volledig getoond wordt. Hoeveel ruimte er is voor het logo wordt door de NLDS-tokens bepaald.

- Favicon
   Hier kan het favicon van het Burgerprortaal geüpload worden. Dit is het icoon dat in browsers inde navigatiebalk of het browser-tabblad wordt getoond.

- Sfeerfoto
   Hier kan de sfeerfoto geüpload worden. Deze foto wordt op het Burgerportaal op iedere webpagina getoond als horizontale balk onder het organistaie-logo en het menu. 
   De foto wordt geschaald en bijgesneden (een gelijk deel boven en onder) zodat de horizontale balk geheel gevuld wordt.

Onder ieder item staat een knop "Vervangen" waarmee een lokaal opgeslagen afbeelding geslecteerd en geüpload kan worden.
Bij ieder item staat een toelichting achter een (?)-knop met informatie over o.a. geaccepteerde bestandsformaten en bestandsgrootte.

.. tip:: Check na het uploaden van een afbeelding het resultaat op het Burgerportaal. is het resultaat niet geheel naar wens? Gebruik dan software om de foto of afbeelding te bewerken, upload de afbeelding opnieuw en check het resutaat. Soms vergt het enig geëxpermenteer om tot een tevredenstellend resultaat te komen.

Externe links
^^^^^^^^^^^^^

Op het beheerscherm kunnen onder het menu-item "Externe links" vier items gewijzigd worden:

- URL Website organisatie
   Dit is de URL naar de (hoofd-)website van de organisatie. 
   Deze URL wordt geopend wanneer de burger op het Burgerportaal in het menu op het menu-item "Naar de gemeente" klikt.
   Het menu-item wordt niet getoond als in het beheerscherm geen URL hiervoor is ingevoerd.
   De naam van het menu-item kan bij de installatie van bet Burgerportaal gewijzigd worden naar: "Naar de provincie", "Naar het Waterschap", etc. via de omgevingsvariabele `RESOURCES:ORGANISATIE_TYPE`.

- URL Toegankelijkheidsverklaring
   Dit is de URL naar de toegankelijkheidsverklaring van het Burgerportaal. Deze staat bijvoorbeeld op `https://www.digitoegankelijk.nl/dashboard`.
   Deze URL wordt geopend wanneer de burger op het Burgerportaal in de voetbalk onderaan op de link "Toegankelijkheid" klikt.
   Deze link wordt niet getoond als in het beheerscherm geen URL hiervoor is ingevoerd.

- URL Privacy-verklaring
   Dit is de URL naar de privacy-verklaring van de organisatie. Deze staat bijvoorbeeld op de (hoofd-)website van de gemeente.
   Deze URL wordt geopend wanneer de burger op het Burgerportaal in de voetbalk onderaan op de link "Privacy" klikt.
   Deze link wordt niet getoond als in het beheerscherm geen URL hiervoor is ingevoerd.

- URL Contact-pagina
   Dit is de URL naar de contactgegevens van de organisatie. Deze staat bijvoorbeeld op de (hoofd-)website van de gemeente.
   Deze URL wordt geopend wanneer de burger op het Burgerportaal in de voetbalk onderaan op de link "Contact" klikt.
   Deze link wordt niet getoond als in het beheerscherm geen URL hiervoor is ingevoerd.

Onderaan het scherm staan knoppen om de wijzigingen op te slaan of de wijzigingen te annuleren.

Uitloggen
^^^^^^^^^^

Door op dit menu-item te klikken wordt de beheerder uitgelogd.

.. _handleiding_index_voorburgers

Zoeken, vinden en raadplegen voor burgers
------------------------------------------

**Nog invullen**

.. _handleiding_index_aansluitinggwv

Aansluiting op de landelijke voorziening
-----------------------------------------

Voor een correct werkende aansluiting op de `Generieke Woo-Voorzieing (GWV) <https://open.overheid.nl/>`_ moeten de openbare documenten vermeld worden in een sitemap. Het GPP-Burgerportaal genereert deze sitemaps, zodat de `Woo-harvester <https://standaarden.overheid.nl/diwoo/>`_ ze kan overnemen. De sitemap wordt iedere 24 uur geactualiseerd. De sitemap is te vinden op ``https://domeinnaam-van-het-burgerportaal/robots.txt``, en voldoet aan het het schema ``diwoo-metadata.xsd`` versie 0.9.8.
