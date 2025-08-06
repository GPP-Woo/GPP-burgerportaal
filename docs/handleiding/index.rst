.. _handleiding_index:

Inrichting van het Burgerportaal
==============================

Vormgeving en links naar de gemeentelijk website
-------------------------------------------------

Het GPP-Burgerportaal, maakt gebruik van het `NL Design System (NLDS) <https://nldesignsystem.nl/>`_. Daarnaast zijn er diverse onderdelen van de interface die gemeente-specifiek ingericht kunnen worden. 

Let op: het is belangrijk om na te denken over deze onderdelen vóórdat een gemeente het GPP-Burgerportaal gaat installeren. Zie ook `het stappenplan op de community website <https://www.gpp-woo.nl/implementatie>`_. 

In de `lijst met omgevingsvariabelen <https://github.com/GPP-Woo/GPP-burgerportaal?tab=readme-ov-file#burgerportaal>`_ staat uitgelegd welke gegevens het Burgerportaal nodig heeft van de gemeente, om aan te sluiten bij de gemeentelijke website. Het gaat dan om die variabelen die beginnen met ``RESOURCES:``

Informatie op de homepage
-----------------------------
Het is mogelijk om op de homepage van het Burgerportaal gemeentespecifieke informatie te tonen. Deze informatie moet zijn opgemaakt in HTML-format. Hierin is beperkte opmaak mogelijk: kopjes van 2 niveaus, links naar andere websites en genummerde of bullet-lijsten. Dit kan met de volgende HTML-elementen: ``<h1>``, ``<h2>``, ``<p>``, ``<a>``, ``<ul>``, ``<ol>``, ``<li>``.

Video 
^^^^^^
Het is ook mogelijk om een video met uitleg te plaatsen op de homepage. Dit moet een video zijn vanaf Vimeo of vanaf YouTube. De verwijzing naar die URL moet ingesteld worden in de omgevingsvariabelen. Als er een video-url wordt ingevuld, zal deze naast de gemeentespecifieke informatie getoond worden. Als er géén video is, wordt deze informatie over de breedte van de homepage getoond. Let op: het gebruik van een video heeft invloed op de Cross-Origin-Embedder-Policy. Meer informatie hierover staat `in de Readme van de repository <https://github.com/GPP-Woo/GPP-burgerportaal?tab=readme-ov-file#cross-origin-resource-sharing-cors-en-cross-origin-embedder-policy-coep>`_.


Lokale thema's: onderwerpen
---------------------------
Op de voorpagina van het Burgerportaal staat een caroussel waarin belangrijke lokale thema's worden getoond. Daarnaast is er in het menu in de bovenbalk een link naar alle thema's van de gemeente. 

De inhoud van de thema's komt geheel uit de GPP-Publicatiebank. Deze staan in de Publicatiebank onder Onderwerpen. Meer informatie hierover vindt u in de `documentatie van de GPP-Publicatiebank <https://gpp-publicatiebank.readthedocs.io/en/latest/admin/publicaties/index.html#onderwerpen>`_.

Sitemap t.b.v. landelijke Woo-index
-----------------------------------
Voor een correct werkende aansluiting op de `landelijke Woo-index <https://open.overheid.nl/>`_ moeten de openbare documenten vermeld worden in een sitemap. Het GPP-Burgerportaal genereert deze sitemaps, zodat de `Woo-harvester <https://standaarden.overheid.nl/diwoo/>`_ ze kan overnemen. De sitemap wordt iedere 24 uur geactualiseerd. De sitemap is te vinden op `domeinnaam-van-het-burgerportaal\robots.txt`. 
