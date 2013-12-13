Examonitor
=======================
<h3>Administrator configuratie</h3>
<p>Dit email adres wordt gebruikt voor het versturen van bevestingingsmails.Ook de link voor het resetten van het wachtwoord worden hiernaar gestuurd</p>
<p>Om het adres te wijzigen dient het volgende XML bestand te worden aangepast: web.config.</p>

<p><b>
&lt;appSettings&gt;<br />
    &lt;add key=&quot;AdminEmail&quot; value=&quot;examonitor@ap.be&quot; /&gt;<br />
&lt;/appSettings&gt;<br />
</b></p>


<h3>Mail server instellen</h3>
<p>Om het versturen van mails door de applicatie mogelijk te maken dienen de volgende instellingen te worden aangepast aan de configuratie van de server.</p>
<p><b>
&lt;system.net&gt;<br />
    &lt;mailSettings&gt;<br />
      &lt;smtp deliveryMethod=&quot;SpecifiedPickupDirectory&quot;&gt; <br />
        &lt;specifiedPickupDirectory pickupDirectoryLocation=&quot;c:\email&quot; /&gt; <br />
        &lt;network host=&quot;localhost&quot; /&gt;<br />
      &lt;/smtp&gt;<br />
    &lt;/mailSettings&gt;<br />
  &lt;/system.net&gt;<br />
</b></p>

<h3>Concept</h3>
<p>De applicatie maakt het mogelijk om toezichtsbeurten te registreren voor de docenten. Docenten dienen een account te registreren, daarna kunnen ze inloggen.Eenmaal ingelogd krijgen ze een lijst van examens met het benodigde aantal toezichters, ze kunnen door middel van een knop zich al toezichter registreren aan dat bepaalde examen.</p>
<p>De administrator kan examens aanmakken alsook campussen.</p>

<h3>Import</h3>
<p>De administrator kan een json bestand importeren om de verschillende examens en campussen sneller toe te voegen.</p>

<h3>Berichten<h3>
<p>De administrator kan een bericht ingeven dat aan alle gebruikers wordt getoond. Deze berichten worden beheerd via een aparte pagina in het adminpaneel, zo kunnen berichten: worden aangemaakt en verwijderd en worden gemaakt.</p>
>

